using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Exceptions;
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
        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="nickname">User's nickname.</param>
        /// <param name="password">User's password.</param>
        /// <param name="confirmPassword">User's confirmed password.</param>
        /// <returns>Result of the registration.</returns>
        /// <exception cref="Exception">Raised when the registration fails.</exception>
        Task<RegistrationResult> Register(string nickname, string password, string confirmPassword, string email, bool isAdmin);

        /// <summary>
        /// Checks if login is possible and returns user.
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="password"></param>
        /// <returns>The user object.</returns>
        /// <exception cref="UserNotFoundException">Raised when nickname doesn't exist.</exception>
        /// <exception cref="InvalidPasswordException">Raised when the password is incorrect.</exception>
        /// <exception cref="Exception">Raised when unexpected error occurs.</exception>
        Task<User> Login(string nickname, string password);
    }
}
