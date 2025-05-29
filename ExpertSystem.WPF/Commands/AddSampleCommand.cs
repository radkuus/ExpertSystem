using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ExpertSystem.WPF.Helpers;
using ExpertSystem.WPF.Helpers.Sample;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class AddSampleCommand : ICommand
    {

        private readonly AnalysisViewModel _analysisViewModel;

        public AddSampleCommand(AnalysisViewModel analysisViewModel)
        {
            _analysisViewModel = analysisViewModel;
            _analysisViewModel.UserSample.UserSamples.CollectionChanged += (s, e) => RaiseCanExecuteChanged();
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object? parameter)
        {
            var samples = _analysisViewModel.UserSample.UserSamples;

            if (samples == null || samples.Count == 0)
            {
                return true;
            }

            var lastSample = samples.LastOrDefault();

            if (lastSample == null)
            {
                return false;
            }

            return lastSample.All(entry => !string.IsNullOrWhiteSpace(entry.Value));
        }

        public void Execute(object? parameter)
        {
            var columns = _analysisViewModel.DatasetColumnNames.Take(_analysisViewModel.DatasetColumnNames.Count - 1).ToList();
            if (columns == null || columns.Count == 0)
            {
                MessageBox.Show("Brak kolumn do utworzenia próbki.");
                return;
            }

            _analysisViewModel.UserSample.AddNewSample(columns, RaiseCanExecuteChanged);
        }
    }
}

