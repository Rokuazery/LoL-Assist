using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.Windows.Input;
using System.Windows;
using System;

namespace LoL_Assist_WAPP.View
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
        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Decided?.Invoke(false);

        //private void Close()
        //{
        //    Utils.Animation.FadeOut(backDrop, 0.13);
        //    Utils.Animation.Margin(this, ConfigModel.marginOpen, new Thickness(0, 330, 0, 0), 0.13);
        //}
    }
}
