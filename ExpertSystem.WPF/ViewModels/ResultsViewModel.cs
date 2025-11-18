using ExpertSystem.Domain.Models;
using ExpertSystem.EntityFramework.Services;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels.Factories;
using ExpertSystem.WPF.ViewModels.Results;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using Microsoft.Diagnostics.Runtime.Utilities;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Media;

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

        public void LoadResults(List<ModelAnalysisResult> results, Dictionary<string, List<Dictionary<string, string>>> samples, Dictionary<string, string> hyperparameters, List<DecisionRule> decisionRules)
        {
            Models.Clear();
            List<string> outputLabels;
            foreach (var result in results)
            {
                outputLabels = result.ClassLabels ?? new List<string>();

                hyperparameters.TryGetValue(result.ModelName, out var hyperparamsForModel);

                var modelVm = new ModelResultViewModel
                {
                    Name = result.ModelName ?? $"Model {Models.Count + 1}",
                    ClassificationReport = GenerateClassificationReport(result, hyperparamsForModel, decisionRules),

                    BarChartSeries = GenerateBarChartSeries(result),
                    YAxesBarChartSeries = new ICartesianAxis[]
                    {
                        new Axis 
                        { 
                            MinLimit = 0, 
                            MaxLimit = 1 ,
                            Labeler = value => (value * 100).ToString("0") + "%",
                            LabelsPaint = new SolidColorPaint(SKColors.White)
                        }
                    },
                    XAxesBarChartSeries = new ICartesianAxis[]
                    {
                        new Axis 
                        { 
                            Labels = ["F1 Score", "Precision", "Recall", "Accuracy"],
                            LabelsPaint = new SolidColorPaint(SKColors.White)
                        }
                    },

                    ConfusionMatrixSeries = GenerateConfusionMatrixSeries(result),
                    ConfusionMatrixXAxes = GenerateConfusionMatrixXAxes(outputLabels),
                    ConfusionMatrixYAxes = GenerateConfusionMatrixYAxes(outputLabels)
                };

                samples.TryGetValue(result.ModelName, out var inputList);

                var outputs = result.SamplesHistory ?? new List<string>();

                if (inputList != null)
                {

                    for (int i = 0; i < inputList.Count; i++)
                    {
                        var inputPairs = inputList[i].Select(kvp => $"{kvp.Key} = {kvp.Value}");
                        string sampleLabel = $"Sample {i + 1}:";
                        string sampleData = string.Join(", ", inputPairs);
                        string output = outputs[i];

                        modelVm.Samples.Add(new SampleResult
                        {
                            SampleLabel = sampleLabel,
                            SampleData = sampleData,
                            Output = output
                        });
                        }

                }

                Models.Add(modelVm);
                OnPropertyChanged(nameof(HasResults));
        }
    }

        private string GenerateClassificationReport(ModelAnalysisResult result, string hyperparameters, List<DecisionRule> decisionRules)
        {
            var sb = new StringBuilder();
            if (hyperparameters != null && hyperparameters != "{}")
            {
                string dictContent = string.Join(", ", hyperparameters);
                sb.AppendLine($"For hyperparameters: {dictContent}");
                sb.AppendLine("");
            }
            if (result.ModelName == "IfThen")
            {
                sb.AppendLine(DecisionRulesView(decisionRules));
            }



            sb.AppendLine($"F1 Score: {result.F1 * 100:0.##}%");
            sb.AppendLine($"Precision: {result.Precision * 100:0.##}%");
            sb.AppendLine($"Recall: {result.Recall * 100:0.##}%");
            sb.AppendLine($"Accuracy: {result.Accuracy * 100:0.##}%");
            return sb.ToString();
        }

        private string DecisionRulesView(List<DecisionRule> rules)
        {
            var sb = new StringBuilder();

            int i = 0;
            while (i < rules.Count)
            {
                var current = rules[i];

                if (current.LogicOperator == LogicOperator.And)
                {
                    sb.Append("If ");

                    while (i < rules.Count)
                    {
                        var rule = rules[i];
                        sb.Append($"{rule.Column} {ConvertOperator(rule.Operator)} {rule.Threshold}");
                        i++;

                        if (i < rules.Count &&
                           (rules[i].LogicOperator == LogicOperator.And || rules[i - 1].LogicOperator == LogicOperator.And)) // jeśli nastepny warunek (kolejny wiersz w bazie) ma AND
                        {
                            sb.Append(" AND ");
                        }
                        else
                        {
                            break;
                        }
                    }

                    sb.AppendLine($" then {current.Result}");
                }
                else // jesli "OR"
                {
                    var rule = rules[i];
                    sb.AppendLine($"If {rule.Column} {ConvertOperator(rule.Operator)} {rule.Threshold} then {rule.Result}");
                    i++;
                }
            }

            return sb.ToString();
        }

        private string ConvertOperator(Operator op)
        {
            return op switch
            {
                Operator.GreaterThan => ">",
                Operator.Equal => "=",
                Operator.LessThan => "<",
            };
        }


        private ISeries[] GenerateBarChartSeries(ModelAnalysisResult result)
        {
            var series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = [result.F1, result.Precision, result.Recall, result.Accuracy],
                    Stroke = null,
                    Fill = new SolidColorPaint(SKColors.RoyalBlue),
                    IgnoresBarPosition = false,
                    DataLabelsSize = 14,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsPosition = DataLabelsPosition.Middle,
                    DataLabelsFormatter = (point) => $"{point.Coordinate.PrimaryValue:P2}"
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
                        SKColors.White.AsLvcColor(),
                        SKColors.Blue.AsLvcColor()
                    ],
                    Values = result.ConfusionMatrix
                    .SelectMany((row, rowIndex) =>
                        row.Select((value, colIndex) =>
                        new WeightedPoint(colIndex, result.ConfusionMatrix.Count - 1 - rowIndex, value)))
                            .ToArray(),
                    DataLabelsSize = 16,
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsPosition = DataLabelsPosition.Middle,
                    DataLabelsFormatter = (point) => ((WeightedPoint)point.Model!).Weight.ToString()
                }
            };
            return series;
        }

        private ICartesianAxis[] GenerateConfusionMatrixXAxes(List<string> classLabels)
        {
            var cartesianAxis = new ICartesianAxis[]
            {
                new Axis
                {
                    Name = "Predicted Labels",
                    Labels = classLabels,
                    NamePaint = new SolidColorPaint(SKColors.White),
                    LabelsPaint = new SolidColorPaint(SKColors.White)
                }
            };
            return cartesianAxis;
        }

        private ICartesianAxis[] GenerateConfusionMatrixYAxes(List<string> classLabels)
        {
            var cartesianAxis = new ICartesianAxis[]
            {
                new Axis
                {
                    Name = "True Labelss",
                    Labels = classLabels.AsEnumerable().Reverse().ToList(),
                    NamePaint = new SolidColorPaint(SKColors.White),
                    LabelsPaint = new SolidColorPaint(SKColors.White) 
                }
            };
            return cartesianAxis;
        }

    }

}