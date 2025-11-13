using ExpertSystem.WPF.Commands;
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
    /// Logika interakcji dla klasy RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public static readonly DependencyProperty RegisterCommandProperty =
            DependencyProperty.Register("RegisterCommand", typeof(ICommand), typeof(RegisterView), new PropertyMetadata(null));

        public ICommand RegisterCommand
        {
            get
            {
                return (ICommand)GetValue(RegisterCommandProperty);
            }
            set
            {
                SetValue(RegisterCommandProperty, value);
            }
        }

        public RegisterView()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.StateChanged += OnWindowStateChanged;
                UpdateMaximizeIcon(window.WindowState);
            }
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            var window = sender as Window;
            if (window != null)
            {
                UpdateMaximizeIcon(window.WindowState);
            }
        }

        private void UpdateMaximizeIcon(WindowState state)
        {
            MaximizeButtonn.Content = state == WindowState.Maximized ? "❐" : "▢";
        }

        private void RegisterButton(object sender, RoutedEventArgs e)
        {
            if (RegisterCommand != null)
            {
                var passwordData = (passwordpb.Password, password2pb.Password);
                RegisterCommand.Execute(passwordData);
            }
        }


        private void DragAreaMouse(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Window.GetWindow(this)?.DragMove();
            }
        }

        private void MinimizeButton(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void MaximizeButton(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void CloseButton(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }
    }
}
