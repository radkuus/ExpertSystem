using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.State.Authenticators
{
    public interface IAuthenticator
    {
        User CurrentUser { get; }
        bool IsUserLoggedIn { get; }
        bool IsAdminLoggedIn { get; }

        event Action StateChanged;

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="nickname">User's nickname.</param>
        /// <param name="password">User's password.</param>
        /// <param name="confirmPassword">User's confirmed password.</param>
        /// <returns>Result of the registration.</returns>
        /// <exception cref="Exception">Raised when the registration fails.</exception>
        Task<RegistrationResult> Register(string email, string nickname, string password, string confirmPassword, bool isAdmin);
        /// <summary>
        /// Login to the application.
        /// </summary>
        /// <param name="nickname">User's nickname.</param>
        /// <param name="password">User's password.</param>
        /// <exception cref="UserNotFoundException">Raised when nickname doesn't exist.</exception>
        /// <exception cref="InvalidPasswordException">Raised when the password is incorrect.</exception>
        /// <exception cref="Exception">Raised when unexpected error occurs.</exception>
        Task Login(string nickname, string password);

        /// <summary>
        /// Edits the user.
        /// </summary>
        /// <param name="id">User's id.</param>
        /// <param name="nickname">User's nickname.</param>
        /// <param name="password">User's password.</param>
        /// <param name="confirmPassword">User's confirmed password.</param>
        /// <param name="email">User's email.</param>
        /// <returns>Result of the edit.</returns>
        /// <exception cref="Exception">Raised when edit operation fails.</exception>
        Task<EditResult> Edit(int id, string nickname, string password, string confirmPassword, string email);

        Task Logout();
    }
}
