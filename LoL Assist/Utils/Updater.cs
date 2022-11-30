using System.Threading.Tasks;
using LoL_Assist_WAPP.Models;
using System.Diagnostics;
using LoLA.Utils.Logger;
using System.Net;
using System;
using LoLA;

namespace LoL_Assist_WAPP.Utils
{
    public static class Updater
    {
        private static bool s_isCheckerBusy = false;
        public static async Task Start()
        {
            if (!s_isCheckerBusy)
            {
                s_isCheckerBusy = true;
                try
                {
                    await Task.Run(() => {
                        Helper.Log("Checking for updates...", LogType.INFO);
                        WebClient client = new WebClient();
                        var versions = client.DownloadString(new Uri("https://raw.githubusercontent.com/Rokuazery/LoL-Assist/master/Version.txt"));
                        string appVersion = Helper.GetLine(versions, 1);
                        string libVersion = Helper.GetLine(versions, 2);

                        Process process = new Process();
                        ProcessStartInfo processInfo = new ProcessStartInfo
                        {
                            FileName = "LoLA Updater.exe",
                            UseShellExecute = true
                        };

                        bool isUpdateAvailable = true;

                        if (appVersion != ConfigModel.r_Version && libVersion != LibInfo.r_Version)
                        {
                            Helper.Log($"Newer version of 'LoL Assist v{appVersion}' & 'LoLA.dll v{libVersion}' is available", LogType.INFO);
                            processInfo.Arguments = "updateBoth";
                        }
                        else if (appVersion != ConfigModel.r_Version)
                        {
                            Helper.Log($"Newer version of 'LoL Assist v{appVersion}' is available", LogType.INFO);
                            processInfo.Arguments = "updateExec";
                        }
                        else if (libVersion != LibInfo.r_Version)
                        {
                            Helper.Log($"Newer version of 'LoLA.dll v{libVersion}' is available", LogType.INFO);
                            processInfo.Arguments = "updateLib";
                        }
                        else
                        {
                            Helper.Log("No updates available", LogType.INFO);
                            isUpdateAvailable = false;
                        }

                        if (isUpdateAvailable)
                        {
                            Helper.Log("Updating...", LogType.INFO);
                            process.StartInfo = processInfo;
                            process.Start();
                            Environment.Exit(69);
                        }
                    });
                    s_isCheckerBusy = false;
                }
                catch
                {
                    Helper.Log("Failed to check for updates", LogType.EROR);
                    s_isCheckerBusy = false;
                }
            }
        }
    }
}
