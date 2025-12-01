using ExpertSystem.Domain.Contracts;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.IO;
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
                {
                    _adminViewModel.ErrorMessage = "No user selected.";
                    return;
                }

                var (password1, password2) = parameter is Tuple<string, string> passwordData
                   ? (passwordData.Item1, passwordData.Item2)
                   : (string.Empty, string.Empty);



                string? nicknameToEdit = string.IsNullOrEmpty(_adminViewModel.Nickname) ? null : _adminViewModel.Nickname;
                string? emailToEdit = string.IsNullOrEmpty(_adminViewModel.Email) ? null : _adminViewModel.Email;
                string? passwordToEdit = string.IsNullOrEmpty(password1) ? null : password1;
                string? confirmPasswordToEdit = string.IsNullOrEmpty(password2) ? null : password2;

                System.Diagnostics.Debug.WriteLine($"Raw: '{password1}', '{password2}'");
                System.Diagnostics.Debug.WriteLine($"Final: '{passwordToEdit}', '{confirmPasswordToEdit}'");

                EditResult editResult = await _authenticator.Edit(
                    _adminViewModel.SelectedUser.Id,
                    nicknameToEdit,
                    passwordToEdit,
                    confirmPasswordToEdit,
                    emailToEdit);

                switch (editResult)
                {
                    case EditResult.Success:
                        _adminViewModel.ErrorMessage = "Edition ended successfully.";
                        string? oldNickname = _adminViewModel.SelectedUser.Nickname;
                        await ((BaseAsyncCommand)_adminViewModel.DisplayUsersCommand).ExecuteAsync(null);

                        if (nicknameToEdit != null)
                        {
                            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.Parent.Parent.FullName;
                            string datasetsFolderPath = Path.Combine(projectDirectory, "Datasets");

                            string oldFolderPath = Path.Combine(datasetsFolderPath, oldNickname);
                            string newFolderPath = Path.Combine(datasetsFolderPath, nicknameToEdit);

                            if (Directory.Exists(oldFolderPath) && !Directory.Exists(newFolderPath))
                            {
                                Directory.Move(oldFolderPath, newFolderPath);
                            }
                        }
                        break;
                    case EditResult.UserNotFound:
                        _adminViewModel.ErrorMessage = "User not found.";
                        break;
                    case EditResult.NicknameAlreadyTaken:
                        _adminViewModel.ErrorMessage = "An account for this username already exists.";
                        break;
                    case EditResult.InvalidNicknameFormat:
                        _adminViewModel.ErrorMessage = "The username must be between 3 and 15 characters long and must begin with a letter or a number.";
                        break;
                    case EditResult.OnlyOnePasswordEntered:
                        _adminViewModel.ErrorMessage = "Both password and confirmation must be provided or both left empty.";
                        break;
                    case EditResult.PasswordsDoNotMatch:
                        _adminViewModel.ErrorMessage = "Password does not match confirm password.";
                        break;
                    case EditResult.InvalidPasswordFormat:
                        _adminViewModel.ErrorMessage = "The password must be between 8 and 20 characters long and contain at least one capital letter and one number.";
                        break;
                    case EditResult.EmailAlreadyTaken:
                        _adminViewModel.ErrorMessage = "An account for this email already exists.";
                        break;
                    case EditResult.InvalidEmailFormat:
                        _adminViewModel.ErrorMessage = "The email must be in a valid format (e.g., something@domain.com) with a domain extension of at least 2 characters.";
                        break;
                    case EditResult.NoChangesDetected:
                        _adminViewModel.ErrorMessage = "No changes detected.";
                        break;
                    default:
                        _adminViewModel.ErrorMessage = "Edit failed.";
                        break;
                }
            }
            catch (Exception)
            {
                _adminViewModel.ErrorMessage = "An unexpected error occurred during edit.";
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return _adminViewModel.SelectedUser != null;
        }
    }
}
