using System;
using System.Diagnostics;
using System.Windows.Input;
using LoL_Assist_WAPP.Model;
using System.Windows.Controls;
using LoL_Assist_WAPP.ViewModel;
using System.Windows.Navigation;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for ConfigPanel.xaml
    /// </summary>
    public partial class ConfigPanel : UserControl
    {
        public ConfigPanel()
        {
            InitializeComponent();
            DataContext = new ConfigViewModel();
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            ConfigM.LoadConfig();
            Utils.Animation.FadeOut(this);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
