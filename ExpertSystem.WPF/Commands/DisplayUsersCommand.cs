using ExpertSystem.Domain.Exceptions;
using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.State.Navigators;
using ExpertSystem.WPF.ViewModels;
using Microsoft.AspNet.Identity;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem.WPF.Commands
{
    class DisplayUsersCommand : BaseAsyncCommand
    {
        private readonly AdminViewModel _adminViewModel;
        private readonly IUserService _userService;

        public DisplayUsersCommand(AdminViewModel adminViewModel, IUserService userService)
        {
            _adminViewModel = adminViewModel;
            _userService = userService;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            var users = await _userService.GetAll();
            _adminViewModel.Users.Clear();
            foreach (var user in users)
            {
                if (!user.IsAdmin)
                {
                    _adminViewModel.Users.Add(user);
                }
            }
        }
    }
}
