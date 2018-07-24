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
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        int time = 0;

        public CallWindow(string login, string password, string friendAvatar, string friendLogin, string address_ip, string call_id)
        {
            InitializeComponent();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            time++;
            Timer.Text = "Czas: " + timeToString();
        }

        private string timeToString()
        {
            int secunds = time % 60;
            int minutes = time / 60;
            if(minutes < 1)
            {
                return secunds.ToString() + " sekund";
            }
            return minutes.ToString() + " minut " + secunds.ToString() + " sekund";
        }
    }
}
