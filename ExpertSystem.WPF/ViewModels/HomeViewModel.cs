using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.WPF.State.Authenticators;

namespace ExpertSystem.WPF.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IAuthenticator _authenticator;

        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            private set
            {
                _nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }

        public HomeViewModel(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            Nickname = _authenticator.CurrentUser.Nickname;
        }
    }
}
