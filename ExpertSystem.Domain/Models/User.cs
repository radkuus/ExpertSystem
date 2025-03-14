using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Models
{
    public class User : BaseObject
    {
        public string Nickname { get; set; }

        public string PasswordHashed { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        public ICollection<Database> Databases { get; set; }
        public ICollection<Experiment> Experiments { get; set; }


        public User(string nickname, string passwordHashed, string email, bool isAdmin)
        {
            Nickname = nickname;
            PasswordHashed = passwordHashed;
            Email = email;
            IsAdmin = isAdmin;
        }

        public User()
        {
        }
    }
}
