using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ExpertSystem.WPF.Helpers;
using ExpertSystem.WPF.Helpers.IfThen;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class AddIfConditionCommand : ICommand
    {
        private readonly AnalysisViewModel _analysisViewModel;

        public AddIfConditionCommand(AnalysisViewModel analysisViewModel)
        {
            _analysisViewModel = analysisViewModel;
            _analysisViewModel.IfThenConditions.CollectionChanged += (s, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var newGroup = new IfThenConditionGroup();
            newGroup.Conditions.Add(new IfThenCondition
            {
                SelectedColumn = null,
                SelectedOperator = null,
                SelectedValue = null,
                Type = "If"
            });

            _analysisViewModel.IfThenConditions.Add(newGroup);
        }
    }
}
