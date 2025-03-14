using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Metrics;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{
    public enum Metric
    {
        Accuracy,
        F1Score,
        Precision,
        Recall
    }

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
        public Metric Metric { get; set; }
        public Operator Operator { get; set; }
        public double Threshold { get; set; }
        public LogicOperator LogicOperator { get; set; }

        public Experiment Experiment { get; set; }
    }
}
