using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< Updated upstream
=======
using System.Windows;
using System.Windows.Input;
using ExpertSystem.Domain.Services;
using ExpertSystem.WPF.Commands;
using ExpertSystem.WPF.State.Authenticators;
>>>>>>> Stashed changes

namespace ExpertSystem.WPF.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
<<<<<<< Updated upstream
=======
        private readonly IAuthenticator _authenticator;
        private readonly IDatasetService _datasetService;
        private readonly IFileDialogService _fileDialogService;
        private string _selectedFilePath;
        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            private set
            {
                _nickname = value;
                OnPropertyChanged(nameof(Nickname));
            }
        }


        public HomeViewModel(IAuthenticator authenticator, IDatasetService datasetService, IFileDialogService fileDialogService)
        {
            _authenticator = authenticator;
            _datasetService = datasetService;
            _fileDialogService = fileDialogService;
            AddDatabaseCommand = new AddDatabaseCommand(this, fileDialogService, datasetService, authenticator);
            _authenticator.StateChanged += () => CommandManager.InvalidateRequerySuggested();
            Nickname = _authenticator.CurrentUser.Nickname;
        }

        public ICommand AddDatabaseCommand { get; }

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
>>>>>>> Stashed changes
    }
}
