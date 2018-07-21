using SkajPajClientWPF.ViewModels;
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
using System.Windows.Shapes;

namespace SkajPajClientWPF.Views
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        ChangePasswordViewModel cpw;

        public ChangePasswordWindow(string login)
        {
            InitializeComponent();
            cpw = new ChangePasswordViewModel(login);
            cpw.RequestClose += new EventHandler(CloseWindow);
            DataContext = cpw;
        }

        public void CloseWindow(Object source, EventArgs args)
        {
            Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).SecurePassword = ((PasswordBox)sender).SecurePassword; }
        }

        private void PasswordBox_PasswordChanged2(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).SecurePassword2 = ((PasswordBox)sender).SecurePassword; }
        }
    }
}
