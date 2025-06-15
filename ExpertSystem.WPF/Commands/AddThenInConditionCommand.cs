using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Helpers.IfThen;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class AddThenInConditionCommand : ICommand
    {
        private readonly IDatasetService _datasetService;
        private readonly AnalysisViewModel _analysisViewModel;

        public AddThenInConditionCommand(IDatasetService datasetService, AnalysisViewModel analysisViewModel)
        {
            _datasetService = datasetService;
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
            return parameter is Dataset && _analysisViewModel.IfThenConditions.Any() 
                                        && !_analysisViewModel.IfThenConditions.Last().Conditions.Any(c => c.Type == "then");
        }

        public async void Execute(object? parameter)
        {
            if (parameter is Dataset dataset)
            {
                var uniqueNames = await _datasetService.GetUniqueNamesFromClassifyingColumn(dataset.Id, _analysisViewModel.SelectedResultColumn);
                _analysisViewModel.UniqueNamesFromClassifyingColumn = uniqueNames;

                var lastGroup = _analysisViewModel.IfThenConditions.Last();
                lastGroup.Conditions.Add(new IfThenCondition
                {
                    Type = "then",
                    SelectedClass = null
                });

                MessageBox.Show(string.Join(Environment.NewLine, uniqueNames), "Unikalne wartości");
            }
                
        }
    }
}
