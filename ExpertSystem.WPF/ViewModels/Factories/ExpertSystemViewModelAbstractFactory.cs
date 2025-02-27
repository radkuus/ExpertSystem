using ExpertSystem.WPF.State.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels.Factories
{
    public class ExpertSystemViewModelAbstractFactory : IExpertSystemViewModelAbstractFactory
    {
        private readonly IExpertSystemViewModelFactory<HomeViewModel> _homeViewModelFactory;
        private readonly IExpertSystemViewModelFactory<AnalysisViewModel> _analysisViewModelFactory;
        private readonly IExpertSystemViewModelFactory<LoginViewModel> _loginViewModelFactory;

        public ExpertSystemViewModelAbstractFactory(IExpertSystemViewModelFactory<HomeViewModel> homeViewModelFactory, 
            IExpertSystemViewModelFactory<AnalysisViewModel> analysisViewModelFactory,
            IExpertSystemViewModelFactory<LoginViewModel> loginViewModelFactory)
        {
            _homeViewModelFactory = homeViewModelFactory;
            _analysisViewModelFactory = analysisViewModelFactory;
            _loginViewModelFactory = loginViewModelFactory;
        }

        public BaseViewModel CreateViewModel(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Login:
                    return _loginViewModelFactory.CreateViewModel();
                case ViewType.Home:
                    return _homeViewModelFactory.CreateViewModel();
                case ViewType.Analysis:
                    return _analysisViewModelFactory.CreateViewModel();
                default:
                    throw new ArgumentException("ViewType doesn't have ViewModel", "viewType");
            }
        }
    }
}
