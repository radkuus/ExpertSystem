using ExpertSystem.WPF.ViewModels;
using SimpleTrader.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExpertSystem.WPF.Views
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        public static readonly DependencyProperty RegisterCommandProperty =
                    DependencyProperty.Register("RegisterCommand", typeof(BaseAsyncCommand), typeof(AdminView), new PropertyMetadata(null));

        public BaseAsyncCommand RegisterCommand
        {
            get
            {
                return (BaseAsyncCommand)GetValue(RegisterCommandProperty);
            }
            set
            {
                SetValue(RegisterCommandProperty, value);
            }
        }


        public static readonly DependencyProperty DisplayUsersCommandProperty =
            DependencyProperty.Register("DisplayUsersCommand", typeof(BaseAsyncCommand), typeof(AdminView), new PropertyMetadata(null));

        public BaseAsyncCommand DisplayUsersCommand
        {
            get
            {
                return (BaseAsyncCommand)GetValue(DisplayUsersCommandProperty);
            }
            set
            {
                SetValue(DisplayUsersCommandProperty, value);
            }
        }

        public static readonly DependencyProperty EditUserCommandProperty =
    DependencyProperty.Register("EditUserCommand", typeof(BaseAsyncCommand), typeof(AdminView), new PropertyMetadata(null));

        public BaseAsyncCommand EditUserCommand
        {
            get
            {
                return (BaseAsyncCommand)GetValue(EditUserCommandProperty);
            }
            set
            {
                SetValue(EditUserCommandProperty, value);
            }
        }

        public AdminView()
        {
            InitializeComponent();
        }

        private async void RegisterButton(object sender, RoutedEventArgs e)
        {
            if (RegisterCommand != null)
            {
                var passwordData = (passwordpb.Password, password2pb.Password);
                try
                {
                    await RegisterCommand.ExecuteAsync(passwordData);
                    await DisplayUsersCommand.ExecuteAsync(null);
                }
                finally
                {
                    passwordpb.Clear();
                    password2pb.Clear();
                }
            }
        }

        private async void EditButton(object sender, RoutedEventArgs e)
        {
            if (EditUserCommand != null)
            {
                var passwordData = Tuple.Create(passwordpb.Password, password2pb.Password);

                try
                {
                    await EditUserCommand.ExecuteAsync(passwordData);
                    await DisplayUsersCommand.ExecuteAsync(null);
                }
                finally
                {
                    passwordpb.Clear();
                    password2pb.Clear();
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                passwordpb.Password = string.Empty;
                password2pb.Password = string.Empty;
            }
        }

    }
}

