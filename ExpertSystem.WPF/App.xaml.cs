using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework;
using ExpertSystem.EntityFramework.Services;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.State.Authenticators;
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
        authentication.Register("kamil", "1234", "1234", "kamil@gmail.com", true);

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

        services.AddSingleton<IExpertSystemViewModelFactory, ExpertSystemViewModelAbstractFactory>();

        services.AddSingleton<CreateViewModel<HomeViewModel>>(services =>
        {
            return () => new HomeViewModel(
                services.GetRequiredService<IAuthenticator>(),
                services.GetRequiredService<IDatasetService>(),
                services.GetRequiredService<IFileDialogService>()
            );
        });

        services.AddSingleton<CreateViewModel<AnalysisViewModel>>(services =>
        {
            return () => new AnalysisViewModel();
        });

        services.AddSingleton<ViewModelFactoryRenavigator<LoginViewModel>>();
        services.AddSingleton<CreateViewModel<RegisterViewModel>>(services =>
        {
            return () => new RegisterViewModel(
                services.GetRequiredService<IAuthenticator>(),
                services.GetRequiredService<ViewModelFactoryRenavigator<LoginViewModel>>());
        });

        services.AddSingleton<ViewModelFactoryRenavigator<RegisterViewModel>>();
        services.AddSingleton<ViewModelFactoryRenavigator<HomeViewModel>>();
        services.AddSingleton<CreateViewModel<LoginViewModel>>(services =>
        {
            return () => new LoginViewModel(
                services.GetRequiredService<IAuthenticator>(),
                services.GetRequiredService<ViewModelFactoryRenavigator<HomeViewModel>>(),
                services.GetRequiredService<ViewModelFactoryRenavigator<RegisterViewModel>>());
        });

        services.AddScoped<INavigator, Navigator>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddScoped<IAuthenticator, Authenticator>();
        services.AddScoped<MainViewModel>();

        services.AddScoped<MainWindow>(services => new MainWindow(services.GetRequiredService<MainViewModel>()));

        return services.BuildServiceProvider();
    }
}

