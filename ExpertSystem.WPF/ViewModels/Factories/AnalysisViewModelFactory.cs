using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels.Factories
{
    public class AnalysisViewModelFactory : IExpertSystemViewModelFactory<AnalysisViewModel>
    {
        public AnalysisViewModel CreateViewModel()
        {
            return new AnalysisViewModel();
        }
    }
}
