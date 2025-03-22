using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels.Factories;

namespace ExpertSystem.WPF.ViewModels
{
    public class AnalysisViewModel : BaseViewModel
    {
        private readonly INavigator _navigator;
        private readonly IExpertSystemViewModelFactory _viewModelAbstractFactory;

        public ICommand UpdateCurrentViewModelCommand { get; }

        public AnalysisViewModel(INavigator navigator, IExpertSystemViewModelFactory viewModelAbstractFactory)
        {
            _navigator = navigator;
            _viewModelAbstractFactory = viewModelAbstractFactory;

            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelAbstractFactory);
        }

        

    }
}
