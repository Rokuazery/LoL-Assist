using System.Diagnostics;
using System.Linq;
using System.IO;
using System;

namespace LoLA.Networking.LCU
{
    public static class LeagueClient
    {
        public const string PROCC_NAME = "LeagueClient";
        public static string GetLocation()
        {
            var process = Process.GetProcessesByName(PROCC_NAME);
            if (process.Length > 0)
            {
                try
                {
                    var fullPath = process?.First()?.MainModule?.FileName;
                    return Path.GetDirectoryName(fullPath);
                } catch(NullReferenceException) {  }
            }
            return null;
        }
    }
}
