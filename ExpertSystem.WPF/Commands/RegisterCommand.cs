using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpertSystem.Domain.Contracts;

namespace ExpertSystem.WPF.Commands
{
    public class RegisterCommand : BaseAsyncCommand
    {
        private readonly IRegistrationData _registerViewModel;
        private readonly IAuthenticator _authenticator;
        private readonly IRenavigator _renavigator;

        public RegisterCommand(IRegistrationData registerViewModel, IAuthenticator authenticator, IRenavigator? renavigator)
        {
            _registerViewModel = registerViewModel;
            _authenticator = authenticator;
            _renavigator = renavigator;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            try
            {
                var (password1, password2) = ((string, string))parameter!;
                RegistrationResult registrationResult = await _authenticator.Register(
                    _registerViewModel.Email,
                    _registerViewModel.Nickname,
                    password1,
                    password2,
                    false);

                switch (registrationResult)
                {
                    case RegistrationResult.Success:
                        if (_authenticator.CurrentUser == null)
                        {
                            _renavigator.Renavigate();
                        }
                        break;
                    case RegistrationResult.PasswordsDoNotMatch:
                        _registerViewModel.ErrorMessage = "Password does not match confirm password.";
                        break;
                    case RegistrationResult.EmailAlreadyTaken:
                        _registerViewModel.ErrorMessage = "An account for this email already exists.";
                        break;
                    case RegistrationResult.NicknameAlreadyTaken:
                        _registerViewModel.ErrorMessage = "An account for this username already exists.";
                        break;
                    default:
                        _registerViewModel.ErrorMessage = "Registration failed.";
                        break;
                }
            }
            catch (Exception)
            {
                _registerViewModel.ErrorMessage = "Registration failed.";
            }
        }
    }
}
