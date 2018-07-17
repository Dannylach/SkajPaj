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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mvm;

        public MainWindow(string login, string password)
        {
            
            InitializeComponent();
            mvm = new MainViewModel(login, password);
            mvm.RequestClose += new EventHandler(CloseWindow);
            DataContext = mvm;
        }

        public void CloseWindow(Object source, EventArgs args)
        {
            Close();
        }
    }
}
