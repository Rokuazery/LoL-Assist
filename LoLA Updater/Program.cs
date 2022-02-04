using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;
using System;

namespace LoLA_Updater
{
    class Program
    {
        const string Lib = "LolA.dll";
        const string Exe = "LoL Assist.exe";
        const string urlLib = "https://onedrive.live.com/download?resid=5E12824F9E63EA74%214953&authkey=AKMvepedMnjGt_c";
        const string urlExe = "https://onedrive.live.com/download?resid=5E12824F9E63EA74%214954&authkey=AJkleVt8SSljXJY";
        static void Main(string[] args)
        {
            Console.Title = "LoLA Updater";
            Console.ForegroundColor = ConsoleColor.Yellow;
            if(args.Length > 0)
            {
                WebClient client = new WebClient();
                string latestVersions = client.DownloadString("https://raw.githubusercontent.com/Rokuazery/LoL-Assist/master/Version.txt");
                string execVersion = GetLine(latestVersions, 1);
                string libVersion = GetLine(latestVersions, 2);

                foreach (var process in Process.GetProcessesByName("LoL Assist"))
                    process.Kill();

                Thread.Sleep(1300); // add a delay to wait for the process to shutdown completely

                if (args[0] == "updateExec")
                    DownloadFunction(Exe, execVersion, urlExe);
                else if (args[0] == "updateLib")
                    DownloadFunction(Lib, libVersion, urlLib);
                else if (args[0] == "updateBoth")
                {
                    DownloadFunction(Lib, libVersion, urlLib);
                    DownloadFunction(Exe, execVersion, urlExe);
                }
                else Environment.Exit(69);

                Console.WriteLine("Launching LoL Assist...", Console.ForegroundColor = ConsoleColor.Green);
                if (Process.GetProcessesByName("LoL Assist").Length < 1)
                    Process.Start("LoL Assist.exe");
            }

            Environment.Exit(69);
            Console.ReadLine();
        }

        public static void DownloadFunction(string fileName, string version, string url)
        {
            try
            {
                WebClient client = new WebClient();
                Console.WriteLine($"Deleteing {fileName}...");
                File.Delete(fileName);
                Console.WriteLine($"Downloading {fileName} v{version}...");
                client.DownloadFile(url, fileName);
                client.Dispose();
                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to update {fileName}. {ex.Message}");
            }
        }

        public static string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }
    }
}
