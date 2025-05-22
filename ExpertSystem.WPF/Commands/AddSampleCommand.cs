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
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var columns = _analysisViewModel.DatasetColumnNames.Take(_analysisViewModel.DatasetColumnNames.Count - 1).ToList();
            if (columns == null || columns.Count == 0)
            {
                MessageBox.Show("Brak kolumn do utworzenia próbki.");
                return;
            }

            _analysisViewModel.UserSample.AddNewSample(columns);
        }
    }
}

