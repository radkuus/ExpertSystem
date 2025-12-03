using ExpertSystem.Domain.Contracts;
using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Services;
using ExpertSystem.EntityFramework.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.Commands
{
    public class RegisterCommand : BaseAsyncCommand
    {
        private readonly IRegistrationData _registerViewModel;
        private readonly IAuthenticator _authenticator;
        private readonly IRenavigator _renavigator;
        private readonly IUserService _userService;

        public RegisterCommand(IRegistrationData registerViewModel, IAuthenticator authenticator, IRenavigator? renavigator, IUserService userService)
        {
            _registerViewModel = registerViewModel;
            _authenticator = authenticator;
            _renavigator = renavigator;
            _userService = userService;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            try
            {
                bool anyUsers = await _userService.Any();
                var (password1, password2) = ((string, string))parameter!;
                RegistrationResult registrationResult;

                if (anyUsers == true)
                {
                    registrationResult = await _authenticator.Register(
                        _registerViewModel.Email,
                        _registerViewModel.Nickname,
                        password1,
                        password2,
                        false);
                }
                else
                {
                    registrationResult = await _authenticator.Register(
                        _registerViewModel.Email,
                        _registerViewModel.Nickname,
                        password1,
                        password2,
                        true);
                }

                switch (registrationResult)
                {
                    case RegistrationResult.Success:
                        _registerViewModel.ErrorMessage = "";
                        if (_authenticator.CurrentUser == null)
                        {
                            _renavigator.Renavigate();
                        }
                        break;
                    case RegistrationResult.NicknameAlreadyTaken:
                        _registerViewModel.ErrorMessage = "An account for this username already exists.";
                        break;
                    case RegistrationResult.InvalidNicknameFormat:
                        _registerViewModel.ErrorMessage = "The username must be between 3 and 15 characters long and must begin with a letter or a number";
                        break;
                    case RegistrationResult.PasswordsDoNotMatch:
                        _registerViewModel.ErrorMessage = "Password does not match confirm password.";
                        break;
                    case RegistrationResult.InvalidPasswordFormat:
                        _registerViewModel.ErrorMessage = "The password must be between 8 and 20 characters long and contain at least one capital letter and one number.";
                        break;
                    case RegistrationResult.EmailAlreadyTaken:
                        _registerViewModel.ErrorMessage = "An account for this email already exists.";
                        break;
                    case RegistrationResult.InvalidEmailFormat:
                        _registerViewModel.ErrorMessage = "The email must be in a valid format (e.g., something@domain.com) and the domain extension must be at least 2 characters long.";
                        break;
                    default:
                        _registerViewModel.ErrorMessage = "Registration failed.";
                        break;
                }
            }
            catch (Exception)
            {
                _registerViewModel.ErrorMessage = "An unexpected error occurred during registration.";
            }
        }
    }
}
