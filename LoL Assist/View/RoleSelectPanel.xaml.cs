﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for RoleSelectPanel.xaml
    /// </summary>
    public partial class RoleSelectPanel : UserControl
    {
        public RoleSelectPanel()
        {
            InitializeComponent();
        }

        private void Role_Click(object sender, RoutedEventArgs e) => Utils.Animation.FadeOut(this);
    }
}
