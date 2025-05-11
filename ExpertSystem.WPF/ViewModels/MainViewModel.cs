using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels.Factories;
using ExpertSystem.WPF.ViewModels;
using System.Windows.Input;

public class MainViewModel : BaseViewModel
{
    private readonly IExpertSystemViewModelFactory _viewModelAbstractFactory;
    private readonly INavigator _navigator;
    private readonly IAuthenticator _authenticator;
    private bool _areResultsGenerated;
    public bool IsUserLoggedIn => _authenticator.IsUserLoggedIn;
    public bool IsAdminLoggedIn => _authenticator.IsAdminLoggedIn;

    public BaseViewModel CurrentViewModel => _navigator.CurrentViewModel;

    public ViewType CurrentViewType
    {
        get => _navigator.CurrentViewType;
        set
        {
            _navigator.CurrentViewType = value;
            OnPropertyChanged(nameof(CurrentViewType));
        }
    }

    public ICommand UpdateCurrentViewModelCommand { get; }

    public MainViewModel(INavigator navigator, IExpertSystemViewModelFactory viewModelAbstractFactory, IAuthenticator authenticator)
    {
        _navigator = navigator;
        _authenticator = authenticator;
        _viewModelAbstractFactory = viewModelAbstractFactory;
        _areResultsGenerated = false;
        _navigator.StateChanged += Navigator_StateChanged;
        _authenticator.StateChanged += Authenticator_StateChanged;

        UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelAbstractFactory);
        UpdateCurrentViewModelCommand.Execute(ViewType.Login);
    }

    public void Navigator_StateChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
        OnPropertyChanged(nameof(CurrentViewType));
    }

    public void Authenticator_StateChanged()
    {
        OnPropertyChanged(nameof(IsUserLoggedIn));
        OnPropertyChanged(nameof(IsAdminLoggedIn));
        if (!IsUserLoggedIn)
        {
            AreResultsGenerated = false;
            _navigator.CurrentViewModel = _viewModelAbstractFactory.CreateViewModel(ViewType.Login);
            _navigator.CurrentViewType = ViewType.Login;
        }
    }
    public bool AreResultsGenerated
    {
        get => _areResultsGenerated;
        set
        {
            _areResultsGenerated = value;
            OnPropertyChanged(nameof(AreResultsGenerated));
        }
    }
}