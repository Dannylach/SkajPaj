﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SkajPajClientWPF
{
    class MusicPlayer
    {
        [DllImport("Winmm.dll")]
        private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, int hwndCallback);

        public void open(string file)
        {
            string command = "open \"" + file + "\" type MPEGVideo alias MyMp3";
            mciSendString(command, null, 0, 0);
        }

        public void play()
        {
            string command = "play MyMp3";
            mciSendString(command, null, 0, 0);
        }

        public void stop()
        {
            string command = "stop MyMp3";
            mciSendString(command, null, 0, 0);

            command = "close MyMp3";
            mciSendString(command, null, 0, 0);
        }
    }
}
