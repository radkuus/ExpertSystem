using ExpertSystem.Domain.Contracts;
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

namespace ExpertSystem.WPF.Commands
{
    class EditUserCommand : BaseAsyncCommand
    {
        private readonly AdminViewModel _adminViewModel;
        private readonly IAuthenticator _authenticator;

        public EditUserCommand(AdminViewModel adminViewModel, IAuthenticator authenticator)
        {
            _adminViewModel = adminViewModel;
            _authenticator = authenticator;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            try
            {
                if (_adminViewModel.SelectedUser == null)
                    return;

                var (password1, password2) = ((string, string))parameter!;
                EditResult editResult = await _authenticator.Edit(
                    _adminViewModel.SelectedUser.Id,
                    _adminViewModel.Nickname,
                    password1,
                    password2,
                    _adminViewModel.Email);

                switch (editResult)
                {
                    case EditResult.Success:
                        _adminViewModel.ErrorMessage = "Edition ended successfully.";
                        break;
                    case EditResult.PasswordsDoNotMatch:
                        _adminViewModel.ErrorMessage = "Password does not match confirm password.";
                        break;
                    case EditResult.EmailAlreadyTaken:
                        _adminViewModel.ErrorMessage = "An account for this email already exists.";
                        break;
                    case EditResult.NicknameAlreadyTaken:
                        _adminViewModel.ErrorMessage = "An account for this username already exists.";
                        break;
                    case EditResult.UserNotFound:
                        _adminViewModel.ErrorMessage = "User not found.";
                        break;
                    case EditResult.OnlyOnePasswordEntered:
                        _adminViewModel.ErrorMessage = "Only one password was entered.";
                        break;
                    case EditResult.NoChangesDetected:
                        _adminViewModel.ErrorMessage = "No changes detected.";
                        break;
                    default:
                        _adminViewModel.ErrorMessage = "Registration failed.";
                        break;
                }
            }
            catch (Exception)
            {
                _adminViewModel.ErrorMessage = "Registration failed.";
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return _adminViewModel.SelectedUser != null;
        }
    }
}
