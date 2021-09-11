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
        private Border backDrop = new Border();
        public InfoPanel(Border border)
        {
            InitializeComponent();
            backDrop = border;
            Version.Text = $"App Verion {ConfigM.version} | Lib Version {Global.version}";
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) 
        {
            Utils.Animation.FadeOut(backDrop);
            Utils.Animation.Margin(this, ConfigM.marginOpen, ConfigM.marginClose, 0.2);
        }

        private void CreatorLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) => Process.Start(e.Uri.ToString());
        private void TesterLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) => Process.Start(e.Uri.ToString());
    }
}
