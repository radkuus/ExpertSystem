using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.Domain.Models;

namespace ExpertSystem.WPF.Commands
{
    public class AddDatasetCommand : ICommand
    {
        private readonly HomeViewModel _homeViewModel;
        private readonly AdminDatasetViewModel _adminDatasetViewModel;
        private readonly IFileDialogService _fileDialogService;
        private readonly IDatasetService _datasetService;
        private readonly IAuthenticator _authenticator;

        public event EventHandler? CanExecuteChanged;

        public AddDatasetCommand(HomeViewModel homeViewModel, AdminDatasetViewModel adminDatasetViewModel, IFileDialogService fileDialogService, IDatasetService datasetService, IAuthenticator authenticator)
        {
            _homeViewModel = homeViewModel;
            _adminDatasetViewModel = adminDatasetViewModel;
            _fileDialogService = fileDialogService;
            _datasetService = datasetService;
            _authenticator = authenticator;
        }


        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            string filePath = _fileDialogService.OpenFileDialog(
                "CSV files (*.csv)|*.csv|All files (*.*)|*.*");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }


            string fileName;
            var datasetUser = null as Domain.Models.User;
            if (_authenticator.IsUserLoggedIn)
            {
                datasetUser = _authenticator.CurrentUser;
                _homeViewModel.SelectedFilePath = filePath;
                fileName = Path.GetFileName(filePath);
            }
            else
            {
                datasetUser = parameter as Domain.Models.User;
                _adminDatasetViewModel.SelectedFilePath = filePath;
                fileName = Path.GetFileName(filePath);
            }

            if (datasetUser == null || datasetUser.Id <= 0 || datasetUser.IsAdmin)
            {
                return;
            }

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
            string datasetsFolderDirectory = Path.Combine(projectDirectory, "Datasets", _authenticator.CurrentUser.Nickname);

            if (!Directory.Exists(datasetsFolderDirectory))
            {
                Directory.CreateDirectory(datasetsFolderDirectory);
            }

            string destPath = Path.Combine(datasetsFolderDirectory, fileName);

            try
            {
                File.Copy(filePath, destPath, overwrite: true);

                var dataset = new Dataset
                {
                    Name = fileName,
                    UserId = datasetUser.Id
                };

                await _datasetService.AddDataset(dataset);
                if (_homeViewModel!= null)
                {
                    _homeViewModel.DisplayUserDatasetsCommand.Execute(null);
                }
                if (_adminDatasetViewModel != null)
                {
                    _adminDatasetViewModel.DisplayAllDatasetsCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
