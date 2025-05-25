using ExpertSystem.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpertSystem.WPF.State.Navigators
{
    public enum ViewType
    {
        Login,
        Home,
        Analysis,
        History,
        Results,
        AdminUsers,
        AdminDataset
    }

    public interface INavigator
    {
        BaseViewModel CurrentViewModel { get; set; }
        ViewType CurrentViewType { get; set; } 
        event Action StateChanged;
    }
}
