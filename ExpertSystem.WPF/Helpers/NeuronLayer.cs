using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.WPF.Core;
using ExpertSystem.WPF.ViewModels;

namespace ExpertSystem.WPF.Auxiliary
{
    public class NeuronLayer :ObservableObject
    {
        private readonly AnalysisViewModel _analysisViewModel;
        private string _neuronCount;

        public NeuronLayer(AnalysisViewModel analysisViewModel)
        {
            _analysisViewModel = analysisViewModel;
        }

        public string NeuronCount
        {
            get => _neuronCount;
            set
            {
                _neuronCount = value;
                OnPropertyChanged(nameof(NeuronCount));
                _analysisViewModel?.RaiseAreDetailsChangedCanGenerateResultAndCanViewUserSample();
            }
        }
    }
}
