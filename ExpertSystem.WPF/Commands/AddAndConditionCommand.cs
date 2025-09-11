using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.WPF.Helpers;
using ExpertSystem.WPF.Helpers.IfThen;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class AddAndConditionCommand : ICommand
    {
        private readonly AnalysisViewModel _analysisViewModel;

        public AddAndConditionCommand(AnalysisViewModel analysisViewModel)
        {
            _analysisViewModel = analysisViewModel;

            _analysisViewModel.IfThenConditions.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (IfThenConditionGroup newGroup in e.NewItems)
                    {
                        SubscribeToConditionChanges(newGroup);
                    }
                }
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            };

            foreach (var group in _analysisViewModel.IfThenConditions)
            {
                SubscribeToConditionChanges(group);
            }
        }

        private void SubscribeToConditionChanges(IfThenConditionGroup group)
        {
            group.Conditions.CollectionChanged += (s, e) =>
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _analysisViewModel.IfThenConditions.Any() && !_analysisViewModel.IfThenConditions.Last().Conditions.Any(c => c.SelectedType == "then");
        }

        public void Execute(object? parameter)
        {
            var lastGroup = _analysisViewModel.IfThenConditions.Last();
            lastGroup.Conditions.Add(new IfThenCondition
            {
                SelectedColumn = null,
                SelectedOperator = null,
                SelectedValue = null,
                SelectedType = "and"
            });
        }
    }
}
