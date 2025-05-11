using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.WPF.Conditions;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class AddConditionCommand : ICommand
    {

        private readonly AnalysisViewModel _analysisViewModel;

        public AddConditionCommand(AnalysisViewModel analysisViewModel)
        {
            _analysisViewModel = analysisViewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _analysisViewModel.Conditions.Add(new Condition(_analysisViewModel));
        }
    }
}
