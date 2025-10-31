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
        public Dataset Dataset { get; set; }

        public ICollection<ModelConfiguration> ModelConfigurations { get; set; }
       
    }
}
