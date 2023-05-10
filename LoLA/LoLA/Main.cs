using LoLA.Networking.WebWrapper.DataProviders.METAsrc;
using LoLA.Networking.WebWrapper.DataProviders.UGG;
using LoLA.Networking.WebWrapper.DataProviders;
using LoLA.Networking.WebWrapper.DataDragon;
using static LoLA.Utils.Logger.LogService;
using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using LoLA.Utils.Logger;
using LoLA.Data.Enums;
using System.IO;
using LoLA.Data;
using LoLA.Networking.WebWrapper.DataProviders.OPGG;

namespace LoLA
{
    public static class Main
    {
        public static async Task Init()
        {
            try
            {
                CreateLibFolder();
                DataConverter.Init();

                await Task.WhenAll(MetasrcClass.Init(), 
                    DataDragonWrapper.InitAsync(), 
                    OPGGRankedRoot.InitializeOPGG());

                Log("LoLA has been initialized!", LogType.INFO);
            }
            catch
            {
                Log("Failed to initialize", LogType.EROR);
            }
        }

        public static async Task<ChampionBuild> RequestBuildsData(string championId, GameMode gameMode, Provider provider, Role role = Role.RECOMENDED, int index = 0)
        {
            ChampionBuild championBuild = null;
            IDataProvider dataProvider = null;

            switch (provider)
            {
                case Provider.Local:
                    var buildName = DataProviders.LocalBuild.GetLocalBuildName(championId, gameMode);
                    championBuild = DataProviders.LocalBuild.FetchData(championId, Path.GetFileNameWithoutExtension(buildName), gameMode);
                    break;
                case Provider.METAsrc:
                    dataProvider = new MetasrcWrapper();
                    break;
                case Provider.UGG:
                    if(gameMode == GameMode.CLASSIC || gameMode == GameMode.PRACTICETOOL || gameMode == GameMode.ARAM)
                        dataProvider = new UGGWrapper();
                    else dataProvider = new MetasrcWrapper();
                    break;
                case Provider.OPGG:
                    if (gameMode == GameMode.CLASSIC || gameMode == GameMode.PRACTICETOOL || gameMode == GameMode.ARAM || gameMode == GameMode.URF)
                        dataProvider = new OPGGWrapper();
                    else dataProvider = new MetasrcWrapper();
                    break;
            }

            championBuild = provider != Provider.Local ? await dataProvider?.FetchDataAsync(championId, gameMode, role) : championBuild;
            return championBuild;
        }

        public static void CreateLibFolder()
        {
            if (Directory.Exists(LibInfo.r_LibFolderPath)) return;

            Log($"Creating {LibInfo.NAME} folder...", LogType.INFO);
            Directory.CreateDirectory(LibInfo.r_LibFolderPath);
        }
    }
}
