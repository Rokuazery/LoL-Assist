using LoLA.Networking.WebWrapper.DataDragon.Data;
using static LoLA.Utils.Logger.LogService;
using LoLA.Networking.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoLA.Networking.Model;
using LoLA.Utils.Logger;
using System.Linq;
using LoLA.Utils;
using System.IO;

namespace LoLA.Networking.WebWrapper.DataDragon
{
    public class DataDragonWrapper
    {
        public static List<Perk> s_Perks = new List<Perk>();
        public static Champions s_Champions = new Champions();
        public static List<string> s_Versions = new List<string>();
        private static readonly string r_dataDragonUrl = $"{Protocol.HTTP}ddragon.leagueoflegends.com/cdn/";

        public static async Task InitAsync(string patch = "latest")
        {
            Log("Downloading prerequisite from DataDragon...", LogType.INFO);
            Task<List<Perk>> perkTasks = null;
            Task<Champions> championTasks = null;
            List<Task> tasks = new List<Task>();

            s_Versions = await WebEx.DlDe<List<string>>("https://ddragon.leagueoflegends.com/api/versions.json");

            GlobalConfig.s_DataDragonPatch = patch == "latest" ?  s_Versions[0] : patch;

            var championsWebModel = new WebModel()
            {
                Path = $"{LibInfo.r_LibFolderPath}\\{GlobalConfig.s_DataDragonPatch}_champion.json",
                Url = $"{r_dataDragonUrl}{GlobalConfig.s_DataDragonPatch}/data/en_US/champion.json"
            };

            var perksWebModel = new WebModel()
            {
                Path = $"{LibInfo.r_LibFolderPath}\\{GlobalConfig.s_DataDragonPatch}_perks.json",
                Url = $"{r_dataDragonUrl}{GlobalConfig.s_DataDragonPatch}/data/en_US/runesReforged.json"
            };

            tasks.Add(championTasks = WebEx.DlDeAndSaveToFile<Champions>(championsWebModel));
            tasks.Add(perkTasks = WebEx.DlDeAndSaveToFile<List<Perk>>(perksWebModel));

            Parallel.ForEach(tasks, async task => { await task; });

            s_Champions = await championTasks;
            s_Perks = await perkTasks;
        }

        public static async Task<string> GetChampionImage(string championId)
        {
            string path;
            WebModel webModel = new WebModel();

            string imageName = championId + ".png";
            path = string.Format($"{ChampionFolder(championId)}\\champion_{imageName}");

            webModel.Url = $"{r_dataDragonUrl}{GlobalConfig.s_DataDragonPatch}/img/champion/{imageName}";
            webModel.Path = path;
            return await WebEx.RunDownloadAysnc(webModel);
        }

        public static string ChampionFolder(string championId, string Patch = "")
        {
            try
            {
                var id = Misc.Normalize(championId);
                var mainPath = $"{LibInfo.r_LibFolderPath}\\Champions";

                List<string> pathList = new List<string> { mainPath, $"{mainPath}\\{id}"
                , $"{mainPath}\\{id}\\{Patch}" };

                foreach (var item in pathList)
                    Directory.CreateDirectory(item);

                return string.IsNullOrEmpty(Patch) ? pathList[1] : pathList.Last();
            }
            catch (IOException IoEx)
            {
                Log(IoEx.Source, IoEx.Message, LogType.EROR);
                return null;
            }
        }
    }
}
