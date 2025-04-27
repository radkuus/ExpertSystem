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

        public GenerateResultsCommand(
            AnalysisViewModel viewModel,
            IDialogService dialogService,
            IDatasetService datasetService,
            IApiService apiService,
            IExperimentService experimentService)
        {
            _viewModel = viewModel;
            _dialogService = dialogService;
            _datasetService = datasetService;
            _apiService = apiService;
            _experimentService = experimentService;
        }

        public bool CanExecute(object? parameter)
        {
            return _viewModel.CanGenerateResults;
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
                var data = new List<List<string>>();

                foreach (DataRow row in df.Rows)
                {
                    var rowData = new List<string>();
                    foreach (var col in columns)
                    {
                        rowData.Add(row[col].ToString());
                    }
                    data.Add(rowData);
                }

                var results = new List<ModelAnalysisResult>();
                var hyperparameters = new Dictionary<string, string>();

                // KNN
                if (_viewModel.IsKnnChecked)
                {
                    var neighbors = _viewModel.SelectedNeighbours ?? "3";
                    var request = new
                    {
                        data,
                        columns,
                        target_column = _viewModel.SelectedResultColumn,
                        neighbors = int.Parse(neighbors)
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
                        columns,
                        target_column = _viewModel.SelectedResultColumn
                    };
                    var response = await _apiService.PostAsync<ModelAnalysisResult>("/bayes", request);
                    response.ModelName = "Bayes";
                    results.Add(response);
                    hyperparameters["Bayes"] = null;
                }


                await _experimentService.CreateExperimentWithResults(
                    userId: dataset.UserId,
                    datasetId: dataset.Id,
                    analysisResults: results,
                    hyperparameters: hyperparameters
                );

                _dialogService.ShowResultsDialog(results);
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

            // "open" cmd and write -m uvicorn main:app --host 127.0.0.1 --port 8000
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