using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Contracts
{
    public interface IRegistrationData
    {
        string Email { get; set; }
        string Nickname { get; set; }

        string ErrorMessage { set; }
    }
}
