using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{

    public class ModelResult : BaseObject
    {
        public int ConfigId { get; set; }
        public double Accuracy { get; set; }
        public double F1Score { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ConfusionMatrix { get; set; }
        public List<string>? SamplesHistory { get; set; }
        public ModelConfiguration ModelConfiguration { get; set; }
    }
}
