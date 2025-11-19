using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Exceptions
{
    public class DatasetAlreadyExistsException : Exception
    {
        public int UserId { get; set; }
        public string OwnerName { get; set; }

        public DatasetAlreadyExistsException(int userId, string ownerName)
        {
            UserId = userId;
            OwnerName = ownerName;
        }

        public DatasetAlreadyExistsException(string message, int userId, string ownerName) : base(message)
        {
            UserId = userId;
            OwnerName = ownerName;
        }

        public DatasetAlreadyExistsException(string message, Exception innerException, int userId, string ownerName) : base(message, innerException)
        {
            UserId = userId;
            OwnerName = ownerName;
        }
    }
}
