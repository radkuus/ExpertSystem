using ExpertSystem.Domain.Models;
using ExpertSystem.EntityFramework.Services;
using System.Collections.ObjectModel;
using System.Text;

namespace ExpertSystem.WPF.ViewModels
{
    public class ExperimentDetailsViewModel : BaseViewModel
    {
        public int ExperimentId { get; set; }
        public string DatasetName { get; set; }

        private readonly GenericDataService<ModelConfiguration> _configService;
        private readonly GenericDataService<ModelResult> _resultService;

        public ObservableCollection<ModelResultViewModel> Models { get; } = new();

        public ExperimentDetailsViewModel(
            GenericDataService<ModelConfiguration> configService,
            GenericDataService<ModelResult> resultService)
        {
            _configService = configService;
            _resultService = resultService;
        }

        public async Task LoadResultsAsync()
        {
            Models.Clear();

            var configs = await _configService.GetAll();
            var results = await _resultService.GetAll();

            var relatedConfigs = configs.Where(c => c.ExperimentId == ExperimentId).ToList();
            var relatedResults = results.Where(r => relatedConfigs.Select(c => c.Id).Contains(r.ConfigId)).ToList();

            foreach (var r in relatedResults)
            {
                var cfg = relatedConfigs.First(c => c.Id == r.ConfigId);
                Models.Add(new ModelResultViewModel
                {
                    Name = cfg.ModelType.ToString(),
                    ClassificationReport = GenerateClassificationReport(r),
                    ConfusionMatrixText = "Empty for now"
                });
            }
        }

        private string GenerateClassificationReport(ModelResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"F1 Score: {result.F1Score}" + "%");
            sb.AppendLine($"Precision: {result.Precision}" + "%");
            sb.AppendLine($"Recall: {result.Recall}" + "%");
            sb.AppendLine($"Accuracy: {result.Accuracy}" + "%");
            return sb.ToString();
        }

    }


}
