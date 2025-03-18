using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{
    public class Dataset : BaseObject
    {
        public int UserId { get; set; }
        public string Name { get; set; }


        public User User { get; set; }
        public ICollection<Experiment> Experiments { get; set; }

    }
}
