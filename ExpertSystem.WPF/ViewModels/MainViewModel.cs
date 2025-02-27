using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpertSystem.WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IExpertSystemViewModelAbstractFactory _viewModelAbstractFactory;

        public INavigator Navigator { get; set; }
        public IAuthenticator Authenticator { get; }
        public ICommand UpdateCurrentViewModelCommand { get; }

        public MainViewModel(INavigator navigator, IExpertSystemViewModelAbstractFactory viewModelAbstractFactory, IAuthenticator authenticator)
        {
            Navigator = navigator;
            Authenticator = authenticator;
            _viewModelAbstractFactory = viewModelAbstractFactory;

            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelAbstractFactory);

            UpdateCurrentViewModelCommand.Execute(ViewType.Login);
        }
    }
}
