using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public string Nickanme { get; set; }

        public UserNotFoundException(string nickname)
        {
            Nickanme = nickname;
        }

        public UserNotFoundException(string message, string nickname) : base(message)
        {
            Nickanme = nickname;
        }

        public UserNotFoundException(string message, Exception innerException, string nickname) : base(message, innerException)
        {
            Nickanme = nickname;
        }
    }
}
