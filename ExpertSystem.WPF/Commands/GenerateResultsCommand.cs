using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    internal class GenerateResultsCommand : ICommand
    {
        private AnalysisViewModel _viewModel;
        private IDialogService _dialogService;

        public GenerateResultsCommand(AnalysisViewModel viewModel, IDialogService dialogService)
        {
            _viewModel = viewModel;
            _dialogService = dialogService;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _dialogService.ShowResultsDialog();
        }
    }
}
