using static LoLA.Networking.WebWrapper.DataDragon.DataDragonWrapper;
using static LoLA.Utils.Logger.LogService;
using LoLA.Networking.LCU.Enums;
using LoLA.Utils.Logger;
using Newtonsoft.Json;
using LoLA.Data.Enums;
using System.IO;
using LoLA.Data;
using System;

namespace LoLA.Networking.WebWrapper.DataProviders.Utils
{
    public static class Helper
    {
        public static bool IsBuildFileValid(string championId, GameMode gameMode, Role role, Provider provider)
        {
            Log("Checking for build file...", LogType.INFO);

            string json;
            string filePath = DataPath(championId, gameMode, role, provider);

            if (!GlobalConfig.s_Caching || !File.Exists(filePath))
                return false;

            using (var stream = new StreamReader(filePath)) { json = stream.ReadToEnd(); }

            if (string.IsNullOrEmpty(json))
                return false;

            try
            {
                var championBuild = JsonConvert.DeserializeObject<ChampionBuild>(json);

                foreach (var rune in championBuild.Runes)
                    if (rune == null) return false;

                foreach (var spell in championBuild.Spells)
                {
                    if (string.IsNullOrEmpty(spell.First)
                    && string.IsNullOrEmpty(spell.Second))
                        return false;
                }
            }
            catch { return false; }

            return true;
        }

        public static string DataPath(string championId, GameMode gameMode, Role role, Provider provider)
        {
            int maxLength = 5;

            string version = GlobalConfig.s_LatestPatch ? s_Patches[0] : s_Patches[1];

            if (version.Length > maxLength)
                version = version.Substring(0, maxLength);

            string dir = Path.Combine(ChampionFolder(championId), version);
            Directory.CreateDirectory(dir);

            if (role != Role.RECOMENDED)
                return Path.Combine(dir, $"{provider}-{championId} {gameMode} {role}.json");
            else
                return Path.Combine(dir, $"{provider}-{championId} {gameMode}.json");
        }
    }
}
