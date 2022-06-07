using LoLA.WebAPIs.DataDragon;
using System.Threading.Tasks;
using LoLA.WebAPIs.Metasrc;
using LoLA.Utils.Logger;
using LoLA.Objects;
using LoLA.Enums;
using System.IO;
using LoLA.LCU;

namespace LoLA
{
    public class Main
    {
        public static LocalBuild localBuild = new LocalBuild();
        public static MetasrcWrapper Metasrc = new MetasrcWrapper();
        public static async Task<bool> Init()
        {
            try
            {
                CreateLibFolder();
                await Task.Run(() => { Metasrc.metaClass.Fetch(); });
                await Dictionaries.DcsInitAsync();
                await DataDragonWrapper.InitAsync();
                //EdgeWrapper.InitEdge();
                LogService.Log(LogService.Model("LoLA has been initialized!", Global.name, LogType.INFO));
                return true;
            }
            catch
            {
                LogService.Log(LogService.Model("Failed to initialize", Global.name, LogType.EROR));
                return false;
            }
        }

        public static async Task<ChampionBD> RequestBuildsData(string ChampionId, GameMode gameMode, BuildsProvider provider, string role = "", string fileName = null)
        {
            ChampionBD championBD = new ChampionBD(); 
            switch (provider)
            {
                case BuildsProvider.Local:
                    championBD = localBuild.FetchData(ChampionId, fileName, gameMode);
                    break;
                case BuildsProvider.Metasrc:
                    championBD = await Metasrc.FetchDataAsync(ChampionId, gameMode, role);
                    break;
                case BuildsProvider.Mobafire:
                    break;
                case BuildsProvider.LeagueSpy:
                    break;
            }
            return championBD;
        }

        public static void CreateLibFolder()
        {
            LogService.Log(LogService.Model($"Creating {Global.name} folder...", Global.name, LogType.INFO));
            Directory.CreateDirectory(Global.libraryFolder);
        }
    }
}