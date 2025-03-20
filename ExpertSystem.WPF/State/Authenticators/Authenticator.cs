using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.State.Authenticators
{
    public class Authenticator : IAuthenticator
    {
        private readonly IAuthenticationService _authenticationService;

        public Authenticator(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        private User _currentUser;
        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }
            private set
            {
                _currentUser = value;
                StateChanged?.Invoke();
            }
        }

        public bool IsLoggedIn => CurrentUser != null;

        public event Action StateChanged;

        public async Task Login(string nickname, string password)
        {
            CurrentUser = await _authenticationService.Login(nickname, password);
        }

        public async Task Logout()
        {
            CurrentUser = null;
            await Task.CompletedTask;
        }

        public async Task<RegistrationResult> Register(string email, string nickname, string password, string confirmPassword, bool isAdmin)
        {
            return await _authenticationService.Register(nickname, password, confirmPassword, email, isAdmin);
        }
    }
}
