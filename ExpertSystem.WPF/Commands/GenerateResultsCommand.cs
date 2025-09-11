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
using ExpertSystem.WPF.Helpers.Sample;
using System.Printing.IndexedProperties;


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

                var user_samples_raw = _viewModel.UserSample;
                var user_samples = new List<List<string>>();
                var samples = new Dictionary<string, string>();

                if (user_samples_raw == null)
                {
                    user_samples = null;
                }
                else
                {
                    foreach (var row in user_samples_raw.UserSamples)
                    {
                        foreach (var entry in row)
                        {
                            // lista dla API
                            user_samples.Add(new List<string> { entry.ColumnName, entry.Value });
                        }
                    }
                }

                var results = new List<ModelAnalysisResult>();
                var hyperparameters = new Dictionary<string, string>();
                var decisionRules = new List<DecisionRule>();

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



                MessageBox.Show("Generating results"); // tymczasowe

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
                        distance_metric,
                        user_samples
                    };

                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/knn", request);
                    response.ModelName = "KNN";
                    results.Add(response);
                    hyperparameters["KNN"] = JsonSerializer.Serialize(new { neighbors });

                    if (user_samples != null)
                        samples["KNN"] = JsonSerializer.Serialize(user_samples);

                }

                // Bayes
                if (_viewModel.IsBayesChecked)
                {
                    var request = new
                    {
                        data,
                        analysis_columns,
                        target_column,
                        training_size = float.Parse(training_size),
                        user_samples

                    };
                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/bayes", request);
                    response.ModelName = "Bayes";
                    results.Add(response);
                    hyperparameters["Bayes"] = null;
                    if (user_samples != null)
                        samples["Bayes"] = JsonSerializer.Serialize(user_samples);
                }

                // LR model
                if (_viewModel.IsLogisticRegressionChecked)
                {
                    var request = new
                    {
                        data,
                        analysis_columns,
                        target_column,
                        training_size = float.Parse(training_size),
                        user_samples
                    };
                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/lr", request);
                    response.ModelName = "LogisticRegression";
                    results.Add(response);
                    hyperparameters["LogisticRegression"] = null;
                    if (user_samples != null)
                        samples["LogisticRegression"] = JsonSerializer.Serialize(user_samples);
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
                        training_size = float.Parse(training_size),
                        user_samples

                    };
                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/NeuralNetwork", request);
                    response.ModelName = "NeuralNetwork";
                    results.Add(response);
                    hyperparameters["NeuralNetwork"] = JsonSerializer.Serialize(new { layers, neurons });
                    if (user_samples != null)
                        samples["NeuralNetwork"] = JsonSerializer.Serialize(user_samples);
                }

                if (_viewModel.IsIfThenChecked)
                {
                    var ifthen = new List<List<string>>();
                    decisionRules = new List<DecisionRule>(); // reset

                    // dla kazdej reguly
                    foreach (var row in _viewModel.IfThenConditions)
                    {
                        // szukanie pierwszego warunku, który ma "then"- zwraca tylko jak znajdzie (mozna potem zablokowac przycisk
                        // Generate jesli brakuje then)
                        var thenClass = row.Conditions.FirstOrDefault(c => c.SelectedType == "then")?.SelectedClass ?? "";

                        // znalezienie indeksu ostatniego warunku, który nie ma "then"
                        int lastNonThenIndex = -1;
                        for (int i = 0; i < row.Conditions.Count; i++)
                        {
                            if (row.Conditions[i].SelectedType != "then")
                                lastNonThenIndex = i;
                        }

                        for (int i = 0; i < row.Conditions.Count; i++)
                        {
                            var col = row.Conditions[i];

                            // dodanie do ifthen dla API
                            ifthen.Add(new List<string>
                            {
                                col.SelectedType ?? "",
                                col.SelectedColumn ?? "",
                                col.SelectedOperator ?? "",
                                col.SelectedValue?.ToString() ?? "",
                                col.SelectedClass ?? ""
                            });

                            // tworzenie DecisionRule tylko dla warunków bez then
                            if (col.SelectedType != "then")
                            {
                                bool isLastNonThen = (i == lastNonThenIndex);

                                var rule = new DecisionRule
                                {
                                    Column = col.SelectedColumn ?? "",
                                    Operator = col.SelectedOperator switch
                                    {
                                        ">" => Operator.GreaterThan,
                                        "<" => Operator.LessThan,
                                        "=" => Operator.Equal,
                                    },
                                    Threshold = col.SelectedValue != null ? Convert.ToDouble(col.SelectedValue) : 0.0,
                                    LogicOperator = isLastNonThen ? LogicOperator.Or : LogicOperator.And, // OR w bazie jesli ostatni warunek konczy się
                                                                                                          // then, w innym przypadku AND
                                    Result = thenClass
                                };

                                decisionRules.Add(rule);
                            }
                        }
                    }

                    var request = new
                    {
                        data,
                        analysis_columns,
                        target_column,
                        user_samples,
                        ifthen
                    };

                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/ifthen", request);
                    response.ModelName = "Own";
                    results.Add(response);
                    hyperparameters["Own"] = null;

                    if (user_samples != null)
                        samples["Own"] = JsonSerializer.Serialize(user_samples);
                }



                // DB SAVE 
                await _experimentService.CreateExperimentWithResults(
                    userId: dataset.UserId,
                    datasetId: dataset.Id,
                    analysisResults: results,
                    hyperparameters: hyperparameters,
                    samples: samples,
                    decisionRules: decisionRules
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