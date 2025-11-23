using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.Domain.Services;

namespace ExpertSystem.WPF.ViewModels
{
    public class RegisterViewModel : BaseViewModel, IRegistrationData
    {
        private readonly IUserService _userService;
        private readonly IAuthenticator _authenticator;

        private string _nickname;
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
            }
        }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand GoToViewLoginCommand { get; }

        public MessageViewModel ErrorMessageViewModel { get; }

        public string ErrorMessage
        {
            set => ErrorMessageViewModel.Message = value;
        }

        public RegisterViewModel(IAuthenticator authenticator, IRenavigator goToLoginRenavigator, IUserService userService)
        {
            _userService = userService;
            _authenticator = authenticator;

            ErrorMessageViewModel = new MessageViewModel();

            RegisterCommand = new RegisterCommand(this, authenticator, goToLoginRenavigator, userService);
            GoToViewLoginCommand = new RenavigateCommand(goToLoginRenavigator);
        }
    }
}
