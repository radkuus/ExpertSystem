using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework.Services;
using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.State.Authenticators;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpertSystem.WPF.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private readonly IAuthenticator _authenticator;
        private readonly GenericDataService<Experiment> _experimentService;
        private readonly GenericDataService<Dataset> _datasetService;
        private readonly GenericDataService<ModelConfiguration> _configService;
        private readonly GenericDataService<ModelResult> _resultService;
        private readonly GenericDataService<DecisionRule> _rulesService;

        public ObservableCollection<ModelHistory> ExperimentsHistory { get; set; }
            = new ObservableCollection<ModelHistory>();

        public ModelHistory SelectedExperiment { get; set; }

        public ICommand ShowDetailsCommand { get; }

        public HistoryViewModel(IAuthenticator authenticator,
                                GenericDataService<Experiment> experimentService,
                                GenericDataService<Dataset> datasetService,
                                GenericDataService<ModelConfiguration> configService,
                                GenericDataService<ModelResult> resultService,
                                IDialogService dialogService,
                                GenericDataService<DecisionRule> rulesService)
        {
            _authenticator = authenticator;
            _experimentService = experimentService;
            _datasetService = datasetService;
            _configService = configService;
            _resultService = resultService;
            _rulesService = rulesService;


            _ = LoadData();

            ShowDetailsCommand = new ShowDetailsCommand(this, dialogService, configService, resultService, rulesService);
            _rulesService = rulesService;
        }

        private async Task LoadData()
        {
            ExperimentsHistory.Clear();
            var UserId = _authenticator.CurrentUser.Id;
            var experiments = await _experimentService.GetAllByUserId(UserId);
            var datasets = await _datasetService.GetAllByUserId(UserId);
            var configs = await _configService.GetAll();
            var results = await _resultService.GetAll();
            foreach (var exp in experiments)
            {
                var dataset = datasets.FirstOrDefault(d => d.Id == exp.DatasetID);

                var relatedConfigs = configs.Where(c => c.ExperimentId == exp.Id).ToList();
                var relatedResults = results.Where(r => relatedConfigs.Select(c => c.Id).Contains(r.ConfigId)).ToList();

                if (!relatedResults.Any())
                    continue;

                var date = relatedResults.Min(r => r.CreatedAt);

                var models = relatedConfigs.Select(c => c.ModelType.ToString()).ToList();

                var analyzedColumns = relatedConfigs.FirstOrDefault()?.AnalysisColumns ?? new List<string>();

                var targetColumn = relatedConfigs.FirstOrDefault()?.TargetColumn ?? "Unknown";
                var trainingSize = relatedConfigs.FirstOrDefault()?.TrainingSize ?? 0;
                bool hasSamples = relatedConfigs.Any(c => !string.IsNullOrEmpty(c.Samples) && c.Samples != "{}" && c.Samples != "[]");

                ExperimentsHistory.Add(new ModelHistory
                {
                    ExperimentId = exp.Id,
                    Date = date,
                    Dataset = dataset?.Name ?? "Unknown",
                    Models = models,
                    AnalyzedColumns = analyzedColumns,
                    TargetColumn = targetColumn,
                    TrainingSize = trainingSize == 0 ? "-" : trainingSize.ToString(), // "-"dla "Own" 
                    HasSamples = hasSamples
                });
            }
        }


    }

}
