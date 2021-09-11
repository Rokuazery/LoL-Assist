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

        bool IsCheckerBusy = false;
        public async Task Update()
        {
            CheckForUpdatesContent.Text = "Checking for updates...";
            if (!IsCheckerBusy)
            {
                IsCheckerBusy = true;
                try
                {
                    Log.Append("Checking for updates...");
                    WebClient client = new WebClient();
                    var versions = await client.DownloadStringTaskAsync(new Uri("https://raw.githubusercontent.com/Rokuazery/LoL-Assist/master/Version.txt"));
                    string appVersion = Utils.GetLine(versions, 1);
                    string libVersion = Utils.GetLine(versions, 2);

                    Process process = new Process();
                    ProcessStartInfo processInfo = new ProcessStartInfo();
                    processInfo.FileName = "LoLA Updater.exe";
                    processInfo.UseShellExecute = true;

                    bool IsUpdateAvailable = true;

                    if (appVersion != ConfigM.version && libVersion != Global.version)
                        processInfo.Arguments = "updateBoth";
                    else if (appVersion != ConfigM.version)
                        processInfo.Arguments = "updateExec";
                    else if (libVersion != Global.version)
                        processInfo.Arguments = "updateLib";
                    else IsUpdateAvailable = false;

                    if (IsUpdateAvailable)
                    {
                        process.StartInfo = processInfo;
                        process.Start();
                        Environment.Exit(69);
                    }
                }
                catch  { Log.Append("Failed to check for updates."); }

                IsCheckerBusy = false;
                CheckForUpdatesContent.Text = "Check for updates";
            }
        }

        private async void CheckForUpdatesLink_Click(object sender, RoutedEventArgs e) =>  await Update();
    }
}
