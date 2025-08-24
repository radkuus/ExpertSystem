using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.Commands
{
    class DisplayAllDatasetsCommand : BaseAsyncCommand
    {
        private readonly AdminDatasetViewModel _adminDatasetViewModel;
        private readonly IDatasetService _datasetService;

        public DisplayAllDatasetsCommand(AdminDatasetViewModel adminDatasetViewModel, IDatasetService datasetService)
        {
            _adminDatasetViewModel = adminDatasetViewModel;
            _datasetService = datasetService;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            var datasets = await _datasetService.GetAll();
            _adminDatasetViewModel.Datasets.Clear();
            foreach (var dataset in datasets)
            {
                _adminDatasetViewModel.Datasets.Add(dataset);
            }
        }
    }
}
