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
        Equal,
        LessThan,
    }

    public enum LogicOperator
    {
        And,
        Or
    }
    public class DecisionRule : BaseObject
    {
        public int ConfigId { get; set; }
        public string Column { get; set; }
        public Operator Operator { get; set; }
        public double Threshold { get; set; }
        public LogicOperator LogicOperator { get; set; }

        public string Result { get; set; }
        public ModelConfiguration ModelConfiguration { get; set; }
    }
}
