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
        private readonly IDialogService _resultsDialog;
        private bool _isKnnChecked;
        private bool _isLinearRegressionChecked;
        private bool _isBayesChecked;
        private bool _isNeuralNetworkChecked;
        private bool _isOwnChecked;
        private string _selectedNeighbours;
        private string _selectedLayers;
        private Dataset _selectedDataset;
        private string _selectedResultColumn;
        private List<string> _datasetColumnNames;

        public ICommand UpdateCurrentViewModelCommand { get; }
        public ICommand DisplayDatasetAsDataFrameCommand { get; }
        public ICommand LoadDatasetColumnNames {  get; }
        public ICommand GenerateResultsCommand { get; }
        public AnalysisViewModel(INavigator navigator, IExpertSystemViewModelFactory viewModelAbstractFactory, IAuthenticator authenticator, IDatasetService datasetService, IDatasetStore datasetStore, IDialogService dataFrameDialogService, IDialogService resultsDialog)
        {
            _navigator = navigator;
            _viewModelAbstractFactory = viewModelAbstractFactory;
            _authenticator = authenticator;
            _datasetService = datasetService;
            _datasetStore = datasetStore;
            _dataFrameDialogService = dataFrameDialogService; 
            _resultsDialog = resultsDialog;
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelAbstractFactory);
            DisplayDatasetAsDataFrameCommand = new DisplayDatasetAsDataFrameCommand(datasetService, dataFrameDialogService);
            LoadDatasetColumnNames = new LoadDatasetColumnNames(datasetService, this);
            GenerateResultsCommand = new GenerateResultsCommand(this, resultsDialog); 

        }

        public bool CanGenerateResults => SelectedDataset != null && IsAnyModelChecked && !string.IsNullOrEmpty(SelectedResultColumn);

        public ObservableCollection<Dataset> UserDatasets => _datasetStore.UserDatasets;

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
                }
                if (!_isKnnChecked)
                {
                    SelectedNeighbours = null;
                    SelectedResultColumn = null;
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

                }
                if (!_isOwnChecked)
                {
                    SelectedResultColumn = null;
                }
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
                LoadDatasetColumnNames.Execute(_selectedDataset);
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
            }
        }

        public bool IsModelWithParametersChecked => IsKnnChecked || IsNeuralNetworkChecked;
        public bool IsAnyModelChecked => IsKnnChecked || IsNeuralNetworkChecked || IsLinearRegressionChecked || IsBayesChecked || IsOwnChecked;

        public List<string> DatasetColumnNames
        {
            get => _datasetColumnNames;
            set
            {
                _datasetColumnNames = value;
                OnPropertyChanged(nameof(DatasetColumnNames));
            }
        }



    }
}
