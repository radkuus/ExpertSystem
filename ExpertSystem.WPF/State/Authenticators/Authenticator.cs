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
    public class Authenticator : ObservableObject, IAuthenticator
    {
        private readonly IAuthenticationService _authenticationService;

        public Authenticator(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        private User _curremtUser;
        public User CurrentUser
        {
            get
            {
                return _curremtUser;
            }
            private set
            {
                _curremtUser = value;
                OnPropertyChanged(nameof(CurrentUser));
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

        public bool IsLoggedIn => CurrentUser != null;

        public async Task<bool> Login(string nickname, string password)
        {
            bool success = true;
            try
            {
                CurrentUser = await _authenticationService.Login(nickname, password);
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public async Task<RegistrationResult> Register(string email, string nickname, string password, string confirmPassword, bool isAdmin)
        {
            return await _authenticationService.Register(nickname, password, confirmPassword, email, isAdmin);
        }
    }
}
