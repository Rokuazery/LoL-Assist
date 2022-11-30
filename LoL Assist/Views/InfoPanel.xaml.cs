using System.Windows.Controls;
using LoL_Assist_WAPP.Models;
using System.Diagnostics;
using System;
using LoLA;

namespace LoL_Assist_WAPP.Views
{
    /// <summary>
    /// Interaction logic for InfoPanel.xaml
    /// </summary>
    public partial class InfoPanel : UserControl
    {
        private readonly Border backDrop = new Border();
        public InfoPanel(Border border)
        {
            InitializeComponent();
            backDrop = border;
            Version.Text = $"App Verion {ConfigModel.r_Version} | Lib Version {LibInfo.r_Version}";
        }

        private void CreatorLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) => Process.Start(e.Uri.ToString());
        private void TesterLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) => Process.Start(e.Uri.ToString());
        private void Github_Click(object sender, System.Windows.RoutedEventArgs e) => Process.Start("https://github.com/Rokuazery/LoL-Assist");

        private void BackBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Utils.Animation.FadeOut(backDrop);
            Utils.Animation.Margin(this, ConfigModel.r_MarginOpen, ConfigModel.r_MarginClose, 0.2);
        }
    }
}
