using ExpertSystem.WPF.State.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels.Factories
{
    public interface IExpertSystemViewModelAbstractFactory
    {
        BaseViewModel CreateViewModel(ViewType viewType);
    }
}
