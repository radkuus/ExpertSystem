using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Exceptions
{
    public class DatasetNotFoundException : Exception
    {
        public int? Id { get; set; }
        public string? OwnerName { get; set; }
        public string? DatasetName { get; set; }

        public DatasetNotFoundException(int id)
        {
            Id = id;
        }

        public DatasetNotFoundException(string message, int id) : base(message)
        {
            Id = id;
        }

        public DatasetNotFoundException(string message, Exception innerException, int id) : base(message, innerException)
        {
            Id = id;
        }

        public DatasetNotFoundException(string owner, string datasetName)
        {
            OwnerName = owner;
            DatasetName = datasetName;
        }

        public DatasetNotFoundException(string message, string owner, string datasetName) : base(message)
        {
            OwnerName = owner;
            DatasetName = datasetName;
        }

        public DatasetNotFoundException(string message, Exception innerException, string owner, string datasetName) : base(message, innerException)
        {
            OwnerName = owner;
            DatasetName = datasetName;
        }

    }
}
