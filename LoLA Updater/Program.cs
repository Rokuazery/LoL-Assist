using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;
using System;

namespace LoLA_Updater
{
    class Program
    {
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

                Thread.Sleep(1500); // add a delay to wait for the process to shutdown completely

                if (args[0] == "updateExec")
                    DownloadExec(execVersion);
                else if (args[0] == "updateLib")
                    DownloadLib(libVersion);
                else if (args[0] == "updateBoth")
                {
                    DownloadExec(execVersion);
                    DownloadLib(libVersion);
                }
                else Environment.Exit(69);

                if (Process.GetProcessesByName("LoL Assist").Length < 1)
                    Process.Start("LoL Assist.exe");
            }

            Environment.Exit(69);
            Console.ReadLine();
        }

        public static void DownloadExec(string execVersion)
        {
            try
            {
                WebClient client = new WebClient();

                var fileName = "LoL Assist.exe";

                Console.WriteLine($"Deleteing {fileName}...");
                File.Delete(fileName);
                Console.WriteLine($"Downloading LoL Assist v{execVersion}...");
                client.DownloadFile("https://onedrive.live.com/download?resid=5E12824F9E63EA74%214954&authkey=AJkleVt8SSljXJY", fileName);
                client.Dispose();
                Console.WriteLine("Done.");
            }
            catch
            {
                Console.WriteLine("Process failed.");
            }

        }

        public static void DownloadLib(string libVersion)
        {
            try
            {
                WebClient client = new WebClient();

                var fileName = "LoLA.dll";
                Console.WriteLine($"Deleteing {fileName}...");
                File.Delete(fileName);
                Console.WriteLine($"Downloading LoLA v{libVersion}...");
                client.DownloadFile("https://onedrive.live.com/download?resid=5E12824F9E63EA74%214953&authkey=AKMvepedMnjGt_c", fileName);
                client.Dispose();
                Console.WriteLine("Done.");
            }
            catch
            {
                Console.WriteLine("Process failed.");
            }
        }

        public static string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }
    }
}
