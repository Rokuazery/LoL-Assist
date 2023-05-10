using System.Diagnostics;
using System.Linq;
using System.IO;
using System;
using LoLA.Utils.Logger;

namespace LoLA.Networking.LCU
{
    public static class LeagueClient
    {
        public const string PROCC_NAME = "LeagueClientUx";
        public static string GetLocation()
        {
            var processList = Process.GetProcessesByName(PROCC_NAME).ToList();  
            if (processList != null && processList.Count > 0)
            {
                var procc = processList.FirstOrDefault();
                try
                {
                    string fullPath = procc.MainModule?.FileName;
                    return fullPath == null ? fullPath : Path.GetDirectoryName(fullPath);
                }
                catch (Exception ex)
                {
                    LogService.Log($"Error getting process module: {ex.Message}", LogType.DBUG);
                    return null;
                }
            }
            return null;
        }
    }
}
