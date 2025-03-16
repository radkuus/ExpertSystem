using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.WPF.State.Authenticators;

namespace ExpertSystem.WPF.ViewModels.Factories
{
    public class HomeViewModelFactory : IExpertSystemViewModelFactory<HomeViewModel>
    {
        private readonly IAuthenticator _authenticator;

        public HomeViewModelFactory(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        public HomeViewModel CreateViewModel()
        {
            return new HomeViewModel(_authenticator);
        }
    }
}
