using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ExpertSystem.WPF.Helpers.Sample;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Commands
{
    public class RemoveSampleCommand : ICommand
    {
        private readonly AnalysisViewModel _analysisViewModel;

        public RemoveSampleCommand(AnalysisViewModel analysisViewModel)
        {
            _analysisViewModel = analysisViewModel;
            _analysisViewModel.UserSample.UserSamples.CollectionChanged += (s, e) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _analysisViewModel.UserSample.UserSamples.Count > 0;
        }

        public void Execute(object? parameter)
        {
            _analysisViewModel.UserSample.UserSamples.RemoveAt(_analysisViewModel.UserSample.UserSamples.Count - 1);
        }
    }
}
