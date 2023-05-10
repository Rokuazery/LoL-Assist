using System.Collections.Generic;
using LoLA.Networking.LCU.Enums;
using Newtonsoft.Json;
using System.Linq;
using LoLA.Utils;
using LoLA.Data;
using System.IO;

namespace LoLA.DataProviders
{
    public static class LocalBuild
    {
        public static ChampionBuild FetchData(string championId, string fileName, GameMode gameMode)
        {
            string jsonContent = string.Empty;

            var filePath = DataPath(championId, fileName, gameMode);
            using (var stream = new StreamReader(filePath))
            {
                jsonContent = stream.ReadToEnd();
            }

            var championBuild = JsonConvert.DeserializeObject<ChampionBuild>(jsonContent);
            return championBuild;
        }

        public static void DeleteData(string championId, string fileName, GameMode gameMode)
        {
            fileName = DataPath(championId, fileName, gameMode);
            File.Delete(fileName);
        }

        public static List<string> GetBuildFiles(string championId, GameMode gameMode)
        {
            List<string> jsons = new List<string>();
            var buildsFolder = BuildsFolder(championId, gameMode);
            foreach (string filePath in Directory.GetFiles(buildsFolder, "*.json"))
                jsons.Add(filePath);
            return jsons;
        }

        public static string DataPath(string championId, string fileName, GameMode gameMode)
            => Path.Combine(BuildsFolder(championId, gameMode), $"{fileName}.json");

        public static string BuildsFolder(string championId, GameMode gameMode)
        {
            var id = Misc.Normalize(championId);
            var mainPath = $"{LibInfo.r_LibFolderPath}\\Custom Builds";

            List<string> pathList = new List<string> { mainPath, $"{mainPath}\\{id}"
            , $"{mainPath}\\{id}\\Builds", $"{mainPath}\\{id}\\Builds\\{gameMode}" };

            foreach (var item in pathList)
                Directory.CreateDirectory(item);

            return pathList.Last();
        }

        public static string GetLocalBuildName(string championId, GameMode gameMode)
        {
            var defaultBuildConfig = new DefaultBuildConfig();
            if (File.Exists(DefaultConfigPath(championId)))
                defaultBuildConfig = GetDefaultBuildConfig(championId);

            var buildName = defaultBuildConfig.GetDefaultConfig(gameMode);

            var customBuildPath = BuildsFolder(championId, gameMode);
            var fullPath = $"{customBuildPath}\\{defaultBuildConfig.GetDefaultConfig(gameMode)}";

            if (!File.Exists(fullPath))
                defaultBuildConfig.ResetDefaultConfig(gameMode);

            WriteDefaultBuildConfig(championId, defaultBuildConfig);
            return buildName;
        }

        public static string DefaultConfigPath(string championId)
            => BuildsFolder(championId) + "\\DefaultConfig.json";

        public static void WriteDefaultBuildConfig(string championId, DefaultBuildConfig config)
        {
            using (var streamWriter = new StreamWriter(DefaultConfigPath(championId)))
            {
                var defConfig = JsonConvert.SerializeObject(config);
                streamWriter.Write(defConfig);
            }
        }

        public static DefaultBuildConfig GetDefaultBuildConfig(string championId)
        {
            var defaultConfig = new DefaultBuildConfig();
            using (var streamReader = new StreamReader(DefaultConfigPath(championId)))
            {
                var json = streamReader.ReadToEnd();
                defaultConfig = JsonConvert.DeserializeObject<DefaultBuildConfig>(json);
            }
            return defaultConfig;
        }

        public static string BuildsFolder(string championId)
        {
            var id = Misc.Normalize(championId);
            var mainPath = $"{LibInfo.r_LibFolderPath}\\Custom Builds";

            List<string> pathList = new List<string> { mainPath, $"{mainPath}\\{id}"
            , $"{mainPath}\\{id}\\Builds" };

            foreach (var item in pathList)
                Directory.CreateDirectory(item);

            return pathList.Last();
        }
    }
}
