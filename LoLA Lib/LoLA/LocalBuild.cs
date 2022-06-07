using static LoLA.WebAPIs.DataDragon.DataDragonWrapper;
using System.Collections.Generic;
using LoLA.LCU.Objects;
using Newtonsoft.Json;
using LoLA.Objects;
using System.Linq;
using LoLA.Utils;
using System.IO;
using LoLA.LCU;
using System;

namespace LoLA
{
    public class LocalBuild
    {
        public ChampionBD FetchData(string championId, string fileName, GameMode gameMode)
        {
            string jsonContent = string.Empty;

            var filePath = DataPath(championId, fileName, gameMode);
            using (var stream = new StreamReader(filePath))
            {
                jsonContent = stream.ReadToEnd();
            }

            var championBuild = JsonConvert.DeserializeObject<ChampionBD>(jsonContent);
            return championBuild;
        }

        public void DeleteData(string championId, string fileName, GameMode gameMode)
        {
            fileName = DataPath(championId, fileName, gameMode);
            File.Delete(fileName);
        }

        public List<string> GetBuildFiles(string championId, GameMode gameMode)
        {
            List<string> jsons = new List<string>();
            var buildsFolder = BuildsFolder(championId, gameMode);
            foreach (string filePath in Directory.GetFiles(buildsFolder, "*.json"))
                jsons.Add(filePath);
            return jsons;
        }

        public string DataPath(string championId,string fileName, GameMode gameMode) 
        {
            return Path.Combine(BuildsFolder(championId, gameMode), $"{fileName}.json");
        }

        public string BuildsFolder(string championId, GameMode gameMode)
        {
            var id = Misc.Normalize(championId);
            var mainPath = $"{Global.libraryFolder}\\Custom Builds";

            List<string> pathList = new List<string> { mainPath, $"{mainPath}\\{id}"
            , $"{mainPath}\\{id}\\Builds", $"{mainPath}\\{id}\\Builds\\{gameMode}" };

            foreach (var item in pathList)
                Directory.CreateDirectory(item);

            return pathList.Last();
        }

        public string BuildsFolder(string championId)
        {
            var id = Misc.Normalize(championId);
            var mainPath = $"{Global.libraryFolder}\\Custom Builds";

            List<string> pathList = new List<string> { mainPath, $"{mainPath}\\{id}"
            , $"{mainPath}\\{id}\\Builds" };

            foreach (var item in pathList)
                Directory.CreateDirectory(item);

            return pathList.Last();
        }
    }
}
