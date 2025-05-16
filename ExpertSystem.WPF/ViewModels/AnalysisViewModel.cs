using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.Conditions;
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
        private bool _isKnnChecked;
        private bool _isLinearRegressionChecked;
        private bool _isBayesChecked;
        private bool _isNeuralNetworkChecked;
        private bool _isOwnChecked;
        private string _selectedNeighbours;
        private string _selectedLayers;
        private string _selectedTrainingSetPercentage;
        private string _selectedDistanceMetric;
        private Dataset _selectedDataset;
        private string _selectedResultColumn;
        private List<string> _datasetColumnNames;
        private readonly MainViewModel _mainViewModel;
        private ObservableCollection<string> _selectedModels = new ObservableCollection<string>();
        public ObservableCollection<Dataset> UserDatasets => _datasetStore.UserDatasets;
        public ObservableCollection<Condition> Conditions { get; set; } = new ObservableCollection<Condition>();

        public ICommand UpdateCurrentViewModelCommand { get; }
        public ICommand DisplayDatasetAsDataFrameCommand { get; }
        public ICommand LoadDatasetColumnNamesCommand { get; }
        public ICommand AddConditionCommand { get; }
        public ICommand RemoveConditionCommand { get; }
        public ICommand GenerateResultsCommand { get; }

        public AnalysisViewModel(INavigator navigator, IExpertSystemViewModelFactory viewModelAbstractFactory, IAuthenticator authenticator, IDatasetService datasetService, IDatasetStore datasetStore,
            IDialogService dataFrameDialogService, IDatasetStatisticsService dataStatisticsService, IDialogService resultsDialog, IApiService apiService, IExperimentService experimentService, CreateViewModel<ResultsViewModel> resultsFactory, MainViewModel mainViewModel
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

            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelAbstractFactory);
            DisplayDatasetAsDataFrameCommand = new DisplayDatasetAsDataFrameCommand(datasetService, dataFrameDialogService, dataStatisticsService);
            LoadDatasetColumnNamesCommand = new LoadDatasetColumnNamesCommand(datasetService, this);
            AddConditionCommand = new AddConditionCommand(this);
            RemoveConditionCommand = new RemoveConditionCommand(this);
            GenerateResultsCommand = new GenerateResultsCommand(this, resultsDialog, datasetService, apiService, experimentService, resultsFactory, navigator, mainViewModel);
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
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResults));
                    OnPropertyChanged(nameof(IsResultColumnAndHyperparametersChecked));
                    UpdateSelectedModels();
                }
                if (!_isKnnChecked)
                {
                    SelectedNeighbours = null;
                    SelectedResultColumn = null;
                    SelectedDistanceMetric = null;
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
                OnPropertyChanged(nameof(CanGenerateResults));
                OnPropertyChanged(nameof(IsResultColumnAndHyperparametersChecked));

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
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResults));
                    OnPropertyChanged(nameof(IsResultColumnAndHyperparametersChecked));
                    UpdateSelectedModels();

                }
                if (!_isNeuralNetworkChecked)
                {
                    SelectedLayers = null;
                    SelectedResultColumn = null;
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
                OnPropertyChanged(nameof(CanGenerateResults));
                OnPropertyChanged(nameof(IsResultColumnAndHyperparametersChecked));

            }
        }


        public bool IsLinearRegressionChecked
        {
            get => _isLinearRegressionChecked;
            set
            {
                if (_isLinearRegressionChecked != value)
                {
                    _isLinearRegressionChecked = value;
                    OnPropertyChanged(nameof(IsLinearRegressionChecked));
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResults));
                    UpdateSelectedModels();

                }
                if (!_isLinearRegressionChecked)
                {
                    SelectedResultColumn = null;
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
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResults));
                    UpdateSelectedModels();

                }
                if (!_isBayesChecked)
                {
                    SelectedResultColumn = null;
                }
            }
        }

        public bool IsOwnChecked
        {
            get => _isOwnChecked;
            set
            {
                if (_isOwnChecked != value)
                {
                    _isOwnChecked = value;
                    OnPropertyChanged(nameof(IsOwnChecked));
                    OnPropertyChanged(nameof(IsAnyModelChecked));
                    OnPropertyChanged(nameof(CanGenerateResults));
                    UpdateSelectedModels();

                }
                if (!_isOwnChecked)
                {
                    SelectedResultColumn = null;
                }
            }
        }

        public string SelectedDistanceMetric
        {
            get => _selectedDistanceMetric;
            set
            {
                _selectedDistanceMetric = value;
                OnPropertyChanged(nameof(SelectedDistanceMetric));
                OnPropertyChanged(nameof(CanGenerateResults));
                OnPropertyChanged(nameof(IsResultColumnAndHyperparametersChecked));

            }
        }

        public Dataset SelectedDataset
        {
            get => _selectedDataset;
            set
            {
                _selectedDataset = value;
                OnPropertyChanged(nameof(SelectedDataset));
                OnPropertyChanged(nameof(CanGenerateResults));
                LoadDatasetColumnNamesCommand.Execute(_selectedDataset);
            }
        }

        public string SelectedResultColumn
        {
            get => _selectedResultColumn;
            set
            {
                _selectedResultColumn = value;
                OnPropertyChanged(nameof(SelectedResultColumn));
                OnPropertyChanged(nameof(CanGenerateResults));
                OnPropertyChanged(nameof(IsResultColumnAndHyperparametersChecked));
            }
        }

        public string SelectedTrainingSetPercentage
        {
            get => _selectedTrainingSetPercentage;
            set
            {
                _selectedTrainingSetPercentage = value;
                OnPropertyChanged(nameof(SelectedTrainingSetPercentage));
                OnPropertyChanged(nameof(CanGenerateResults));
                OnPropertyChanged(nameof(IsResultColumnAndHyperparametersChecked));
            }
        }


        public List<string> DatasetColumnNames
        {
            get => _datasetColumnNames;
            set
            {
                _datasetColumnNames = value;
                OnPropertyChanged(nameof(DatasetColumnNames));
                OnPropertyChanged(nameof(CanGenerateResults));
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

        public bool IsModelWithParametersChecked => IsKnnChecked || IsNeuralNetworkChecked;
        public bool IsAnyModelChecked => IsKnnChecked || IsNeuralNetworkChecked || IsLinearRegressionChecked || IsBayesChecked || IsOwnChecked;
        public bool IsResultColumnAndHyperparametersChecked => !string.IsNullOrWhiteSpace(SelectedResultColumn) && !string.IsNullOrWhiteSpace(SelectedTrainingSetPercentage) && SelectedTrainingSetPercentage.Length == 2 && (!IsKnnChecked || !string.IsNullOrWhiteSpace(SelectedNeighbours) || !string.IsNullOrWhiteSpace(SelectedDistanceMetric)) && (!IsNeuralNetworkChecked || !string.IsNullOrWhiteSpace(SelectedLayers));
        public bool CanGenerateResults => SelectedDataset != null && IsAnyModelChecked && !string.IsNullOrEmpty(SelectedResultColumn) && !string.IsNullOrWhiteSpace(SelectedTrainingSetPercentage) && SelectedTrainingSetPercentage.Length == 2;

        private void UpdateSelectedModels()
        {
            SelectedModels.Clear();

            if (IsKnnChecked)
            {
                SelectedModels.Add("KNN");
            }
            if (IsLinearRegressionChecked)
            {
                SelectedModels.Add("Linear Regression");
            }
            if (IsBayesChecked)
            {
                SelectedModels.Add("Bayes");
            }
            if (IsNeuralNetworkChecked)
            {
                SelectedModels.Add("Neural Network");
            }
            if (IsOwnChecked)
            {
                SelectedModels.Add("Own");
            }
        }




    }
}
