using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework.Services;


public class ExperimentService : IExperimentService
{
    private readonly GenericDataService<Experiment> _experimentService;
    private readonly GenericDataService<ModelConfiguration> _configService;
    private readonly GenericDataService<ModelResult> _resultService;
    private readonly GenericDataService<DecisionRule> _decisionRuleService;


    public ExperimentService(
        GenericDataService<Experiment> experimentService,
        GenericDataService<ModelConfiguration> configService,
        GenericDataService<ModelResult> resultService,
        GenericDataService<DecisionRule> decisionRuleService)
    {
        _experimentService = experimentService;
        _configService = configService;
        _resultService = resultService;
        _decisionRuleService = decisionRuleService;
    }

    public async Task<int> CreateExperimentWithResults(
        int userId,
        int datasetId,
        List<string> analysisColumns,
        string targetColumn,
        int trainingSize,
        List<ModelAnalysisResult> analysisResults,
        Dictionary<string, string> hyperparameters,
        Dictionary<string, List<Dictionary<string, string>>> samples,
        List<DecisionRule> decisionRules)

    {

        var experiment = new Experiment
        {
            UserId = userId,
            DatasetID = datasetId
        };
        var createdExperiment = await _experimentService.Create(experiment);

        foreach (var result in analysisResults)
        {
            var modelType = result.ModelName switch
            {
                "KNN" => ModelType.KNN,
                "LogisticRegression" => ModelType.LogisticRegression,
                "Bayes" => ModelType.Bayes,
                "NeuralNetwork" => ModelType.NeuralNetwork,
                "Own" => ModelType.Own,
                _ => throw new ArgumentException($"Unknown model name: {result.ModelName}")
            };

            var config = new ModelConfiguration
            {
                ExperimentId = createdExperiment.Id,
                ModelType = modelType,
                AnalysisColumns = analysisColumns,
                TargetColumn = targetColumn,
                TrainingSize = trainingSize,
                Hyperparameters = hyperparameters.ContainsKey(result.ModelName)
                ? (hyperparameters[result.ModelName] ?? "{}")
                : "{}",
                Samples = samples.ContainsKey(result.ModelName)
                ? JsonSerializer.Serialize(samples[result.ModelName])  
                : "{}"

            };
            var createdConfig = await _configService.Create(config);

            var modelResult = new ModelResult
            {
                ConfigId = createdConfig.Id,
                Accuracy = (int)(result.Accuracy * 100),
                F1Score = (int)(result.F1 * 100),
                Precision = (int)(result.Precision * 100),
                Recall = (int)(result.Recall * 100),
                CreatedAt = DateTime.UtcNow,

                ConfusionMatrix = result.ConfusionMatrix != null
                    ? JsonSerializer.Serialize(result.ConfusionMatrix) 
                    : "{}",

                SamplesHistory = result.SamplesHistory != null
                     ? JsonSerializer.Serialize(result.SamplesHistory)
                     : "{}"
            };

            await _resultService.Create(modelResult);

            if (modelType == ModelType.Own)
            {
                foreach (var rule in decisionRules)
                {
                    rule.ConfigId = createdConfig.Id;
                    await _decisionRuleService.Create(rule);
                }
            }
        }


        return createdExperiment.Id;
    }
}