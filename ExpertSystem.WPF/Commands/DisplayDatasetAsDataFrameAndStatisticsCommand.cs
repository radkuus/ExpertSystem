using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.ViewModels;
using ExpertSystem.WPF.Views;

namespace ExpertSystem.WPF.Commands
{
    public class DisplayDatasetAsDataFrameAndStatisticsCommand : ICommand
    {
        private readonly IDatasetService _datasetService;
        private readonly IDialogService _dialogService;
        private readonly IDatasetStatisticsService _datasetStatisticsService;
        private readonly AnalysisViewModel _analysisViewModel;

        public DisplayDatasetAsDataFrameAndStatisticsCommand(IDatasetService datasetService, IDialogService dialogService, IDatasetStatisticsService datasetStatisticsService, AnalysisViewModel analysisViewModel)
        {
            _datasetService = datasetService;
            _dialogService = dialogService;
            _datasetStatisticsService = datasetStatisticsService;
            _analysisViewModel = analysisViewModel;
        }

        public event EventHandler? CanExecuteChanged;


        public bool CanExecute(object? parameter)
        {
            return parameter is Dataset;
        }

        public async void Execute(object? parameter)
        {
            try
            {
                if (parameter is Dataset dataset)
                {
                    _analysisViewModel.IsLoading = true;
                    var dataTable = await _datasetService.GetDatasetAsDataTable(dataset.Id);
                    if (dataTable != null)
                    {
                        var statisticsTable = await _datasetStatisticsService.CalculateDatasetStatistics(dataTable);
                        await Task.Delay(2000);
                        _dialogService.ShowDataFrameDialog(dataTable);
                        _dialogService.ShowDatasetStatistics(statisticsTable);
                    }
                }
            }
            finally
            {
                _analysisViewModel.IsLoading = false;
            }
        }
    }
}
