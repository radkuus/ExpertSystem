using System;
using System.Collections.Generic;
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

    public ExperimentService(
        GenericDataService<Experiment> experimentService,
        GenericDataService<ModelConfiguration> configService,
        GenericDataService<ModelResult> resultService)
    {
        _experimentService = experimentService;
        _configService = configService;
        _resultService = resultService;
    }

    public async Task<int> CreateExperimentWithResults(
        int userId,
        int datasetId,
        List<ModelAnalysisResult> analysisResults,
        Dictionary<string, string> hyperparameters)
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
                "LinearRegression" => ModelType.LinearRegression,
                "Bayes" => ModelType.Bayes,
                "NeuralNetwork" => ModelType.NeuralNetwork,
                "Own" => ModelType.Own,
                _ => throw new ArgumentException($"Nieznany typ modelu: {result.ModelName}")
            };

            var config = new ModelConfiguration
            {
                ExperimentId = createdExperiment.Id,
                ModelType = modelType,
                Hyperparameters = hyperparameters.ContainsKey(result.ModelName)
                ? (hyperparameters[result.ModelName] ?? "{}")
                : "{}"
            };
            var createdConfig = await _configService.Create(config);

            var modelResult = new ModelResult
            {
                ConfigId = createdConfig.Id,
                SetType = SetType.TestSet,
                Accuracy = (int)(result.Accuracy * 100),
                F1Score = (int)(result.F1 * 100),
                Precision = (int)(result.Precision * 100),
                Recall = (int)(result.Recall * 100),
                CreatedAt = DateTime.UtcNow
            };
            await _resultService.Create(modelResult);
        }

        return createdExperiment.Id;
    }
}