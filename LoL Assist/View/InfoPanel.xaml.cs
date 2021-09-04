using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Controls;
using System.Diagnostics;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for InfoPanel.xaml
    /// </summary>
    public partial class InfoPanel : UserControl
    {
        private string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public InfoPanel()
        {
            InitializeComponent();
            Version.Text = $"Version {version} - Windows App Edition";
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Utils.Animation.FadeOut(this);

        private void CreatorLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) => Process.Start(e.Uri.ToString());
        private void TesterLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) => Process.Start(e.Uri.ToString());
    }
}
