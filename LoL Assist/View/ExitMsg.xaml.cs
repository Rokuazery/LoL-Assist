using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.Windows.Input;
using System.Windows;
using System;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for ExitMsg.xaml
    /// </summary>
    public partial class ExitMsg : UserControl
    {
        private Border backDrop = new Border();
        public ExitMsg(Border border)
        {
            backDrop = border;
            InitializeComponent();
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Close();
        private void NoBtn_Click(object sender, RoutedEventArgs e) => Close();

        private void Close()
        {
            Utils.Animation.FadeOut(backDrop, 0.15);
            Utils.Animation.Margin(this, ConfigM.marginOpen, new Thickness(0, 330, 0, 0), 0.15);
        }

        private void YesBtn_Click(object sender, RoutedEventArgs e) => Environment.Exit(69);

    }
}
