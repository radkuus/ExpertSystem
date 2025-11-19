using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ExpertSystem.WPF.Commands
{
    public class AdminAddDatasetCommnand : ICommand
    {
        private readonly AdminDatasetViewModel _adminDatasetViewModel;
        private readonly IFileDialogService _fileDialogService;
        private readonly IDatasetService _datasetService;
        private readonly IUserService _userService;

        public AdminAddDatasetCommnand(AdminDatasetViewModel adminDatasetViewModel, IFileDialogService fileDialogService, IDatasetService datasetService, IUserService userService)
        {
            _adminDatasetViewModel = adminDatasetViewModel;
            _fileDialogService = fileDialogService;
            _datasetService = datasetService;
            _userService = userService;

            _adminDatasetViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_adminDatasetViewModel.DatasetOwnerName))
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(_adminDatasetViewModel.DatasetOwnerName);
        }

        public async void Execute(object? parameter)
        {
            try
            {
                    string filePath = _fileDialogService.OpenFileDialog(
                    "CSV files (*.csv)|*.csv|All files (*.*)|*.*");

                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                _adminDatasetViewModel.SelectedFilePath = filePath;
                string fileName = Path.GetFileName(filePath);

                var owner = await _userService.GetByNickname(_adminDatasetViewModel.DatasetOwnerName);
                if (owner == null || owner.Id <= 0)
                {
                    throw new UserNotFoundException(_adminDatasetViewModel.DatasetOwnerName);
                }

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
                string datasetsFolderDirectory = Path.Combine(projectDirectory, "Datasets", _adminDatasetViewModel.DatasetOwnerName);

                if (!Directory.Exists(datasetsFolderDirectory))
                {
                    Directory.CreateDirectory(datasetsFolderDirectory);
                }

                string destPath = Path.Combine(datasetsFolderDirectory, fileName);

            
                File.Copy(filePath, destPath, overwrite: true);

                var dataset = new Dataset
                {
                    Name = fileName,
                    UserId = owner.Id
                };

                await _datasetService.AddDataset(dataset);
                _adminDatasetViewModel.DisplayAllDatasetsCommand.Execute(null);
                _adminDatasetViewModel.ErrorMessage = null;
            }
            catch (UserNotFoundException)
            {
                _adminDatasetViewModel.ErrorMessage = "User not found";
            }
            catch (DatasetAlreadyExistsException)
            {
                _adminDatasetViewModel.ErrorMessage = "Dataset with this name already exists for this user";
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException;
                MessageBox.Show($"Error: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
