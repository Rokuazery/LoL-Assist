using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.Diagnostics;
using System.Net;
using LoLA.Utils;
using System;
using LoLA;

namespace LoL_Assist_WAPP
{
    public static class Update
    {        
        private static bool IsCheckerBusy = false;
        public static async Task Start()
        {
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
                    IsCheckerBusy = false;
                }
                catch
                {
                    IsCheckerBusy = false;
                    Log.Append("Failed to check for updates.");
                }
            }
        }
    }
}
