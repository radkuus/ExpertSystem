using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.Commands
{
    public class LogoutCommand : BaseAsyncCommand
    {
        private readonly IAuthenticator _authenticator;
        private readonly INavigator _navigator;
        private readonly CreateViewModel<LoginViewModel> _createLoginViewModel;

        public LogoutCommand(CreateViewModel<LoginViewModel> createLoginViewModel, IAuthenticator authenticator, INavigator navigator)
        {
            _createLoginViewModel = createLoginViewModel;
            _authenticator = authenticator;
            _navigator = navigator;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            await _authenticator.Logout();
            _navigator.CurrentViewModel = _createLoginViewModel(); ;
        }
    }
}
