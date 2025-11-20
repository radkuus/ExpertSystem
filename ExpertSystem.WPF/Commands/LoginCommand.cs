using ExpertSystem.Domain.Exceptions;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels;
using ExpertSystem.WPF.ViewModels.Factories;
using Microsoft.EntityFrameworkCore.Metadata;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpertSystem.WPF.Commands
{
    public class LoginCommand : BaseAsyncCommand
    {
        private readonly LoginViewModel _loginViewModel;
        private readonly IAuthenticator _authenticator;
        private readonly IRenavigator _goToHomeRenavigator;
        private readonly IRenavigator _goToAdminRenavigator;
        private readonly INavigator _navigator;

        public LoginCommand(LoginViewModel loginViewModel, IAuthenticator authenticator, 
            IRenavigator goToHomeRenavigator, IRenavigator goToAdminRenavigator, INavigator navigator)
        {
            _loginViewModel = loginViewModel;
            _authenticator = authenticator;
            _goToHomeRenavigator = goToHomeRenavigator;
            _goToAdminRenavigator = goToAdminRenavigator;
            _navigator = navigator;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            try
            {
                await _authenticator.Login(_loginViewModel.Nickname, parameter.ToString());
                if (_authenticator.IsUserLoggedIn)
                {
                    _goToHomeRenavigator.Renavigate();
                    _navigator.CurrentViewType = ViewType.Home;
                }
                else if (_authenticator.IsAdminLoggedIn)
                {
                    _goToAdminRenavigator.Renavigate();
                    _navigator.CurrentViewType = ViewType.AdminUsers;
                }

            }
            catch (UserNotFoundException)
            {
                _loginViewModel.ErrorMessage = "Nickname doesn't exist.";
            }
            catch (InvalidPasswordException)
            {
                _loginViewModel.ErrorMessage = "Invalid password.";
            }
            catch (Exception)
            {
                _loginViewModel.ErrorMessage = "Login failed.";
            }
        }
    }
}
