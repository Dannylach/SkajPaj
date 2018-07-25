using SkajPajClientWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.ViewModels
{
    public class CallViewModel : ObservableObject
    {
        private CallModel _callModel;
        public CallModel CallModel
        {
            get { return _callModel; }
            set { _callModel = value; }
        }

        public CallViewModel()
        {

        }

        public CallViewModel(string login, string password, string friendAvatar, string friendLogin, string address_ip, string call_id, string state)
        {
            CallModel = new CallModel(login, password, friendAvatar, friendLogin, address_ip, call_id, state);
        }

        public event EventHandler RequestClose;

        private void CloseWindow()
        {
            EventHandler handler = this.RequestClose;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
