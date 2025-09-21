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
        public string ConfusionMatrixText { get; set; }

        public ObservableCollection<SampleResult> Samples { get; set; }
        = new ObservableCollection<SampleResult>();

        public bool HasSamples => Samples.Count > 0;
    }

}
