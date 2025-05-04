using System.Collections.ObjectModel;
using ExpertSystem.Domain.Models;
using System.Text;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels.Factories;

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

        public void LoadResults(List<ModelAnalysisResult> results)
        {
            Models.Clear();
            foreach (var result in results)
            {
                Models.Add(new ModelResultViewModel
                {
                    Name = result.ModelName ?? $"Model {Models.Count + 1}",
                    ClassificationReport = GenerateClassificationReport(result),
                    ConfusionMatrixText = "Empty for now"
                });
            }
            OnPropertyChanged(nameof(HasResults));
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
    }

    public class ModelResultViewModel
    {
        public string Name { get; set; }
        public string ClassificationReport { get; set; }
        public string ConfusionMatrixText { get; set; }
    }
}