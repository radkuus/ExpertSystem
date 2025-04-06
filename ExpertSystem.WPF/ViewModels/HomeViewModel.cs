using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class HomeViewModel : BaseViewModel
    {
        private readonly IAuthenticator _authenticator;
        private readonly IDatasetService _datasetService;
        private readonly IFileDialogService _fileDialogService;
        private readonly IExpertSystemViewModelFactory _viewModelAbstractFactory;
        private readonly CreateViewModel<LoginViewModel> _createLoginViewModel;
        private readonly INavigator _navigator;
        private readonly IDatasetStore _datasetStore;
        private string _selectedFilePath;

        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            private set
            {
                _nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }

        public ICommand RemoveDatasetCommand { get; }
        public ICommand DisplayUserDatasetsCommand { get; }
        public ICommand AddDatabaseCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand UpdateCurrentViewModelCommand {  get; }

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }

        public HomeViewModel(IAuthenticator authenticator, IDatasetService datasetService, 
            IFileDialogService fileDialogService, CreateViewModel<LoginViewModel> createLoginViewModel, 
            INavigator navigator, IExpertSystemViewModelFactory viewModelAbstractFactory, IDatasetStore datasetStore)
        {
            _authenticator = authenticator;
            _datasetService = datasetService;
            _fileDialogService = fileDialogService;
            _createLoginViewModel = createLoginViewModel;
            _navigator = navigator;
            _viewModelAbstractFactory = viewModelAbstractFactory;
            _datasetStore = datasetStore;
            
            LogoutCommand = new LogoutCommand(createLoginViewModel, authenticator, navigator);
            AddDatabaseCommand = new AddDatasetCommand(this, fileDialogService, datasetService, authenticator);
            DisplayUserDatasetsCommand = new DisplayUserDatasetsCommand(authenticator, datasetService, datasetStore);
            RemoveDatasetCommand = new RemoveDatasetCommand(this, authenticator, datasetService);
            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelAbstractFactory);

            _authenticator.StateChanged += () => CommandManager.InvalidateRequerySuggested();
            Nickname = _authenticator.CurrentUser.Nickname;

            DisplayUserDatasetsCommand.Execute(null);
        }

        public ObservableCollection<Dataset> UserDatasets => _datasetStore.UserDatasets;
    }
}
