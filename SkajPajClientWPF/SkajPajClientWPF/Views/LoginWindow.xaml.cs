﻿using SkajPajClientWPF.ViewModels;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        LoginViewModel lw = new LoginViewModel();

        public LoginWindow()
        {
            InitializeComponent();
            lw.RequestClose += new EventHandler(CloseWindow);
            DataContext = lw;
        }

        public void CloseWindow(Object source, EventArgs args)
        {
            Close();
        }
    }
}