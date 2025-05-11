using ExpertSystem.WPF.ViewModels;
using ExpertSystem.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Specialized;
using System.ComponentModel;
using ExpertSystem.WPF.Core;
using ExpertSystem.WPF.ViewModels.Factories;

namespace ExpertSystem.WPF.State.Navigators
{
    public class Navigator : INavigator
    {
        private BaseViewModel _currentViewModel;
        private ViewType _currentViewType;

        public BaseViewModel CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                StateChanged?.Invoke();
            }
        }

        public ViewType CurrentViewType
        {
            get => _currentViewType;
            set
            {
                _currentViewType = value;
                StateChanged?.Invoke();
            }
        }

        public event Action StateChanged;
    }
}
