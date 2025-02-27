using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.State.Authenticators
{
    public interface IAuthenticator
    {
        User CurrentUser { get; }
        bool IsLoggedIn { get;  }

        Task<RegistrationResult> Register(string email, string nickname, string password, string confirmPassword, bool isAdmin);
        Task<bool> Login(string nickname, string password);
        void Logout();
    }
}
