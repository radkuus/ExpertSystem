using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Models;

namespace ExpertSystem.Domain.Services
{
    public interface IExperimentService
    {
        Task<int> CreateExperimentWithResults(int userId, int datasetId, List<string> analysisColumns, string targetColumn,
        int trainingSize, List<ModelAnalysisResult> analysisResults, Dictionary<string, string> hyperparameters, Dictionary<string, List<Dictionary<string, string>>> samples, List<DecisionRule> decisionRules);
    }
}
