using ExpertSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels.Factories
{
    public interface IResultsViewModelFactory
    {
        ResultsViewModel Create(List<ModelAnalysisResult> results, Dictionary<string, List<Dictionary<string, string>>> samples);

    }
}
