﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class LoadDatasetColumnNamesCommand : ICommand
    {
        private readonly IDatasetService _datasetService;
        private readonly AnalysisViewModel _analysisViewModel;

        public LoadDatasetColumnNamesCommand(IDatasetService datasetService, AnalysisViewModel analysisViewModel)
        {
            _datasetService = datasetService;
            _analysisViewModel = analysisViewModel;
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
                var datasetColumnNames = await _datasetService.GetDatasetColumnNames(dataset.Id);
                var datasetNumericColumnNames = await _datasetService.GetDatasetNumericColumnNames(dataset.Id);
                var datasetTextColumnNames = await _datasetService.GetDatasetTextColumnNames(dataset.Id);
                _analysisViewModel.DatasetColumnNames = datasetColumnNames;
                _analysisViewModel.DatasetNumericColumnNames = datasetNumericColumnNames;
                _analysisViewModel.DatasetTextColumnNames = datasetTextColumnNames;
            }
        }
    }
}
