using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.Diagnostics;
using System.Threading;
using LoLA.Utils.Log;
using System.Net;
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
                    await Task.Run(() => {
                        Utils.Log("Checking for updates...", LogType.INFO);
                        WebClient client = new WebClient();
                        var versions = client.DownloadString(new Uri("https://raw.githubusercontent.com/Rokuazery/LoL-Assist/master/Version.txt"));
                        string appVersion = Utils.GetLine(versions, 1);
                        string libVersion = Utils.GetLine(versions, 2);

                        Process process = new Process();
                        ProcessStartInfo processInfo = new ProcessStartInfo {
                            FileName = "LoLA Updater.exe",
                            UseShellExecute = true
                        };

                        bool IsUpdateAvailable = true;

                        if (appVersion != ConfigModel.version && libVersion != Global.version)
                        {
                            Utils.Log($"Newer version of 'LoL Assist v{appVersion}' & 'LoLA.dll v{libVersion}' is available", LogType.INFO);
                            processInfo.Arguments = "updateBoth";
                        }
                        else if (appVersion != ConfigModel.version)
                        {
                            Utils.Log($"Newer version of 'LoL Assist v{appVersion}' is available", LogType.INFO);
                            processInfo.Arguments = "updateExec";
                        }
                        else if (libVersion != Global.version)
                        {
                            Utils.Log($"Newer version of 'LoLA.dll v{libVersion}' is available", LogType.INFO);
                            processInfo.Arguments = "updateLib";
                        }
                        else
                        {
                            Utils.Log("No updates available", LogType.INFO);
                            IsUpdateAvailable = false;
                        }

                        if (IsUpdateAvailable)
                        {
                            Utils.Log("Updating...", LogType.INFO);
                            process.StartInfo = processInfo;
                            process.Start();
                            Environment.Exit(69);
                        }
                    });
                    Thread.Sleep(1000);
                    IsCheckerBusy = false;
                }
                catch
                {
                    Utils.Log("Failed to check for updates", LogType.EROR);
                    IsCheckerBusy = false;
                }
            }
        }
    }
}
