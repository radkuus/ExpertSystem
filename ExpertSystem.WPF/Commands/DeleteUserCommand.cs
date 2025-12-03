using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExpertSystem.WPF.Commands
{
    class DeleteUserCommand : BaseAsyncCommand
    {
        private readonly AdminViewModel _adminViewModel;
        private readonly IUserService _userService;
        private readonly IDatasetService _datasetService;

        public DeleteUserCommand(AdminViewModel adminViewModel, IUserService userService, IDatasetService datasetService)
        {
            _adminViewModel = adminViewModel;
            _userService = userService;
            _datasetService = datasetService;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            try
            {
                //User deletion
                if (_adminViewModel.SelectedUser == null)
                    return;

                int userId = _adminViewModel.SelectedUser.Id;
                string nickname = _adminViewModel.SelectedUser.Nickname;

                await _userService.Delete(userId);
                _adminViewModel.Users.Remove(_adminViewModel.SelectedUser);

                // Users datasets deletion
                var datasets = await _datasetService.GetUserDatasets(userId);
                foreach (var dataset in datasets)
                {
                    await _datasetService.RemoveDataset(dataset.Id);
                }
                string folderPath = Path.Combine(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName,
                    "Datasets",
                    nickname);

                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, recursive: true);
                }
                _adminViewModel.ErrorMessage = "";
                _adminViewModel.SelectedUser = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return (_adminViewModel.SelectedUser != null && _adminViewModel.SelectedUser.IsAdmin == false);
        }
    }
}
