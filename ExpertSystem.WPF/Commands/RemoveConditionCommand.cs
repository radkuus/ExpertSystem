using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class RemoveConditionCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly AnalysisViewModel _analysisViewModel;

        public RemoveConditionCommand(AnalysisViewModel analysisViewModel)
        {
            _analysisViewModel = analysisViewModel;
            _analysisViewModel.Conditions.CollectionChanged += (s, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object? parameter)
        {
            return _analysisViewModel.Conditions.Count > 0;
        }

        public void Execute(object? parameter)
        {
            if (_analysisViewModel.Conditions.Count > 0)
            {
                _analysisViewModel.Conditions.RemoveAt(_analysisViewModel.Conditions.Count - 1);
            }
        }
    }

}

