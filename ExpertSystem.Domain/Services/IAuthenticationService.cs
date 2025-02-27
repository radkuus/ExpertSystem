using ExpertSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.Domain.Services
{
    public enum RegistrationResult
    {
        Success,
        PasswordsDoNotMatch,
        EmailAlreadyTaken,
        NicknameAlreadyTaken
    }

    public interface IAuthenticationService
    {
        Task<RegistrationResult> Register(string nickname, string password, string confirmPassword, string email, bool isAdmin);
        Task<User> Login(string nickname, string password);
    }
}
