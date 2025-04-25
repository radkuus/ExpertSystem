using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Views;

namespace ExpertSystem.WPF.Commands
{
    public class DisplayDatasetAsDataFrameCommand : ICommand
    {
        private readonly IDatasetService _datasetService;
        private readonly IDialogService _dataFrameDialogService;

        public DisplayDatasetAsDataFrameCommand(IDatasetService datasetService, IDialogService dataFrameDialogService)
        {
            _datasetService = datasetService;
            _dataFrameDialogService = dataFrameDialogService;
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
                var dataTable = await _datasetService.GetDatasetAsDataTable(dataset.Id);
                if (dataTable != null)
                {
                    _dataFrameDialogService.ShowDataFrameDialog(dataTable);
                }
            }
        }
    }
}
