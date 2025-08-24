using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool _isKnnChecked;
        private bool _isLogisticRegressionChecked;
        private bool _isBayesChecked;
        private bool _isNeuralNetworkChecked;
        private bool _isIfThenChecked;
        private string _selectedNeighbours;
        private string _selectedLayers;
        private string _selectedTrainingSetPercentage;
        private string _selectedDistanceMetrics;
        private Dataset _selectedDataset;
        private ObservableCollection<string> _selectedColumnsForAnalysis = new ObservableCollection<string>();
        private string _selectedResultColumn;
        private ObservableCollection<string> _datasetColumnNames;
        private ObservableCollection<string> _datasetNumericColumnNames;
        private ObservableCollection<string> _datasetTextColumnNames;
        private ObservableCollection<string> _selectedModels = new ObservableCollection<string>();
        private ObservableCollection<string> _uniqueNamesFromClassifyingColumn = new ObservableCollection<string>();
        public ObservableCollection<string> Operators => IfThenOperators.Operators;
        public ObservableCollection<Dataset> UserDatasets => _datasetStore.UserDatasets;
        public ObservableCollection<IfThenConditionGroup> IfThenConditions { get; set; } = new ObservableCollection<IfThenConditionGroup>();
        public ObservableCollection<NeuronLayer> NeuronCounts { get; set; } = new ObservableCollection<NeuronLayer>();

        public ICommand UpdateCurrentViewModelCommand { get; }
        public ICommand DisplayDatasetAsDataFrameCommand { get; }
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

            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelAbstractFactory);
            DisplayDatasetAsDataFrameCommand = new DisplayDatasetAsDataFrameCommand(datasetService, dataFrameDialogService, dataStatisticsService);
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
                    OnPropertyChanged(nameof(IsIfThenEnabled));
                    OnPropertyChanged(nameof(AreOtherModelsEnabled));
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
                    OnPropertyChanged(nameof(IsIfThenEnabled));
                    OnPropertyChanged(nameof(AreOtherModelsEnabled));
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
                    OnPropertyChanged(nameof(IsIfThenEnabled));
                    OnPropertyChanged(nameof(AreOtherModelsEnabled));
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
                    OnPropertyChanged(nameof(IsIfThenEnabled));
                    OnPropertyChanged(nameof(AreOtherModelsEnabled));
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
                    OnPropertyChanged(nameof(IsIfThenEnabled));
                    OnPropertyChanged(nameof(AreOtherModelsEnabled));
                    OnPropertyChanged(nameof(IsIfThenAndResultColumnChecked));
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
                OnPropertyChanged(nameof(IsIfThenAndResultColumnChecked));
                OnPropertyChanged(nameof(UniqueNamesFromClassifyingColumn));
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
        public ObservableCollection<string> DatasetTextColumnNames
        {
            get => _datasetTextColumnNames;
            set
            {
                _datasetTextColumnNames = value;
                OnPropertyChanged(nameof(DatasetTextColumnNames));
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

        public bool IsIfThenEnabled => !IsKnnChecked && !IsLogisticRegressionChecked && !IsBayesChecked && !IsNeuralNetworkChecked;
        public bool AreOtherModelsEnabled => !IsIfThenChecked;
        public bool IsIfThenAndResultColumnChecked => IsIfThenChecked && !string.IsNullOrWhiteSpace(SelectedResultColumn) && SelectedColumnsForAnalysis.Any();
        public bool IsModelWithParametersChecked => IsKnnChecked || IsNeuralNetworkChecked;
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
            )
            ||
            (
                IsIfThenChecked &&
                !string.IsNullOrWhiteSpace(SelectedResultColumn)
            );

        public UserSample UserSample => _userSample;

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

        public void RaiseAreDetailsChanged()
        {
            OnPropertyChanged(nameof(AreDetailsChecked));
        }

        private void UpdateColumnsForAnalysis()
        {
            OnPropertyChanged(nameof(IsAnyModelAndColumnForAnalysisChecked));
            OnPropertyChanged(nameof(IsAnyModelWithoutIfThenAndColumnForAnalysisChecked));
            OnPropertyChanged(nameof(IsIfThenAndResultColumnChecked));
            this.UserSample.UserSamples.Clear();
        }

    }
}
