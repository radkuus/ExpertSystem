using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{
    public enum ModelType
    {
        KNN,
        LogisticRegression,
        Bayes,
        NeuralNetwork,
        Own
    }
    public class ModelConfiguration : BaseObject
    {
        public int ExperimentId { get; set; }
        public ModelType ModelType { get; set; }
        public string Hyperparameters { get; set; }


        public Experiment Experiment { get; set; }
        public ICollection<ModelResult> ModelResults { get; set; }

    }
}
