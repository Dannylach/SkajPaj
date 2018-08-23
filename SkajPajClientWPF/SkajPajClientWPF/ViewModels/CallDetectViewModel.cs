using SkajPajClientWPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF.ViewModels
{
    public class CallDetectViewModel : ObservableObject
    {
        public ObservableCollection<Log> _logs;
        public ObservableCollection<Log> Logs { get => _logs;
            set {
                _logs = value;
                OnPropertyChanged();
            }
        }

        public List<Log> XDSLKFJLKS;
        public CallDetectViewModel()
        {
            ObservableCollection<Log> tmp = new ObservableCollection<Log>();
            tmp.Add(new Log("test", "test"));
            Logs = tmp;
        }
    }

    public class Log : ObservableObject
    {
        private DateTime time;
        private string type;
        private string content;

        public DateTime Time { get => time; set { time = value; OnPropertyChanged();} }
        public string Type { get => type; set { type = value; OnPropertyChanged(); } }
        public string Content { get => content; set { content = value; OnPropertyChanged(); } }

        public Log(string type, string content)
        {
            Time = DateTime.Now;
            this.Type = type;
            this.Content = content;
        }
    }
}
