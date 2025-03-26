using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.Commands
{
    class DeleteUserCommand : BaseAsyncCommand
    {
        private readonly AdminViewModel _adminViewModel;
        private readonly IUserService _userService;

        public DeleteUserCommand(AdminViewModel adminViewModel, IUserService userService)
        {
            _adminViewModel = adminViewModel;
            _userService = userService;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            if (_adminViewModel.SelectedUser == null)
                return;

            await _userService.Delete(_adminViewModel.SelectedUser.Id);
            _adminViewModel.Users.Remove(_adminViewModel.SelectedUser);
            _adminViewModel.SelectedUser = null;
        }

        public override bool CanExecute(object? parameter)
        {
            return _adminViewModel.SelectedUser != null;
        }
    }
}
