using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Exceptions
{
    public class InvalidPasswordException : Exception
    {
        public string Nickname { get; set; }
        public string Password { get; set; }

        public InvalidPasswordException(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }

        public InvalidPasswordException(string message, string nickname, string password) : base(message)
        {
            Nickname = nickname;
            Password = password;
        }

        public InvalidPasswordException(string message, Exception innerException, string nickname, string password) : base(message, innerException)
        {
            Nickname = nickname;
            Password = password;
        }
    }
}
