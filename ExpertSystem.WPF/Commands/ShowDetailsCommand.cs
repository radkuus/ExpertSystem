using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework.Services;
using ExpertSystem.WPF.ViewModels;
using System.Windows.Input;

public class ShowDetailsCommand : ICommand
{
    private readonly HistoryViewModel _historyVm;
    private readonly IDialogService _dialogService;
    private readonly GenericDataService<ModelConfiguration> _configService;
    private readonly GenericDataService<ModelResult> _resultService;
    private readonly GenericDataService<DecisionRule> _rulesService;

    public ShowDetailsCommand(
        HistoryViewModel historyVm,
        IDialogService dialogService,
        GenericDataService<ModelConfiguration> configService,
        GenericDataService<ModelResult> resultService,
        GenericDataService<DecisionRule> rulesService)
    {
        _historyVm = historyVm;
        _dialogService = dialogService;
        _configService = configService;
        _resultService = resultService;
        _rulesService = rulesService;
    }

    public bool CanExecute(object? parameter) => true;

    public async void Execute(object? parameter)
    {
        var exp = _historyVm.SelectedExperiment;
        if (exp == null) return;

        var detailsVm = new ExperimentDetailsViewModel(_configService, _resultService, _rulesService)
        {
            ExperimentId = exp.ExperimentId,
            DatasetName = exp.Dataset
        };

        await detailsVm.LoadResultsAsync();

        _dialogService.ShowExperimentDetails(detailsVm);
    }

    public event EventHandler? CanExecuteChanged
    {
        // Komentarz: CommandManager jest globalny (statyczny), tzn. że wiekszosc aktywności uzytkownika wywołuję
        // sprawdzenie CanExecute- w efekcie program działa troche mniej wydajnie (niezauwazalnie)- ale aktualnie
        // tylko 2 przyciski to mają
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
