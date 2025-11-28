using ExpertSystem.Domain.Models;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.State.Authenticators;
using ExpertSystem.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ExpertSystem.WPF.Commands
{
    public class RemoveDatasetCommand : ICommand
    {
        private readonly IAuthenticator _authenticator;
        private readonly IDatasetService _datasetService;
        private readonly HomeViewModel _viewModel;

        public RemoveDatasetCommand(HomeViewModel viewModel, IAuthenticator authenticator, IDatasetService datasetService)
        {
            _authenticator = authenticator;
            _datasetService = datasetService;
            _viewModel = viewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return parameter is Dataset;
        }

        public async void Execute(object? parameter)
        {
            try
            {
                if (parameter is Dataset dataset)
                {
                    await _datasetService.RemoveDataset(dataset.Id);
                    _viewModel.UserDatasets.Remove(dataset);

                    string filePath = Path.Combine(
                        Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName,
                        "Datasets",
                        _authenticator.CurrentUser.Nickname,
                        dataset.Name);

                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
