using ExpertSystem.Domain.Models;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels.Factories;
using ExpertSystem.WPF.ViewModels.Results;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Diagnostics.Runtime.Utilities;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace ExpertSystem.WPF.ViewModels
{
    public class ResultsViewModel : BaseViewModel
    {
        private readonly INavigator _navigator;

        public ObservableCollection<ModelResultViewModel> Models { get; }
        public bool HasResults => Models.Count > 0;

        public ResultsViewModel(INavigator navigator,
                                IExpertSystemViewModelFactory factory)
        {
            _navigator = navigator;
            Models = new ObservableCollection<ModelResultViewModel>();
        }

        public void LoadResults(List<ModelAnalysisResult> results, Dictionary<string, List<Dictionary<string, string>>> samples)
        {
            Models.Clear();
            foreach (var result in results)
            {
                var modelVm = new ModelResultViewModel
                {
                    Name = result.ModelName ?? $"Model {Models.Count + 1}",
                    ClassificationReport = GenerateClassificationReport(result),

                    BarChartSeries = GenerateBarChartSeries(result),
                    YAxesBarChartSeries = new ICartesianAxis[]
                    {
                        new Axis { MinLimit = 0, MaxLimit = 1 }
                    },
                    XAxesBarChartSeries = new ICartesianAxis[]
                    {
                        new Axis { Labels = ["F1 Score", "Precision", "Recall", "Accuracy"] }
                    },

                    ConfusionMatrixSeries = GenerateConfusionMatrixSeries(result),
                    //ConfusionMatrixYAxes = GenerateConfusionMatrixYAxes(result),
                    //ConfusionMatrixXAxes = GenerateConfusionMatrixXAxes(result)
                };

                samples.TryGetValue(result.ModelName, out var inputList);

                var outputs = result.SamplesHistory ?? new List<string>();

                if (inputList != null)
                {
                    if (inputList != null)
                    {
                        for (int i = 0; i < inputList.Count; i++)
                        {
                            var inputPairs = inputList[i].Select(kvp => $"{kvp.Key} = {kvp.Value}");
                            string sampleLabel = $"Sample {i + 1}:";
                            string sampleData = string.Join(", ", inputPairs);
                            string output = i < outputs.Count ? outputs[i] : "—";

                            modelVm.Samples.Add(new SampleResult
                            {
                                SampleLabel = sampleLabel,
                                SampleData = sampleData,
                                Output = output
                            });
                        }
                    }

                }

                Models.Add(modelVm);
                OnPropertyChanged(nameof(HasResults));
        }
    }

        private string GenerateClassificationReport(ModelAnalysisResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"F1 Score: {result.F1:F2}");
            sb.AppendLine($"Precision: {result.Precision:F2}");
            sb.AppendLine($"Recall: {result.Recall:F2}");
            sb.AppendLine($"Accuracy: {result.Accuracy:F2}");
            return sb.ToString();
        }

        private ISeries[] GenerateBarChartSeries(ModelAnalysisResult result)
        {
            var series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = [result.F1, result.Precision, result.Recall, result.Accuracy],
                    Stroke = null,
                    Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                    IgnoresBarPosition = true
                }
            };
            return series;
        }

        private ISeries[] GenerateConfusionMatrixSeries(ModelAnalysisResult result)
        {
            var series = new ISeries[]
            {
                new HeatSeries<WeightedPoint>
                {
                    HeatMap = 
                    [
                        new SKColor(29, 185, 84).AsLvcColor(), // the smallest element is the "greenest"
                        SKColors.Red.AsLvcColor() // the largest element is the "warmest"
                    ],
                    Values = result.ConfusionMatrix
                    .SelectMany((row, rowIndex) =>
                        row.Select((value, colIndex) =>
                        new WeightedPoint(rowIndex, colIndex, value)))
                            .ToArray()
                }
            };
            return series;
        }

        //private ICartesianAxis[] GenerateConfusionMatrixYAxes(ModelAnalysisResult result)
        //{

        //}

        //private ICartesianAxis[] GenerateConfusionMatrixXAxes(ModelAnalysisResult result)
        //{

        //}

    }

}