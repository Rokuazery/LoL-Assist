using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.Windows.Input;
using System.Diagnostics;
using System;
using LoLA;

namespace LoL_Assist_WAPP.View
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
            Version.Text = $"App Verion {ConfigModel.version} | Lib Version {Global.version}";
        }

        private void CreatorLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) => Process.Start(e.Uri.ToString());
        private void TesterLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) => Process.Start(e.Uri.ToString());
        private void Github_Click(object sender, System.Windows.RoutedEventArgs e) => Process.Start("https://github.com/Rokuazery/LoL-Assist");

        private void BackBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Utils.Animation.FadeOut(backDrop);
            Utils.Animation.Margin(this, ConfigModel.marginOpen, ConfigModel.marginClose, 0.2);
        }
    }
}
