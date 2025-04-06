using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Datasets;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class DisplayUserDatasetsCommand :ICommand
    {
        private readonly IAuthenticator _authenticator;
        private readonly IDatasetService _datasetService;
        private readonly IDatasetStore _datasetStore;

        public DisplayUserDatasetsCommand(IAuthenticator authenticator, IDatasetService datasetService, IDatasetStore datasetStore)
        {
            _authenticator = authenticator;
            _datasetService = datasetService;
            _datasetStore = datasetStore;
        }

        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            var datasets = await _datasetService.GetUserDatasets(_authenticator.CurrentUser.Id);
            _datasetStore.UserDatasets.Clear();
            foreach (var dataset in datasets)
            {
                _datasetStore.UserDatasets.Add(dataset);
            }
        }
    }
}
