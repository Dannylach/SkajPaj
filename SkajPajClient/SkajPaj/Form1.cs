using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace SkajPaj
{
    public partial class Form1 : Form
    {
        private readonly AudioManager audioManager = new AudioManager();
        private readonly ConnectionManager connectionManager = new ConnectionManager();
        private string userName = "łukasz";
        private string ip = "192.168.43.227";

        public Form1()
        {
            InitializeComponent();
            audioManager.Initialize(userName);
        }

        private void RecordBtn_Click(object sender, EventArgs e)
        {
            audioManager.StartCall(ip);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //connectionManager.Initialize(userName);
            audioManager.BeginCall(ip);
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            audioManager.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            audioManager.ListenForMessage();
        }
    }
}
