using ExpertSystem.Domain.Contracts;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.Domain.Contracts;
using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExpertSystem.WPF.ViewModels
{
    public class AdminViewModel : BaseViewModel, IRegistrationData
    {
        private readonly IUserService _userService;
        private readonly IDatasetService _datasetService;
        private readonly INavigator _navigator;
        private readonly IAuthenticator _authenticator;
        private readonly CreateViewModel<LoginViewModel> _createLoginViewModel;

        public ObservableCollection<User> Users { get; } = new();

        public ICommand DisplayUsersCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand EditUserCommand { get; }

        public AdminViewModel(IUserService userService,
            IAuthenticator authenticator,
            INavigator navigator,
            CreateViewModel<LoginViewModel> createLoginViewModel,
            IDatasetService datasetService)
        {
            ErrorMessageViewModel = new MessageViewModel();

            _userService = userService;
            _datasetService = datasetService;
            _createLoginViewModel = createLoginViewModel;
            _authenticator = authenticator;
            _navigator = navigator;

            DisplayUsersCommand = new DisplayUsersCommand(this, userService, authenticator);
            LogoutCommand = new LogoutCommand(createLoginViewModel, authenticator, navigator);
            DeleteUserCommand = new DeleteUserCommand(this, userService, datasetService);
            RegisterCommand = new RegisterCommand(this, authenticator, null, userService);
            EditUserCommand = new EditUserCommand(this, authenticator);

            DisplayUsersCommand.Execute(null);
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged(nameof(SelectedUser));
                    if (_selectedUser != null)
                    {
                        Nickname = _selectedUser.Nickname;
                        Email = _selectedUser.Email;
                    }
                    else
                    {

                        Nickname = string.Empty;
                        Email = string.Empty;
                    }
                }
            }
        }
        
        private string _nickname = "";
        public string Nickname
        {
            get { return _nickname; }
            set
            {
                _nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }

        private string _email = "";
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }


        private string _password = "";
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _confirmPassword = "";
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public MessageViewModel ErrorMessageViewModel { get; }
        public string ErrorMessage
        {
            set => ErrorMessageViewModel.Message = value;
        }
    }
}
