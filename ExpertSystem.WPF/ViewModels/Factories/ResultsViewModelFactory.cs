using ExpertSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels.Factories
{
    public class ResultsViewModelFactory : IResultsViewModelFactory
    {
        private readonly CreateViewModel<ResultsViewModel> _vmFactory;

        public ResultsViewModelFactory(CreateViewModel<ResultsViewModel> vmFactory)
        {
            _vmFactory = vmFactory;
        }

        public ResultsViewModel Create(List<ModelAnalysisResult> results, Dictionary<string, List<Dictionary<string, string>>> samples)
        {
            var vm = _vmFactory();
            vm.LoadResults(results, samples);
            return vm;
        }
    }

}
