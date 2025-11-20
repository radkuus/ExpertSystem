using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Auxiliary;
using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.Helpers.IfThen;
using ExpertSystem.WPF.Helpers.Sample;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Datasets;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels.Factories;

namespace ExpertSystem.WPF.ViewModels
{
    public class AnalysisViewModel : BaseViewModel
    {
        private readonly INavigator _navigator;
        private readonly IExpertSystemViewModelFactory _viewModelAbstractFactory;
        private readonly IAuthenticator _authenticator;
        private readonly IDatasetService _datasetService;
        private readonly IDatasetStore _datasetStore;
        private readonly IDialogService _dataFrameDialogService;
        private readonly IDatasetStatisticsService _dataStatisticsService;
        private readonly IDialogService _resultsDialog;
        private readonly IApiService _apiService;
        private readonly IExperimentService _experimentService;
        private readonly CreateViewModel<ResultsViewModel> _resultsFactory;
        private readonly MainViewModel _mainViewModel;
        private readonly UserSample _userSample;
        private bool _isLoading;
        private bool _isKnnChecked;
        private bool _isLogisticRegressionChecked;
        private bool _isBayesChecked;
        private bool _isNeuralNetworkChecked;
        private bool _isIfThenChecked;
        private string _loadingMessage;
        private string _selectedNeighbours;
        private string _selectedLayers;
        private string _selectedTrainingSetPercentage;
        private string _selectedDistanceMetrics;
        private Dataset _selectedDataset;
        private ObservableCollection<string> _selectedColumnsForAnalysis = new ObservableCollection<string>();
        private string _selectedResultColumn;
        private ObservableCollection<string> _datasetColumnNames;
        private ObservableCollection<string> _datasetNumericColumnNames;
        private ObservableCollection<string> _selectedModels = new ObservableCollection<string>();
        private ObservableCollection<string> _uniqueNamesFromClassifyingColumn = new ObservableCollection<string>();
        public UserSample UserSample => _userSample;
        public ObservableCollection<string> Operators => IfThenOperators.Operators;
        public ObservableCollection<Dataset> UserDatasets => _datasetStore.UserDatasets;
        public ObservableCollection<IfThenConditionGroup> IfThenConditions { get; set; } = new ObservableCollection<IfThenConditionGroup>();
        public ObservableCollection<NeuronLayer> NeuronCounts { get; set; } = new ObservableCollection<NeuronLayer>();

        public ICommand UpdateCurrentViewModelCommand { get; }
        public ICommand DisplayDatasetAsDataFrameAndStatisticsCommand { get; }
        public ICommand LoadDatasetColumnNamesCommand { get; }
        public ICommand AddSampleCommand { get; }
        public ICommand RemoveIfConditionCommand { get; }
        public ICommand GenerateResultsCommand { get; }
        public ICommand RemoveSampleCommand { get; }
        public ICommand AddIfConditionCommand { get; }
        public ICommand AddAndConditionCommand { get; }
        public ICommand RemoveAndConditionCommand { get; }
        public ICommand AddThenInConditionCommand { get; }
        public ICommand RemoveThenInConditionCommand { get; }

        public AnalysisViewModel(INavigator navigator, IExpertSystemViewModelFactory viewModelAbstractFactory, IAuthenticator authenticator, IDatasetService datasetService, IDatasetStore datasetStore,
            IDialogService dataFrameDialogService, IDatasetStatisticsService dataStatisticsService, IDialogService resultsDialog, IApiService apiService, IExperimentService experimentService, CreateViewModel<ResultsViewModel> resultsFactory, MainViewModel mainViewModel, UserSample userSample
)
        {
            _navigator = navigator;
            _viewModelAbstractFactory = viewModelAbstractFactory;
            _authenticator = authenticator;
            _datasetService = datasetService;
            _datasetStore = datasetStore;
            _dataFrameDialogService = dataFrameDialogService;
            _dataStatisticsService = dataStatisticsService;
            _resultsDialog = resultsDialog;
            _apiService = apiService;
            _experimentService = experimentService;
            _resultsFactory = resultsFactory;
            _mainViewModel = mainViewModel;
            _userSample = userSample;

            ErrorMessageViewModel = new MessageViewModel();
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelAbstractFactory);
            DisplayDatasetAsDataFrameAndStatisticsCommand = new DisplayDatasetAsDataFrameAndStatisticsCommand(datasetService, dataFrameDialogService, dataStatisticsService, this);
            LoadDatasetColumnNamesCommand = new LoadDatasetColumnNamesCommand(datasetService, this);
            AddSampleCommand = new AddSampleCommand(this);
            RemoveIfConditionCommand = new RemoveIfConditionCommand(this);
            GenerateResultsCommand = new GenerateResultsCommand(this, resultsDialog, datasetService, apiService, experimentService, resultsFactory, navigator, mainViewModel);
            RemoveSampleCommand = new RemoveSampleCommand(this);
            AddIfConditionCommand = new AddIfConditionCommand(this);
            AddAndConditionCommand = new AddAndConditionCommand(this);
            RemoveAndConditionCommand = new RemoveAndConditionCommand(this);
            AddThenInConditionCommand = new AddThenInConditionCommand(datasetService, this);
            RemoveThenInConditionCommand = new RemoveThenInConditionCommand(this);

            SelectedColumnsForAnalysis.CollectionChanged += (s, e) => UpdateColumnsForAnalysis();
            _userSample.UserSamples.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanGenerateResultUserSamples));
                OnPropertyChanged(nameof(CanGenerateResult));
            };
            _userSample.AddNewSampleCallback = () =>
            {
                OnPropertyChanged(nameof(CanGenerateResultUserSamples));
                OnPropertyChanged(nameof(CanGenerateResult));
            };

            IfThenConditions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CanGenerateResultIfThen));
            IfThenConditions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CanGenerateResult));
            IfThenConditions.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (IfThenConditionGroup group in e.NewItems)
                    {
                        SubscribeToGroup(group);
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (IfThenConditionGroup group in e.OldItems)
                    {
                        UnsubscribeFromGroup(group);
                    }
                }

                OnPropertyChanged(nameof(CanGenerateResultIfThen));
                OnPropertyChanged(nameof(CanGenerateResult));
            };
        }
        private void SubscribeToGroup(IfThenConditionGroup group)
        {
            group.Conditions.CollectionChanged += ConditionsCollectionChanged;

            foreach (var condition in group.Conditions)
            {
                condition.PropertyChanged += ConditionPropertyChanged;
            }
        }

        private void UnsubscribeFromGroup(IfThenConditionGroup group)
        {
            group.Conditions.CollectionChanged -= ConditionsCollectionChanged;

            foreach (var condition in group.Conditions)
            {
                condition.PropertyChanged -= ConditionPropertyChanged;
            }
        }

        private void ConditionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (IfThenCondition condition in e.NewItems)
                {
                    condition.PropertyChanged += ConditionPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (IfThenCondition condition in e.OldItems)
                {
                    condition.PropertyChanged -= ConditionPropertyChanged;
                }
            }

            OnPropertyChanged(nameof(CanGenerateResultIfThen));
            OnPropertyChanged(nameof(CanGenerateResult));
        }

        private void ConditionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanGenerateResultIfThen));
            OnPropertyChanged(nameof(CanGenerateResult));
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public string LoadingMessage
        {
            get => _loadingMessage;
            set
            {
                if (_loadingMessage != value)
                {
                    _loadingMessage = value;
                    OnPropertyChanged(nameof(LoadingMessage));
                }
            }
        }


        public bool IsKnnChecked
        {
            get => _isKnnChecked;
            set
            {
                if (_isKnnChecked != value)
                {
                    _isKnnChecked = value;
                    OnPropertyChanged(nameof(IsKnnChecked));
                    OnPropertyChanged(nameof(IsModelWithParametersChecked));
                    OnPropertyChanged(nameof(IsAnyModelAndColumnForAnalysisChecked));
                    OnPropertyChanged(nameof(IsAnyModelWithoutIfThenAndColumnForAnalysisChecked));
                    OnPropertyChanged(nameof(AreDetailsChecked));
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResult)); 
                    OnPropertyChanged(nameof(IsAnyModelWithoutIfThenChecked));
                    OnPropertyChanged(nameof(CanViewUserSample));
                    UpdateSelectedModels();
                }
                if (!_isKnnChecked)
                {
                    SelectedNeighbours = null;
                    SelectedResultColumn = null;
                    SelectedDistanceMetrics = null;
                    SelectedTrainingSetPercentage = null;
                    _userSample.UserSamples.Clear();
                }
            }
        }

        public string SelectedNeighbours
        {
            get => _selectedNeighbours;
            set
            {
                _selectedNeighbours = value;
                OnPropertyChanged(nameof(SelectedNeighbours));
                OnPropertyChanged(nameof(AreDetailsChecked));
                OnPropertyChanged(nameof(CanGenerateResult));
            }
        }

        public bool IsNeuralNetworkChecked
        {
            get => _isNeuralNetworkChecked;
            set
            {
                if (_isNeuralNetworkChecked != value)
                {
                    _isNeuralNetworkChecked = value;
                    OnPropertyChanged(nameof(IsNeuralNetworkChecked));
                    OnPropertyChanged(nameof(IsModelWithParametersChecked));
                    OnPropertyChanged(nameof(IsAnyModelAndColumnForAnalysisChecked));
                    OnPropertyChanged(nameof(IsAnyModelWithoutIfThenAndColumnForAnalysisChecked));
                    OnPropertyChanged(nameof(AreDetailsChecked));
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResult));
                    OnPropertyChanged(nameof(IsAnyModelWithoutIfThenChecked));
                    OnPropertyChanged(nameof(CanViewUserSample));
                    UpdateSelectedModels();

                }
                if (!_isNeuralNetworkChecked)
                {
                    SelectedLayers = null;
                    SelectedResultColumn = null;
                    SelectedTrainingSetPercentage = null;
                    NeuronCounts.Clear();
                    _userSample.UserSamples.Clear();
                }
            }
        }

        public string SelectedLayers
        {
            get => _selectedLayers;
            set
            {
                _selectedLayers = value;
                OnPropertyChanged(nameof(SelectedLayers));
                OnPropertyChanged(nameof(AreDetailsChecked));
                OnPropertyChanged(nameof(CanGenerateResult));
                OnPropertyChanged(nameof(CanViewUserSample));
                UpdateNeuronCounts();
            }
        }


        public bool IsLogisticRegressionChecked
        {
            get => _isLogisticRegressionChecked;
            set
            {
                if (_isLogisticRegressionChecked != value)
                {
                    _isLogisticRegressionChecked = value;
                    OnPropertyChanged(nameof(IsLogisticRegressionChecked));
                    OnPropertyChanged(nameof(IsAnyModelAndColumnForAnalysisChecked));
                    OnPropertyChanged(nameof(IsAnyModelWithoutIfThenAndColumnForAnalysisChecked));
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResult));
                    OnPropertyChanged(nameof(IsAnyModelWithoutIfThenChecked));
                    OnPropertyChanged(nameof(CanViewUserSample));
                    UpdateSelectedModels();

                }
                if (!_isLogisticRegressionChecked)
                {
                    SelectedResultColumn = null;
                    SelectedTrainingSetPercentage = null;
                    _userSample.UserSamples.Clear();
                }
            }
        }

        public bool IsBayesChecked
        {
            get => _isBayesChecked;
            set
            {
                if (_isBayesChecked != value)
                {
                    _isBayesChecked = value;
                    OnPropertyChanged(nameof(IsBayesChecked));
                    OnPropertyChanged(nameof(IsAnyModelAndColumnForAnalysisChecked));
                    OnPropertyChanged(nameof(IsAnyModelWithoutIfThenAndColumnForAnalysisChecked));
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResult));
                    OnPropertyChanged(nameof(IsAnyModelWithoutIfThenChecked));
                    OnPropertyChanged(nameof(CanViewUserSample));
                    UpdateSelectedModels();

                }
                if (!_isBayesChecked)
                {
                    SelectedResultColumn = null;
                    SelectedTrainingSetPercentage = null;
                    _userSample.UserSamples.Clear();
                }
            }
        }

        public bool IsIfThenChecked
        {
            get => _isIfThenChecked;
            set
            {
                if (_isIfThenChecked != value)
                {
                    _isIfThenChecked = value;
                    OnPropertyChanged(nameof(IsIfThenChecked));
                    OnPropertyChanged(nameof(IsAnyModelAndColumnForAnalysisChecked));
                    OnPropertyChanged(nameof(IsIfThenAndResultColumnChecked));
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResult));
                    OnPropertyChanged(nameof(CanViewUserSample));
                    UpdateSelectedModels();

                }
                if (!_isIfThenChecked)
                {
                    SelectedResultColumn = null;
                    _userSample.UserSamples.Clear();
                }
            }
        }

        public string SelectedDistanceMetrics
        {
            get => _selectedDistanceMetrics;
            set
            {
                _selectedDistanceMetrics = value;
                OnPropertyChanged(nameof(SelectedDistanceMetrics));
                OnPropertyChanged(nameof(AreDetailsChecked));
                OnPropertyChanged(nameof(CanGenerateResult));
                OnPropertyChanged(nameof(CanViewUserSample));

            }
        }

        public Dataset SelectedDataset
        {
            get => _selectedDataset;
            set
            {
                _selectedDataset = value;
                OnPropertyChanged(nameof(SelectedDataset)); 
                LoadDatasetColumnNamesCommand.Execute(_selectedDataset);
                OnPropertyChanged(nameof(IsLoading));
                _userSample.UserSamples.Clear();
                SelectedColumnsForAnalysis.Clear();
            }
        }

        public string SelectedResultColumn
        {
            get => _selectedResultColumn;
            set
            {
                _selectedResultColumn = value;
                OnPropertyChanged(nameof(SelectedResultColumn));
                OnPropertyChanged(nameof(AreDetailsChecked));
                OnPropertyChanged(nameof(CanGenerateResult));
                OnPropertyChanged(nameof(IsIfThenAndResultColumnChecked));
                OnPropertyChanged(nameof(UniqueNamesFromClassifyingColumn));
                OnPropertyChanged(nameof(CanViewUserSample));
                IfThenConditions.Clear();
            }
        }

        public string SelectedTrainingSetPercentage
        {
            get => _selectedTrainingSetPercentage;
            set
            {
                _selectedTrainingSetPercentage = value;
                OnPropertyChanged(nameof(SelectedTrainingSetPercentage));
                OnPropertyChanged(nameof(AreDetailsChecked));
                OnPropertyChanged(nameof(CanGenerateResult));
                OnPropertyChanged(nameof(CanViewUserSample));
            }
        }


        public ObservableCollection<string> DatasetColumnNames
        {
            get => _datasetColumnNames;
            set
            {
                _datasetColumnNames = value;
                OnPropertyChanged(nameof(DatasetColumnNames));
            }
        }

        public ObservableCollection<string> DatasetNumericColumnNames
        {
            get => _datasetNumericColumnNames;
            set
            {
                _datasetNumericColumnNames = value;
                OnPropertyChanged(nameof(DatasetNumericColumnNames));
            }
        }

        public ObservableCollection<string> SelectedModels
        {
            get => _selectedModels;
            set
            {
                _selectedModels = value;
                OnPropertyChanged(nameof(SelectedModels));
            }
        }

        public ObservableCollection<string> SelectedColumnsForAnalysis
        {
            get => _selectedColumnsForAnalysis;
            set
            {
                _selectedColumnsForAnalysis = value;
                OnPropertyChanged(nameof(SelectedColumnsForAnalysis));
                OnPropertyChanged(nameof(IsAnyModelAndColumnForAnalysisChecked));
                OnPropertyChanged(nameof(IsAnyModelWithoutIfThenAndColumnForAnalysisChecked));
                OnPropertyChanged(nameof(IsIfThenAndResultColumnChecked));
                OnPropertyChanged(nameof(CanViewUserSample));
            }
        }

        public ObservableCollection<string> UniqueNamesFromClassifyingColumn
        {
            get => _uniqueNamesFromClassifyingColumn;
            set
            {
                _uniqueNamesFromClassifyingColumn = value;
                OnPropertyChanged(nameof(UniqueNamesFromClassifyingColumn));
            }
        }

        public bool IsIfThenAndResultColumnChecked => IsIfThenChecked && !string.IsNullOrWhiteSpace(SelectedResultColumn) && SelectedColumnsForAnalysis.Any();
        public bool IsModelWithParametersChecked => IsKnnChecked || IsNeuralNetworkChecked;
        public bool IsAnyModelChecked => IsKnnChecked || IsBayesChecked || IsLogisticRegressionChecked || IsNeuralNetworkChecked || IsIfThenChecked;
        public bool IsAnyModelWithoutIfThenChecked => IsKnnChecked || IsBayesChecked || IsLogisticRegressionChecked || IsNeuralNetworkChecked;
        public bool IsAnyModelAndColumnForAnalysisChecked => (IsKnnChecked || IsNeuralNetworkChecked || IsLogisticRegressionChecked || IsBayesChecked || IsIfThenChecked) && SelectedColumnsForAnalysis.Any();
        public bool IsAnyModelWithoutIfThenAndColumnForAnalysisChecked => (IsKnnChecked || IsNeuralNetworkChecked || IsLogisticRegressionChecked || IsBayesChecked) && SelectedColumnsForAnalysis.Any();
        public bool AreDetailsChecked =>
            (
            !string.IsNullOrWhiteSpace(SelectedResultColumn) &&
            !string.IsNullOrWhiteSpace(SelectedTrainingSetPercentage) &&
            SelectedTrainingSetPercentage.Length == 2 &&
            (
                !IsKnnChecked ||
                (
                    !string.IsNullOrWhiteSpace(SelectedNeighbours) &&
                    !string.IsNullOrWhiteSpace(SelectedDistanceMetrics)
                )
            ) &&
            (
                !IsNeuralNetworkChecked ||
                (
                    !string.IsNullOrWhiteSpace(SelectedLayers) &&
                    NeuronCounts.All(n => !string.IsNullOrWhiteSpace(n.NeuronCount))
                )
            )
            );
        public bool CanViewUserSample => (!IsAnyModelWithoutIfThenChecked && IsIfThenAndResultColumnChecked) ||
                                         (IsAnyModelWithoutIfThenChecked && AreDetailsChecked);
        public bool CanGenerateResult => (IsAnyModelWithoutIfThenChecked && IsIfThenChecked && AreDetailsChecked && CanGenerateResultUserSamples && CanGenerateResultIfThen) || 
                                         (!IsAnyModelWithoutIfThenChecked && IsIfThenChecked && !string.IsNullOrWhiteSpace(SelectedResultColumn) && CanGenerateResultIfThen && CanGenerateResultUserSamples) || 
                                         (IsAnyModelWithoutIfThenChecked && !IsIfThenChecked && AreDetailsChecked && CanGenerateResultUserSamples);
        public bool CanGenerateResultUserSamples => _userSample.UserSamples.Count == 0 || 
                                                    _userSample.UserSamples.All(sample => sample.All(entry => !string.IsNullOrWhiteSpace(entry.Value) && 
                                                    !entry.Value.EndsWith("-") && 
                                                    !entry.Value.EndsWith(",") &&
                                                    !entry.Value.StartsWith(",") &&
                                                    !entry.Value.StartsWith("-,") &&
                                                    !(entry.Value.StartsWith("0") && entry.Value.ElementAtOrDefault(1) != ',')));
        public bool CanGenerateResultIfThen => IfThenConditions.Count() != 0 &&
                    IfThenConditions.All(group => group.Conditions.Any((condition => condition.SelectedType == "then"))) &&
                    IfThenConditions.All(group => group.Conditions.All(condition => (condition.SelectedType == "If" &&
                    !string.IsNullOrWhiteSpace(condition.SelectedColumn) &&
                    !string.IsNullOrWhiteSpace(condition.SelectedOperator) &&
                    !string.IsNullOrWhiteSpace(condition.SelectedValue) &&
                    !condition.SelectedValue.EndsWith(",") &&
                    !condition.SelectedValue.StartsWith(",") &&
                    !condition.SelectedValue.StartsWith("-,") &&
                    !condition.SelectedValue.EndsWith("-") &&
                    !(condition.SelectedValue.StartsWith("0") && condition.SelectedValue.ElementAtOrDefault(1) != ',') ||
                    (condition.SelectedType == "then" &&
                    !string.IsNullOrWhiteSpace(condition.SelectedClass) ||
                    (condition.SelectedType == "and" &&
                    !string.IsNullOrWhiteSpace(condition.SelectedColumn) &&
                    !string.IsNullOrWhiteSpace(condition.SelectedOperator) &&
                    !string.IsNullOrWhiteSpace(condition.SelectedValue) &&
                    !condition.SelectedValue.EndsWith(",") &&
                    !condition.SelectedValue.StartsWith(",") &&
                    !condition.SelectedValue.StartsWith("-,") &&
                    !condition.SelectedValue.EndsWith("-") &&
                    !(condition.SelectedValue.StartsWith("0") && condition.SelectedValue.ElementAtOrDefault(1) != ','))))));

        private void UpdateNeuronCounts()
        {
            if (!int.TryParse(SelectedLayers, out int layerCount))
            {
                return;
            }
            while (NeuronCounts.Count < layerCount)
            {
                NeuronCounts.Add(new NeuronLayer(this) { NeuronCount = null });
            }
            while (NeuronCounts.Count > layerCount)
            {
                NeuronCounts.RemoveAt(NeuronCounts.Count - 1);
            }
        }

        private void UpdateSelectedModels()
        {
            SelectedModels.Clear();

            if (IsKnnChecked)
            {
                SelectedModels.Add("KNN");
            }
            if (IsLogisticRegressionChecked)
            {
                SelectedModels.Add("Logistic Regression");
            }
            if (IsBayesChecked)
            {
                SelectedModels.Add("Bayes");
            }
            if (IsNeuralNetworkChecked)
            {
                SelectedModels.Add("Neural Network");
            }
            if (IsIfThenChecked)
            {
                SelectedModels.Add("If=>Then");
            }
        }

        public void RaiseAreDetailsChangedAndCanGenerateResult()
        {
            OnPropertyChanged(nameof(AreDetailsChecked));
            OnPropertyChanged(nameof(CanGenerateResult));
        }

        private void UpdateColumnsForAnalysis()
        {
            OnPropertyChanged(nameof(IsAnyModelAndColumnForAnalysisChecked));
            OnPropertyChanged(nameof(IsAnyModelWithoutIfThenAndColumnForAnalysisChecked));
            OnPropertyChanged(nameof(IsIfThenAndResultColumnChecked));
            this.UserSample.UserSamples.Clear();
        }

        public MessageViewModel ErrorMessageViewModel { get; }
        public string ErrorMessage
        {
            set => ErrorMessageViewModel.Message = value;
        }

    }
}
