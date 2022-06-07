using System.Diagnostics;
using LoLA.Utils.Logger;
using System.Linq;
using System.IO;

namespace LoLA.LCU
{
    public static class LeagueClient
    {
        public static string ProccName = "LeagueClient";
        public static string GetLocation()
        {
            var process = Process.GetProcessesByName(ProccName);
            if (process.Length > 0)
            {
                string fullPath = process.First()?.MainModule?.FileName;
                return Path.GetDirectoryName(fullPath);
            }
            else { return null; }
        }
    }
}
