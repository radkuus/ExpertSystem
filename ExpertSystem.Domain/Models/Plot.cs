using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{
    public enum PlotType
    {
        ConfusionMatrix,
        ROC
    }

    public class Plot : BaseObject
    {

        public int ResultId { get; set; }
        public PlotType PlotType { get; set; }
        public string FilePath { get; set; }

        public ModelResult ModelResult { get; set; }
    }
}
