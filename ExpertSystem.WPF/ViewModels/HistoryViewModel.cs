using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private readonly INavigator _navigator;

        public HistoryViewModel(INavigator navigator)
        {
            _navigator = navigator;
        }
    }
}
