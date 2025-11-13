using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExpertSystem.WPF.Controls
{
    public partial class NavigationBar : UserControl
    {
        public NavigationBar()
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

        private void DragAreaMouse(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this)?.DragMove();
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