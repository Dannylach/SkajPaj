using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.Objects
{
    public class Call : User
    {
        public Call(string l, string a, string ip, DateTime sd) : base(l, a, ip)
        {
            StartDate = sd;
        }

        private DateTime _startDate;
        private string _isReceived;
        private DateTime _endDate;

        public DateTime StartDate { get => _startDate; set => _startDate = value; }
        public string IsReceived { get => _isReceived; set => _isReceived = value; }
        public DateTime EndDate { get => _endDate; set => _endDate = value; }
        public TimeSpan Time {
            get
            {
                return EndDate.Subtract(StartDate);
            }
        }
        public string StartDateString{
            get
            {
                return _startDate.ToString(new CultureInfo("pl-PL"));
            }
        }
    }
}
