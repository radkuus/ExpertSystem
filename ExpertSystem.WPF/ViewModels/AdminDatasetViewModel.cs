using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Commands;
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

        public ObservableCollection<Dataset> Datasets { get; } = new();

        public ICommand DisplayAllDatasetsCommand { get; }

        public AdminDatasetViewModel(IDatasetService datasetService,
            IAuthenticator authenticator,
            INavigator navigator)
        {
            ErrorMessageViewModel = new MessageViewModel();

            _datasetService = datasetService;
            _authenticator = authenticator;
            _navigator = navigator;

            DisplayAllDatasetsCommand = new DisplayAllDatasetsCommand(this, datasetService);




            DisplayAllDatasetsCommand.Execute(null);
        }

        private User _selectedDataset;
        public User SelectedDataset
        {
            get { return _selectedDataset; }
            set
            {
                if (_selectedDataset != value)
                {
                    _selectedDataset = value;
                    OnPropertyChanged(nameof(SelectedDataset));

                }
            }
        }

        private string _datasetname;
        public string Datasetname
        {
            get { return _datasetname; }
            set
            {
                _datasetname = value;
                OnPropertyChanged(nameof(Datasetname));
            }
        }




        public MessageViewModel ErrorMessageViewModel { get; }
        public string ErrorMessage
        {
            set => ErrorMessageViewModel.Message = value;
        }

    }
}
