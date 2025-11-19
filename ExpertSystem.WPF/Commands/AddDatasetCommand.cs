using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            try
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

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
                string datasetsFolderDirectory = Path.Combine(projectDirectory, "Datasets", _authenticator.CurrentUser.Nickname);

                if (!Directory.Exists(datasetsFolderDirectory))
                {
                    Directory.CreateDirectory(datasetsFolderDirectory);
                }

                string destPath = Path.Combine(datasetsFolderDirectory, fileName);

            
                File.Copy(filePath, destPath, overwrite: true);

                var dataset = new Dataset
                {
                    Name = fileName,
                    UserId = currentUser.Id
                };

                await _datasetService.AddDataset(dataset);
                _viewModel.DisplayUserDatasetsCommand.Execute(null);
                _viewModel.ErrorMessage = null;
            }
            catch (DatasetAlreadyExistsException)
            {
                _viewModel.ErrorMessage = "Dataset with this name already exists for this user";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
