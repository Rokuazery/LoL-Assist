using System.Windows.Controls;
using LoL_Assist_WAPP.Models;
using System.Windows.Input;
using System.Windows;
using System;

namespace LoL_Assist_WAPP.Views
{
    /// <summary>
    /// Interaction logic for MsgBox.xaml
    /// </summary>
    public partial class MsgBox : UserControl
    {
        public delegate void DecidedHandler(bool result);
        public event DecidedHandler Decided;
        public MsgBox(string msg, double width = 250, double height = 160)
        {
            InitializeComponent();
            Msg.Text = msg;
            Width = width;
            Height = height;
        }

        private void NoBtn_Click(object sender, RoutedEventArgs e) => Decided?.Invoke(false);
        private void YesBtn_Click(object sender, RoutedEventArgs e) => Decided?.Invoke(true);
        private void CancelBtn_Click(object sender, RoutedEventArgs e) => Decided?.Invoke(false);
    }
}
