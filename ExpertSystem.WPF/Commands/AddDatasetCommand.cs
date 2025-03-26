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
        private readonly HomeViewModel _viewModel;
        private readonly IFileDialogService _fileDialogService;
        private readonly IDatasetService _datasetService;
        private readonly IAuthenticator _authenticator;

        public event EventHandler? CanExecuteChanged;

        public AddDatasetCommand(HomeViewModel viewModel, IFileDialogService fileDialogService, IDatasetService datasetService, IAuthenticator authenticator)
        {
            _viewModel = viewModel;
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
            if (!_authenticator.IsUserLoggedIn)
            {
                return;
            }

            string filePath = _fileDialogService.OpenFileDialog(
                "CSV files (*.csv)|*.csv|All files (*.*)|*.*");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            _viewModel.SelectedFilePath = filePath;
            string fileName = Path.GetFileName(filePath);

            var currentUser = _authenticator.CurrentUser;
            if (currentUser == null || currentUser.Id <= 0)
            {
                return;
            }

            var dataset = new Dataset
            {
                Name = fileName,
                UserId = currentUser.Id
            };

            try
            {
                await _datasetService.AddDataset(dataset);
                _viewModel.DisplayUserDatasetsCommand.Execute(null);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
