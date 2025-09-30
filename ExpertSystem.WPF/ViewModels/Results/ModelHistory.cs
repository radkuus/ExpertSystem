using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{
    public class ModelHistory
    {
        public int ExperimentId { get; set; }
        public DateTime Date { get; set; }
        public string Dataset { get; set; }
        public List<string> Models { get; set; } = new List<string>();

        public List<string> AnalyzedColumns { get; set; } = new List<string>();

        public string TargetColumn { get; set; }
        public string TrainingSize {get; set; }  // string, żeby móc wyswietlic "-" dla modelu Own 

        public bool HasSamples { get; set; }

        public string ModelsDisplay => Models.Count == 1
            ? Models[0]
            : string.Join(", ", Models);


        // zeby godzina była zgodna ze strefa czasową (wedlug systemu)
        public string DateDisplay
        {
            get
            {
                DateTimeOffset localTime = Date.ToLocalTime();
                return localTime.ToString("dd.MM.yyyy HH:mm:ss");
            }
        }
    }
}
