using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Metrics;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{

    public enum Operator
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public enum LogicOperator
    {
        And,
        Or
    }
    public class DecisionRule : BaseObject
    {
        public int ExperimentID { get; set; }
        public string Column { get; set; }
        public Operator Operator { get; set; }
        public double Threshold { get; set; }
        public LogicOperator LogicOperator { get; set; }

        public Experiment Experiment { get; set; }
        public string Result { get; set; }
    }
}
