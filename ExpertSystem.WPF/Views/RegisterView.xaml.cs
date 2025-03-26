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
        }

        private void RegisterButton(object sender, RoutedEventArgs e)
        {
            if (RegisterCommand != null)
            {
                var passwordData = (passwordpb.Password, password2pb.Password);
                RegisterCommand.Execute(passwordData);
            }
        }
    }
}
