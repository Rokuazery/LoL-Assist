using LoLA.WebAPIs.DataDragon.Objects;
using System.Collections.Generic;
using static LoLA.Utils.Protocol;
using System.Threading.Tasks;
using LoLA.Utils.Logger;
using System.Linq;
using LoLA.Utils;
using System.IO;
using System;

namespace LoLA.WebAPIs.DataDragon
{
    public static class DataDragonWrapper
    {
        public static List<Perk> perks = new List<Perk>();
        public static Champions Champions = new Champions();
        public static List<string> patches = new List<string>();
        private static readonly string ddragon = $"{HTTP}ddragon.leagueoflegends.com/cdn/";

        public static async Task InitAsync(string patch = "latest")
        {
            LogService.Log(LogService.Model("Downloading prerequisite from DataDragon...", Global.name, LogType.INFO));
            Task<List<Perk>> prks = null;
            Task<Champions> champs = null;
            List<Task> tasks = new List<Task>();

            patches = await WebExt.DlDe<List<string>>("https://ddragon.leagueoflegends.com/api/versions.json");

            if (patch == "latest") Global.Config.dDragonPatch = patches[0];

            var wmChampions = new WebModel() {
                path = $"{Global.libraryFolder}\\{Global.Config.dDragonPatch}_champion.json",
                url = string.Format("{0}{1}/data/en_US/champion.json", ddragon, Global.Config.dDragonPatch)
            };

            var wmPerks = new WebModel(){
                path = $"{Global.libraryFolder}\\{Global.Config.dDragonPatch}_perks.json",
                url = string.Format("{0}{1}/data/en_US/runesReforged.json", ddragon, Global.Config.dDragonPatch)
            };

            tasks.Add(champs = WebExt.DlDeAndSaveToFile<Champions>(wmChampions));
            tasks.Add(prks = WebExt.DlDeAndSaveToFile<List<Perk>>(wmPerks));

            Parallel.ForEach(tasks, async task => { await task; });

            Champions = await champs;
            perks = await prks;
        }

        public static async Task<string> GetChampionImage(string championId, string Patch = Global.defaultPatch)
        {
            string Path;
            WebModel webModel = new WebModel(); 

            string Image = championId + ".png";
            Path = string.Format($"{ChampionFolder(championId)}\\champion_{Image}");

            webModel.url = string.Format("{0}{1}/img/champion/{2}", ddragon, Patch, Image);
            webModel.path = Path;
            return await WebExt.RunDownloadAysnc(webModel);
        }

        public static string ChampionFolder(string championId, string Patch = "")
        {
            try
            {
                var id = Misc.Normalize(championId);
                var mainPath = $"{Global.libraryFolder}\\Champions";

                List<string> pathList = new List<string> { mainPath, $"{mainPath}\\{id}"
                , $"{mainPath}\\{id}\\{Patch}" };

                foreach (var item in pathList)
                    Directory.CreateDirectory(item);

                return string.IsNullOrEmpty(Patch) ? pathList[1] : pathList.Last();
            }
            catch (IOException IoEx)
            {
                LogService.Log(LogService.Model(IoEx.Message, IoEx.Source, LogType.EROR));
                return null;
            }
        }
    }
}
