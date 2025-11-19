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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var _datasetOwnerName = _adminDatasetviewModel.DatasetOwnerName;
            var _datasetName = _adminDatasetviewModel.DatasetName;
            var datasetToRemove = _adminDatasetviewModel.Datasets.FirstOrDefault(d => d.User.Nickname == _datasetOwnerName && d.Name == _datasetName);

            try
            {
                await _datasetService.RemoveDatasetByUserAndName(_datasetOwnerName, _datasetName);
                _adminDatasetviewModel.Datasets.Remove(datasetToRemove);
            }
            catch (DatasetNotFoundException)
            {
                _adminDatasetviewModel.ErrorMessage = "Dataset not found";
            }
            


        }

    }
}
