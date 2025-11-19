using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace ExpertSystem.WPF.Commands
{
    public class AdminRemoveDatasetCommand : ICommand
    {
        private readonly IDatasetService _datasetService;
        private readonly AdminDatasetViewModel _adminDatasetviewModel;


        public AdminRemoveDatasetCommand(AdminDatasetViewModel adminDatasetviewModel, IDatasetService datasetService)
        {
            _datasetService = datasetService;
            _adminDatasetviewModel = adminDatasetviewModel;

            _adminDatasetviewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_adminDatasetviewModel.DatasetOwnerName) ||
                e.PropertyName == nameof(_adminDatasetviewModel.DatasetName))
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(_adminDatasetviewModel.DatasetOwnerName)
           && !string.IsNullOrWhiteSpace(_adminDatasetviewModel.DatasetName);
        }

        public async void Execute(object? parameter)
        {
            var datasetOwnerName = _adminDatasetviewModel.DatasetOwnerName;
            var datasetName = _adminDatasetviewModel.DatasetName;
            var datasetToRemove = _adminDatasetviewModel.Datasets.FirstOrDefault(d => d.User.Nickname == datasetOwnerName && d.Name == datasetName);

            try
            {
                await _datasetService.RemoveDatasetByUserAndName(datasetOwnerName, datasetName);
                _adminDatasetviewModel.Datasets.Remove(datasetToRemove);

                string filePath = Path.Combine(
                        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName,
                        "Datasets",
                        datasetOwnerName, datasetName);

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (DatasetNotFoundException)
            {
                _adminDatasetviewModel.ErrorMessage = "Dataset not found";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

    }
}
