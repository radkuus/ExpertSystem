using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework;
using ExpertSystem.EntityFramework.Services;
using ExpertSystem.WPF.Helpers.Sample;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Datasets;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels;
using ExpertSystem.WPF.ViewModels.Factories;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ExpertSystem.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        IServiceProvider serviceProvider = CreateServiceProvider();
        IAuthenticationService authentication = serviceProvider.GetRequiredService<IAuthenticationService>();
        authentication.Register("kamil", "Testowanko1", "Testowanko1", "kamil.kamil@gmail.com", true);
        Window window = serviceProvider.GetRequiredService<MainWindow>();
        window.Show();

        base.OnStartup(e);
    }

    private IServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<ExpertSystemDbContextFactory>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        //services.AddSingleton<IDataService<User>, GenericDataService<User>>();
        services.AddSingleton<IUserService, UserDataService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<IDatasetService, DatasetService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IDatasetStatisticsService, DatasetStatisticsService>();
        services.AddSingleton<IApiService, ApiService>();
        services.AddScoped(typeof(GenericDataService<>), typeof(GenericDataService<>));
        services.AddSingleton<IExperimentService, ExperimentService>();
        services.AddSingleton<IResultsViewModelFactory, ResultsViewModelFactory>();
        services.AddSingleton<UserSample>();
        services.AddSingleton<CreateViewModel<ResultsViewModel>>(provider =>
        {
            return () => provider.GetRequiredService<ResultsViewModel>();
        });
        services.AddSingleton<RegisterViewModel>();

        services.AddSingleton<CreateViewModel<RegisterViewModel>>(services =>
        {
            return () => new RegisterViewModel(
                services.GetRequiredService<IAuthenticator>(),
                services.GetRequiredService<ViewModelFactoryRenavigator<LoginViewModel>>());
        });
        services.AddSingleton<IExpertSystemViewModelFactory, ExpertSystemViewModelAbstractFactory>();

        services.AddSingleton<CreateViewModel<HomeViewModel>>(services =>
        {
            return () => new HomeViewModel(
                services.GetRequiredService<IAuthenticator>(),
                services.GetRequiredService<IDatasetService>(),
                services.GetRequiredService<IFileDialogService>(),
                services.GetRequiredService<CreateViewModel<LoginViewModel>>(),
                services.GetRequiredService<INavigator>(),
                services.GetRequiredService<IExpertSystemViewModelFactory>(),
                services.GetRequiredService<IDatasetStore>()
            );
        });

        services.AddSingleton<CreateViewModel<AnalysisViewModel>>(provider =>
        {
            return () => new AnalysisViewModel(
                provider.GetRequiredService<INavigator>(),
                provider.GetRequiredService<IExpertSystemViewModelFactory>(),
                provider.GetRequiredService<IAuthenticator>(),
                provider.GetRequiredService<IDatasetService>(),
                provider.GetRequiredService<IDatasetStore>(),
                provider.GetRequiredService<IDialogService>(),
                provider.GetRequiredService<IDatasetStatisticsService>(),
                provider.GetRequiredService<IDialogService>(),
                provider.GetRequiredService<IApiService>(),
                provider.GetRequiredService<IExperimentService>(),
                provider.GetRequiredService<CreateViewModel<ResultsViewModel>>(),
                provider.GetRequiredService<MainViewModel>(),
                provider.GetRequiredService<UserSample>()
            );
        });

        services.AddSingleton<CreateViewModel<ResultsViewModel>>(provider =>
            () => new ResultsViewModel(
                provider.GetRequiredService<INavigator>(),
                provider.GetRequiredService<IExpertSystemViewModelFactory>()
            )
        );


        services.AddSingleton<CreateViewModel<HistoryViewModel>>(provider =>
            () => new HistoryViewModel(
                provider.GetRequiredService<GenericDataService<Experiment>>(),
                provider.GetRequiredService<GenericDataService<Dataset>>(),
                provider.GetRequiredService<GenericDataService<ModelConfiguration>>(),
                provider.GetRequiredService<GenericDataService<ModelResult>>(),
                provider.GetRequiredService<IDialogService>(),
                provider.GetRequiredService<GenericDataService<DecisionRule>>()
            )
        );





        services.AddSingleton<ViewModelFactoryRenavigator<LoginViewModel>>();

        services.AddSingleton<ResultsViewModel>();
        services.AddSingleton<CreateViewModel<ResultsViewModel>>(provider =>
            () => provider.GetRequiredService<ResultsViewModel>()
        );


        services.AddSingleton<CreateViewModel<AdminViewModel>>(services =>
        {
            return () => new AdminViewModel(
                services.GetRequiredService<IUserService>(),
                services.GetRequiredService<IAuthenticator>(),
                services.GetRequiredService<INavigator>(),
                services.GetRequiredService<CreateViewModel<LoginViewModel>>());
        });

        services.AddSingleton<AdminDatasetViewModel>();
        services.AddSingleton<CreateViewModel<AdminDatasetViewModel>>(provider =>
            () => provider.GetRequiredService<AdminDatasetViewModel>()
        );

        services.AddSingleton<ViewModelFactoryRenavigator<RegisterViewModel>>();
        services.AddSingleton<ViewModelFactoryRenavigator<HomeViewModel>>();
        services.AddSingleton<ViewModelFactoryRenavigator<AdminViewModel>>();
        services.AddSingleton<CreateViewModel<LoginViewModel>>(services =>
        {
            return () => new LoginViewModel(
                services.GetRequiredService<IAuthenticator>(),
                services.GetRequiredService<ViewModelFactoryRenavigator<HomeViewModel>>(),
                services.GetRequiredService<ViewModelFactoryRenavigator<RegisterViewModel>>(),
                services.GetRequiredService<ViewModelFactoryRenavigator<AdminViewModel>>());
        });

        services.AddSingleton<IDatasetStore, DatasetStore>();
        services.AddScoped<INavigator, Navigator>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddScoped<IAuthenticator, Authenticator>();
        services.AddScoped<MainViewModel>();

        services.AddScoped<MainWindow>(services => new MainWindow(services.GetRequiredService<MainViewModel>()));

        return services.BuildServiceProvider();
    }
}

