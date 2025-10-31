using ExpertSystem.Domain.Models;
using ExpertSystem.EntityFramework.Services;
using ExpertSystem.WPF.ViewModels.Results;
using Microsoft.CodeAnalysis;
using Microsoft.Diagnostics.Runtime.Utilities;
using Perfolizer;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace ExpertSystem.WPF.ViewModels
{
    public class ExperimentDetailsViewModel : BaseViewModel
    {
        public int ExperimentId { get; set; }
        public int DecisionRuleConfigId {  get; set; }
        public string DatasetName { get; set; }

        private readonly GenericDataService<ModelConfiguration> _configService;
        private readonly GenericDataService<ModelResult> _resultService;
        private readonly GenericDataService<DecisionRule> _rulesService;

        public ObservableCollection<ModelResultViewModel> Models { get; }

        public ExperimentDetailsViewModel(
            GenericDataService<ModelConfiguration> configService,
            GenericDataService<ModelResult> resultService,
            GenericDataService<DecisionRule> rulesService)
        {
            _configService = configService;
            _resultService = resultService;
            Models = new ObservableCollection<ModelResultViewModel>();
            _rulesService = rulesService;
        }



        public async Task LoadResultsAsync()
        {
            Models.Clear();

            var configs = await _configService.GetAll();
            var results = await _resultService.GetAll();
            var rules = await _rulesService.GetAll();

            var relatedConfigs = configs.Where(c => c.ExperimentId == ExperimentId).ToList();  // np. pobieram wszystkie ustawienia dla ExperimentId = 5

            
            // przechodze przez wszystkie modele i tam, gdzie jest "Own" pobieram ConfigId w celu informacji, ktory model ma decisionrules (ifthen)
            foreach (var modelConfig in relatedConfigs){
                if (modelConfig.ModelType.ToString() == "Own") {
                    DecisionRuleConfigId = modelConfig.Id;
                }
            }
            var relatedResults = results                                                       // zwraca wyniki, dla ktorych pokrywa się ConfigId z eksperymentami
                .Where(r => relatedConfigs.Select(c => c.Id).Contains(r.ConfigId))             // np. ExpId = 5 dla ConfigId = 5 i 6, to zwraca results dla tych configów
                .ToList();

            //tu potem zmien ExperimentId a nie ID
            var relatedRules = rules.Where(c => c.ConfigId == DecisionRuleConfigId).ToList(); // zwraca warunki tylko dla tego konfigu 
            foreach (var r in relatedResults)
            {
                var cfg = relatedConfigs.First(c => c.Id == r.ConfigId);
                var modelVm = new ModelResultViewModel
                {
                    Name = cfg.ModelType.ToString(),
                    ClassificationReport = GenerateClassificationReport(r, cfg, relatedRules),
                    ConfusionMatrixText = "Empty for now"
                };

                // deserializacja z JSONa inputów (Samples)
                List<Dictionary<string, string>>? inputList = null;
                if (!string.IsNullOrEmpty(cfg.Samples))
                {
                    try
                    {
                        inputList = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(cfg.Samples);
                    }
                    catch
                    {
                        inputList = new List<Dictionary<string, string>>();  // na wszelki wypadek jak catch to pusta lista
                    }
                }

                // deserializacja z JSONa outputów (SamplesHistory)
                List<string>? outputs = null;
                if (!string.IsNullOrEmpty(r.SamplesHistory))
                {
                    try
                    {
                        outputs = JsonSerializer.Deserialize<List<string>>(r.SamplesHistory);
                    }
                    catch
                    {
                        outputs = new List<string>();
                    }
                }

                if (inputList != null && outputs != null && inputList.Count == outputs.Count)
                {
                    for (int i = 0; i < inputList.Count; i++)
                    {
                        string sampleLabel = $"Sample {i + 1}:";
                        string sampleData = string.Join(", ", inputList[i].Select(kv => $"{kv.Key}={kv.Value}")); // formatowanie, żeby bylo kolumna = wartosc
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
            }
        }


        private string GenerateClassificationReport(ModelResult result, ModelConfiguration config, List<DecisionRule> rules)
        {
            var sb = new StringBuilder();
            if (config.Hyperparameters != null && config.Hyperparameters.ToString() != "{}")
            {
                sb.AppendLine($"For hyperparameters: {config.Hyperparameters}");
                sb.AppendLine("");
            }

            if (config.ModelType.ToString() == "Own")
            {
                sb.AppendLine(DecisionRulesView(rules));
            }
            sb.AppendLine($"F1 Score: {result.F1Score}" + "%");
            sb.AppendLine($"Precision: {result.Precision}" + "%");
            sb.AppendLine($"Recall: {result.Recall}" + "%");
            sb.AppendLine($"Accuracy: {result.Accuracy}" + "%");
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

                    sb.AppendLine($" than {current.Result}");
                }
                else // jesli "OR"
                {
                    var rule = rules[i];
                    sb.AppendLine($"If {rule.Column} {ConvertOperator(rule.Operator)} {rule.Threshold} than {rule.Result}");
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




    }

}


