using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class RemoveIfConditionCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly AnalysisViewModel _analysisViewModel;

        public RemoveIfConditionCommand(AnalysisViewModel analysisViewModel)
        {
            _analysisViewModel = analysisViewModel;
            _analysisViewModel.IfThenConditions.CollectionChanged += (s, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object? parameter)
        {
            return _analysisViewModel.IfThenConditions.Count > 0;
        }

        public void Execute(object? parameter)
        {
            _analysisViewModel.IfThenConditions.RemoveAt(_analysisViewModel.IfThenConditions.Count - 1);
        }
    }

}

