using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework;
using ExpertSystem.EntityFramework.Services;
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
        //IAuthenticationService authentication = serviceProvider.GetRequiredService<IAuthenticationService>();
        //authentication.Register("admin", "1234", "1234", "admin@gmail.com", true);

        Window window  = serviceProvider.GetRequiredService<MainWindow>();
        window.Show();

        base.OnStartup(e);
    }

    private IServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton <ExpertSystemDbContextFactory>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        //services.AddSingleton<IDataService<User>, GenericDataService<User>>();
        services.AddSingleton<IUserService, UserDataService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddSingleton<IExpertSystemViewModelAbstractFactory, ExpertSystemViewModelAbstractFactory>();
        services.AddSingleton<IExpertSystemViewModelFactory<HomeViewModel>, HomeViewModelFactory>();
        services.AddSingleton<IExpertSystemViewModelFactory<AnalysisViewModel>, AnalysisViewModelFactory>();

        services.AddSingleton<IExpertSystemViewModelFactory<LoginViewModel>>((services) =>
            new LoginViewModelFactory(services.GetRequiredService<IAuthenticator>(), 
            new ViewModelFactoryRenavigator<HomeViewModel>(services.GetRequiredService<INavigator>(),
            services.GetRequiredService< IExpertSystemViewModelFactory < HomeViewModel >>())));

        //services.AddSingleton<IExpertSystemViewModelFactory<LoginViewModel>>((services) =>
        //    new LoginViewModelFactory(services.GetRequiredService<IAuthenticator>(),
        //    new ViewModelFactoryRenavigator<AnalysisViewModel>(services.GetRequiredService<INavigator>(),
        //    services.GetRequiredService<IExpertSystemViewModelFactory<AnalysisViewModel>>())));

        services.AddScoped<INavigator, Navigator>();
        services.AddScoped<IAuthenticator, Authenticator>();
        services.AddScoped<MainViewModel>();

        services.AddScoped<MainWindow>(s => new MainWindow(s.GetRequiredService<MainViewModel>()));

        return services.BuildServiceProvider();
    }
}

