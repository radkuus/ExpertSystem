using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{
    public class ModelHistory
    {
        public DateTime Date { get; set; }
        public string Dataset { get; set; }
        public List<string> Models { get; set; } = new List<string>();

        public bool HasSamples { get; set; }

        public int ExperimentId { get; set; }

    }
}
