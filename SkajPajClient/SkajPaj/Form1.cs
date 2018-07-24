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
        private readonly string path = Application.StartupPath + "\\buffer.wav";
        private readonly AudioManager audioManager = new AudioManager();
        private readonly ConnectionManager connectionManager = new ConnectionManager();
        private readonly DataPacket dataPacket = new DataPacket();
        private string userName = "Daniel";

        public Form1()
        {
            InitializeComponent();
            connectionManager.Initialize(userName);
        }

        private void RecordBtn_Click(object sender, EventArgs e)
        {
            audioManager.StartRecording(connectionManager, userName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            connectionManager.BeginCall("192.168.1.33");
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            audioManager.EndCall();
            connectionManager.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            connectionManager.ListenForMessage();
        }
    }
}
