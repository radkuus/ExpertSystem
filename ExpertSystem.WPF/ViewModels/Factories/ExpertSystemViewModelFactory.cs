﻿using ExpertSystem.WPF.State.Navigators;
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
        public ExpertSystemViewModelAbstractFactory(
            CreateViewModel<HomeViewModel> createHomeViewModel,
            CreateViewModel<AnalysisViewModel> createAnalysisViewModel,
            CreateViewModel<LoginViewModel> createLoginViewModel,
            CreateViewModel<ResultsViewModel> createResultsViewModel,
            CreateViewModel<HistoryViewModel> createHistoryViewModel)
        {
            _createHomeViewModel = createHomeViewModel;
            _createAnalysisViewModel = createAnalysisViewModel;
            _createLoginViewModel = createLoginViewModel;
            _createResultsViewModel = createResultsViewModel;
            _createHistoryViewModel = createHistoryViewModel;
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
                default:
                    throw new ArgumentException("ViewType doesn't have ViewModel", "viewType");
            }
        }
    }
}
