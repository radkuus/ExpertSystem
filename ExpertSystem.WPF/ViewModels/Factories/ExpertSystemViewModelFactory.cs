using ExpertSystem.WPF.State.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.ViewModels.Factories
{
    public class ExpertSystemViewModelAbstractFactory : IExpertSystemViewModelFactory
    {
        private readonly CreateViewModel<HomeViewModel> _createHomeViewModel;
        private readonly CreateViewModel<AnalysisViewModel> _createAnalysisViewModel;
        private readonly CreateViewModel<LoginViewModel> _createLoginViewModel;
        private readonly CreateViewModel<HistoryViewModel> _createHistoryViewModel;
        private readonly CreateViewModel<ResultsViewModel> _createResultsViewModel;
        private readonly CreateViewModel<AdminViewModel> _createAdminUsersViewModel;
        private readonly CreateViewModel<AdminDatasetViewModel> _createAdminDatasetViewModel;
        public ExpertSystemViewModelAbstractFactory(
            CreateViewModel<HomeViewModel> createHomeViewModel,
            CreateViewModel<AnalysisViewModel> createAnalysisViewModel,
            CreateViewModel<LoginViewModel> createLoginViewModel,
            CreateViewModel<ResultsViewModel> createResultsViewModel,
            CreateViewModel<HistoryViewModel> createHistoryViewModel,
            CreateViewModel<AdminViewModel> createAdminUsersViewModel,
            CreateViewModel<AdminDatasetViewModel> createAdminDatasetViewModel)
        {
            _createHomeViewModel = createHomeViewModel;
            _createAnalysisViewModel = createAnalysisViewModel;
            _createLoginViewModel = createLoginViewModel;
            _createResultsViewModel = createResultsViewModel;
            _createHistoryViewModel = createHistoryViewModel;
            _createAdminUsersViewModel = createAdminUsersViewModel;
            _createAdminDatasetViewModel = createAdminDatasetViewModel;
        }

        public BaseViewModel CreateViewModel(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Login:
                    return _createLoginViewModel();
                case ViewType.Home:
                    return _createHomeViewModel();
                case ViewType.Analysis:
                    return _createAnalysisViewModel();
                case ViewType.History:
                    return _createHistoryViewModel();
                case ViewType.Results:
                    return _createResultsViewModel();
                case ViewType.AdminUsers:
                    return _createAdminUsersViewModel();
                case ViewType.AdminDataset:
                    return _createAdminDatasetViewModel();
                default:
                    throw new ArgumentException("ViewType doesn't have ViewModel", "viewType");
            }
        }
    }
}
