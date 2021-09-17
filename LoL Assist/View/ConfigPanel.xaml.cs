using LoL_Assist_WAPP.ViewModel;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.Net;
using LoLA.Utils;
using System;
using LoLA;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for ConfigPanel.xaml
    /// </summary>
    public partial class ConfigPanel : UserControl
    {
        private Border backDrop = new Border();
        public ConfigPanel(Border border)
        {
            InitializeComponent();
            backDrop = border;
            DataContext = new ConfigViewModel();
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            ConfigM.LoadConfig();
            Utils.Animation.FadeOut(backDrop);
            Utils.Animation.Margin(this, ConfigM.marginOpen, ConfigM.marginClose);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private async void CheckForUpdatesLink_Click(object sender, RoutedEventArgs e) 
        {
            CheckForUpdatesContent.Text = "Checking for updates...";
            await Update.Start();
            CheckForUpdatesContent.Text = "Check for updates";
        }
    }
}
