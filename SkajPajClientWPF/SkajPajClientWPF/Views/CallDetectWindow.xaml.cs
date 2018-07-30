﻿using SkajPajClientWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CallDetectWindow.xaml
    /// </summary>
    public partial class CallDetectWindow : Window
    {
        public CallDetectViewModel cdvm;

        public CallDetectWindow()
        {
            InitializeComponent();

            cdvm = new CallDetectViewModel();
            DataContext = cdvm;
        }
    }
}
