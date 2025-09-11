using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework.Services;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.ViewModels;
using ExpertSystem.Domain.Models;
using System.Diagnostics;
using System.Net.Http;
using System.IO;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.Helpers.IfThen;

namespace ExpertSystem.WPF.Commands
{
    internal class GenerateResultsCommand : ICommand
    {
        private readonly AnalysisViewModel _viewModel;
        private readonly IDialogService _dialogService;
        private readonly IDatasetService _datasetService;
        private readonly IApiService _apiService;
        private readonly IExperimentService _experimentService;
        private Process? _apiProcess;
        private readonly CreateViewModel<ResultsViewModel> _resultsFactory;
        private readonly INavigator _navigator;
        private readonly MainViewModel _mainViewModel;

        public GenerateResultsCommand(
            AnalysisViewModel viewModel,
            IDialogService dialogService,
            IDatasetService datasetService,
            IApiService apiService,
            IExperimentService experimentService,
            CreateViewModel<ResultsViewModel> resultsFactory,
            INavigator navigator,
            MainViewModel mainViewModel)
        {
            _viewModel = viewModel;
            _dialogService = dialogService;
            _datasetService = datasetService;
            _apiService = apiService;
            _experimentService = experimentService;
            _resultsFactory = resultsFactory;
            _navigator = navigator;
            _mainViewModel = mainViewModel;
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            try
            {
                // check server status
                await EnsureServerRunning();



                var dataset = _viewModel.SelectedDataset;
                var df = await _datasetService.GetDatasetAsDataTable(dataset.Id);
                var columns = _viewModel.DatasetColumnNames;
                var analysis_columns = _viewModel.SelectedColumnsForAnalysis;
                var target_column = _viewModel.SelectedResultColumn;
                var training_size = _viewModel.SelectedTrainingSetPercentage;
                var data = new List<List<string>>();

                List<string> selectedColumns = new List<string>(analysis_columns);
                selectedColumns.Add(target_column);

                foreach (DataRow row in df.Rows)
                {
                    var rowData = new List<string>();
                    foreach (var col in selectedColumns)
                    {
                        rowData.Add(row[col].ToString());
                    }
                    data.Add(rowData);
                }


                var results = new List<ModelAnalysisResult>();
                var hyperparameters = new Dictionary<string, string>();
                MessageBox.Show("Generating results");

                // KNN
                if (_viewModel.IsKnnChecked)
                {
                    var neighbors = _viewModel.SelectedNeighbours ?? "3";
                    var distance_metric = _viewModel.SelectedDistanceMetrics;
                    var request = new
                    {
                        data,
                        analysis_columns,
                        target_column,
                        training_size = float.Parse(training_size),
                        neighbors = int.Parse(neighbors),
                        distance_metric
                    };

                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/knn", request);
                    response.ModelName = "KNN";
                    results.Add(response);
                    hyperparameters["KNN"] = JsonSerializer.Serialize(new { neighbors });
                }

                // Bayes
                if (_viewModel.IsBayesChecked)
                {
                    var request = new
                    {
                        data,
                        analysis_columns,
                        target_column,
                        training_size = float.Parse(training_size)

                    };
                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/bayes", request);
                    response.ModelName = "Bayes";
                    results.Add(response);
                    hyperparameters["Bayes"] = null;
                }

                // NeuralNetwork
                if (_viewModel.IsNeuralNetworkChecked)
                {
                    var neurons = _viewModel.NeuronCounts
                        .Select(n => int.Parse(n.NeuronCount))
                        .ToList();
                    var layers = _viewModel.SelectedLayers;
                    var request = new
                    {
                        data,
                        analysis_columns,
                        neurons,
                        layers,
                        target_column,
                        training_size = float.Parse(training_size)

                    };
                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/NeuralNetwork", request);
                    response.ModelName = "NeuralNetwork";
                    results.Add(response);
                    hyperparameters["NeuralNetwork"] = null;
                }

                if (_viewModel.IsLogisticRegressionChecked)
                {
                    var request = new
                    {
                        data,
                        analysis_columns,
                        target_column,
                        training_size = float.Parse(training_size)
                    };
                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/lr", request);
                    response.ModelName = "LogisticRegression";
                    results.Add(response);
                    hyperparameters["LogisticReggresion"] = null;
                }


                await _experimentService.CreateExperimentWithResults(
                    userId: dataset.UserId,
                    datasetId: dataset.Id,
                    analysisResults: results,
                    hyperparameters: hyperparameters
                );

                var resultsVm = _resultsFactory();
                resultsVm.LoadResults(results);

                _navigator.CurrentViewModel = resultsVm;
                _navigator.CurrentViewType = ViewType.Results;
                _mainViewModel.AreResultsGenerated = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private async Task EnsureServerRunning()
        {
            if (_apiProcess != null && !_apiProcess.HasExited)
                return;

            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var solutionRoot = Path.GetFullPath(Path.Combine(exeFolder, "..", "..", "..", ".."));
            var mainPy = Path.Combine(solutionRoot, "main.py");

            if (!File.Exists(mainPy))
                throw new FileNotFoundException($"Can't find file main.py in {solutionRoot}");

            // "open" cmd and write -m
            // uvicorn main:app --host 127.0.0.1 --port 8000
            var startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "-m uvicorn main:app --host 127.0.0.1 --port 8000",
                WorkingDirectory = solutionRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _apiProcess = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Failed to start FastAPI server.");

            // check if server is is up and responding
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(1) };
            var maxTries = 10;
            for (int i = 0; i < maxTries; i++)
            {
                try
                {
                    var r = await client.GetAsync("http://127.0.0.1:8000/health");
                    if (r.IsSuccessStatusCode)
                        return;
                }
                catch
                {

                }
                await Task.Delay(500);
            }

            throw new TimeoutException("FastAPI didn't respond in time.");
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}