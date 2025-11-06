using ExpertSystem.Domain.Models;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels.Results
{
    public class ModelResultViewModel
    {
        public string Name { get; set; }
        public string ClassificationReport { get; set; }

        public ISeries[] BarChartSeries { get; set; }
        public ICartesianAxis[] YAxesBarChartSeries { get; set; }
        public ICartesianAxis[] XAxesBarChartSeries { get; set; }

        public ISeries[] ConfusionMatrixSeries { get; set; }
        public ICartesianAxis[] ConfusionMatrixYAxes { get; set; }
        public ICartesianAxis[] ConfusionMatrixXAxes { get; set; }

        public ObservableCollection<SampleResult> Samples { get; set; }
        = new ObservableCollection<SampleResult>();

        public bool HasSamples => Samples.Count > 0;
    }

}
