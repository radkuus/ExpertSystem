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
        private readonly IDataFrameDialogService _dataFrameDialogService;
        private bool _isKnnChecked;
        private bool _isLinearRegressionChecked;
        private bool _isBayesChecked;
        private bool _isNeuralNetworkChecked;
        private bool _isOwnChecked;
        private string _selectedNeighbours;
        private string _selectedLayers;
        private Dataset _selectedDataset;

        public ICommand UpdateCurrentViewModelCommand { get; }
        public ICommand DisplayDatasetAsDataFrameCommand { get; }

        public AnalysisViewModel(INavigator navigator, IExpertSystemViewModelFactory viewModelAbstractFactory, IAuthenticator authenticator, IDatasetService datasetService, IDatasetStore datasetStore, IDataFrameDialogService dataFrameDialogService)
        {
            _navigator = navigator;
            _viewModelAbstractFactory = viewModelAbstractFactory;
            _authenticator = authenticator;
            _datasetService = datasetService;
            _datasetStore = datasetStore;
            _dataFrameDialogService = dataFrameDialogService; 

            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelAbstractFactory);
            DisplayDatasetAsDataFrameCommand = new DisplayDatasetAsDataFrameCommand(datasetService, dataFrameDialogService);
        }

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
                }
                if(!_isKnnChecked)
                {
                    SelectedNeighbours = null;
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
                }
                if (!_isNeuralNetworkChecked)
                {
                    SelectedLayers = null;
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
            }
        }
        

        public bool IsLinearRegressionChecked
        {
            get => _isLinearRegressionChecked;
            set
            {
                _isLinearRegressionChecked = value;
                OnPropertyChanged(nameof(IsLinearRegressionChecked));
            }
        }

        public bool IsBayesChecked
        {
            get => _isBayesChecked;
            set
            {
                _isBayesChecked = value;
                OnPropertyChanged(nameof(IsBayesChecked));
            }
        }

        public bool IsOwnChecked
        {
            get => _isOwnChecked;
            set
            {
                _isOwnChecked = value;
                OnPropertyChanged(nameof(IsOwnChecked));
            }
        }


        public Dataset SelectedDataset
        {
            get => _selectedDataset;
            set
            {
                _selectedDataset = value;
                OnPropertyChanged(nameof(SelectedDataset));
            }
        }
    }
}
