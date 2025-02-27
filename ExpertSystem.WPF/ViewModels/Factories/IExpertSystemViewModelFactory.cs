using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels.Factories
{
    public interface IExpertSystemViewModelFactory<T> where T : BaseViewModel
    {
        T CreateViewModel();
    }
}
