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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        // Using a DependencyProperty as the backing store for MyProperty LoginCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoginCommandProperty =
            DependencyProperty.Register("LoginCommand", typeof(ICommand), typeof(LoginView), new PropertyMetadata(null));

        public ICommand LoginCommand
        {
            get
            {
                return (ICommand)GetValue(LoginCommandProperty);
            }
            set
            {
                SetValue(LoginCommandProperty, value);
            }
        }

        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginButton(object sender, RoutedEventArgs e)
        {
            if(LoginCommand != null)
            {
                string password = passwordpb.Password;
                LoginCommand.Execute(password);
            }
        }
    }
}
