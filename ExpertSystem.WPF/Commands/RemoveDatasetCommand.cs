using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class RemoveDatasetCommand : ICommand
    {
        private readonly IAuthenticator _authenticator;
        private readonly IDatasetService _datasetService;
        private readonly HomeViewModel _homeViewModel;
        private readonly AdminDatasetViewModel _adminDatasetViewModel;

        public RemoveDatasetCommand(HomeViewModel homeViewModel, AdminDatasetViewModel adminDatasetViewModel, IAuthenticator authenticator, IDatasetService datasetService)
        {
            _authenticator = authenticator;
            _datasetService = datasetService;
            _homeViewModel = homeViewModel;
            _adminDatasetViewModel = adminDatasetViewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return parameter is Dataset;
        }

        public async void Execute(object? parameter)
        {
            if (parameter is Dataset dataset)
            {
                
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
                if (_authenticator.IsUserLoggedIn) 
                {
                    string datasetsFolderDirectory = Path.Combine(projectDirectory, "Datasets", _authenticator.CurrentUser.Nickname);
                    string datasetFilePath = Path.Combine(datasetsFolderDirectory, dataset.Name);

                    await _datasetService.RemoveDataset(dataset.Id);
                    _homeViewModel.UserDatasets.Remove(dataset);
                    File.Delete(datasetFilePath);
                }
                else if (_authenticator.IsAdminLoggedIn)
                {
                    string datasetsFolderDirectory = Path.Combine(projectDirectory, "Datasets", _adminDatasetViewModel.SelectedDataset.User.Nickname);
                    string datasetFilePath = Path.Combine(datasetsFolderDirectory, dataset.Name);

                    await _datasetService.RemoveDataset(dataset.Id);
                    _adminDatasetViewModel.Datasets.Remove(dataset);
                    File.Delete(datasetFilePath);
                }

                
            }
        }
    }
}
