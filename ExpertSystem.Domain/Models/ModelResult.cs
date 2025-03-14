using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{
    public enum SetType
    {
        TrainingSet,
        ValidationSet,
        TestSet,
    }
    public class ModelResult : BaseObject
    {
        public int ConfigId { get; set; }
        public SetType SetType { get; set; }
        public int Accuracy { get; set; }
        public int F1Score { get; set; }
        public int Precision { get; set; }
        public int Recall { get; set; }
        public DateTime CreatedAt { get; set; }


        public ModelConfiguration ModelConfiguration { get; set; }
        public ICollection<Plot> Plots { get; set; }
    }
}
