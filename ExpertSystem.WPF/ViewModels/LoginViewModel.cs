using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpertSystem.WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _nickname = "";
        public string Nickname
        {
            get
            {
                return _nickname;
            }
            set
            {
                _nickname = value;
                OnPropertyChanged(nameof(Nickname));
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToViewRegisterCommand { get; }

        public LoginViewModel(IAuthenticator authenticator, IRenavigator loginRenavigator, IRenavigator goToRegisterRenavigator)
        {
            ErrorMessageViewModel = new MessageViewModel();

            LoginCommand = new LoginCommand(this, authenticator, loginRenavigator);
            GoToViewRegisterCommand = new RenavigateCommand(goToRegisterRenavigator);
        }

        public MessageViewModel ErrorMessageViewModel { get; }

        public string ErrorMessage
        {
            set => ErrorMessageViewModel.Message = value;
        }

        public bool CanLogin => !string.IsNullOrEmpty(Nickname);
    }
}
