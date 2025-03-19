using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{
    public class Experiment : BaseObject
    {
        public int UserId { get; set; }
        public int DatasetID { get; set; }

        public User User { get; set; }
        public Database Database { get; set; }

        public ICollection<ModelConfiguration> ModelConfigurations { get; set; }
        public ICollection<DecisionRule> DecisionRules { get; set; }
    }
}
