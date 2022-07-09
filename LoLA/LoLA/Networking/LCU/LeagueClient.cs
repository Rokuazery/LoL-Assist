using System.Diagnostics;
using System.Linq;
using System.IO;

namespace LoLA.Networking.LCU
{
    public static class LeagueClient
    {
        public static string ProccName = "LeagueClient";
        public static string GetLocation()
        {
            var process = Process.GetProcessesByName(ProccName);
            if (process.Length > 0)
            {
                try
                {
                    string fullPath = process.First()?.MainModule?.FileName;
                    return Path.GetDirectoryName(fullPath);
                }
                catch { return null; }
            }
            return null;
        }
    }
}
