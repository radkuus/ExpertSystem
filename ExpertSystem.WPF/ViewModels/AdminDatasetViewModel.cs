using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpertSystem.WPF.ViewModels
{
    public class AdminDatasetViewModel : BaseViewModel
    {
        private readonly IDatasetService _datasetService;
        private readonly INavigator _navigator;
        private readonly IAuthenticator _authenticator;
        private readonly IFileDialogService _fileDialogService;
        private readonly CreateViewModel<LoginViewModel> _createLoginViewModel;

        private string _selectedFilePath;

        public ObservableCollection<Dataset> Datasets { get; } = new();

        public ICommand LogoutCommand { get; }
        public ICommand DisplayAllDatasetsCommand { get; }
        public ICommand RemoveDatasetCommand { get; }
        public ICommand AddDatasetCommand { get; }

        public AdminDatasetViewModel(IDatasetService datasetService,
            IAuthenticator authenticator,
            INavigator navigator,
            CreateViewModel<LoginViewModel> createLoginViewModel,
            IFileDialogService fileDialogService)
        {
            ErrorMessageViewModel = new MessageViewModel();

            _datasetService = datasetService;
            _authenticator = authenticator;
            _navigator = navigator;
            _fileDialogService = fileDialogService;

            DisplayAllDatasetsCommand = new DisplayAllDatasetsCommand(this, datasetService);
            LogoutCommand = new LogoutCommand(createLoginViewModel, authenticator, navigator);



            DisplayAllDatasetsCommand.Execute(null);
        }

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }

        private Dataset _selectedDataset;
        public Dataset SelectedDataset
        {
            get { return _selectedDataset; }
            set
            {
                if (_selectedDataset != value)
                {
                    _selectedDataset = value;
                    OnPropertyChanged(nameof(SelectedDataset));
                    if (_selectedDataset != null)
                    {
                        DatasetName = _selectedDataset.Name;
                        DatasetOwnerName = _selectedDataset.User.Nickname;
                    }
                    else
                    {

                        DatasetName = string.Empty;
                        DatasetOwnerName = string.Empty;
                    }
                }
            }
        }

        private string _datasetname;
        public string DatasetName
        {
            get { return _datasetname; }
            set
            {
                _datasetname = value;
                OnPropertyChanged(nameof(DatasetName));
            }
        }

        private string _datasetOwnerName;
        public string DatasetOwnerName
        {
            get { return _datasetOwnerName; }
            set
            {
                _datasetOwnerName = value;
                OnPropertyChanged(nameof(DatasetOwnerName));
            }
        }




        public MessageViewModel ErrorMessageViewModel { get; }
        public string ErrorMessage
        {
            set => ErrorMessageViewModel.Message = value;
        }

    }
}
