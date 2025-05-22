using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Helpers
{
    public class Condition
    {
        public string? SelectedModel { get; set; }
        public string? SelectedMetric { get; set; }
        public string? SelectedOperator { get; set; }
        public string? SelectedValue { get; set; }

        public AnalysisViewModel AnalysisViewModel { get; set; }

        public Condition(AnalysisViewModel analysisViewModel)
        {
            AnalysisViewModel = analysisViewModel;
        }
        public ObservableCollection<string> SelectedModels => AnalysisViewModel.SelectedModels;
    }
}
