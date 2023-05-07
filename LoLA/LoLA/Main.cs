//using LoLA.Networking.WebWrapper.DataProviders.METAsrc;
//using LoLA.Networking.WebWrapper.DataProviders.UGG;
using LoLA.Networking.WebWrapper.DataProviders;
using LoLA.Networking.WebWrapper.DataDragon;
using static LoLA.Utils.Logger.LogService;
using System.Collections.Generic;
using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using LoLA.Utils.Logger;
using LoLA.Data.Enums;
using System.IO;
using LoLA.Data;
using System;
using LoLA.Networking.WebWrapper.DataProviders.METAsrc;
using LoLA.Networking.WebWrapper.DataProviders.UGG;

namespace LoLA
{
    public static class Main
    {
        public static async Task<bool> Init()
        {
            try
            {
                CreateLibFolder();
                await Task.Run(() => { MetasrcClass.Init(); });
                //await DataConverter.InitAsync();
                DataConverter.Init();
                await DataDragonWrapper.InitAsync();

                Log("LoLA has been initialized!", LogType.INFO);
                return true;
            }
            catch
            {
                Log("Failed to initialize", LogType.EROR);
                return false;
            }
        }

        public static async Task<ChampionBuild> RequestBuildsData(string championId, GameMode gameMode, Provider provider, Role role = Role.RECOMENDED, int index = 0)
        {
            ChampionBuild championBuild;
            IDataProvider dataProvider= null;

            switch (provider)
            {
                case Provider.Local:
                    var buildName = LocalBuild.GetLocalBuildName(championId, gameMode);
                    championBuild = LocalBuild.FetchData(championId, Path.GetFileNameWithoutExtension(buildName), gameMode);
                    break;
                case Provider.METAsrc:
                    dataProvider = new MetasrcWrapper();
                    break;
                case Provider.UGG:
                    if(gameMode == GameMode.CLASSIC || gameMode == GameMode.PRACTICETOOL || gameMode == GameMode.ARAM)
                        dataProvider = new UGGWrapper();
                    else dataProvider = new MetasrcWrapper();
                    break;
                case Provider.Mobafire:
                    break;
            }

            championBuild = await dataProvider?.FetchDataAsync(championId, gameMode, role);
            return championBuild;
        }

        public static void CreateLibFolder()
        {
            if (!Directory.Exists(LibInfo.r_LibFolderPath))
            {
                Log($"Creating {LibInfo.NAME} folder...", LogType.INFO);
                Directory.CreateDirectory(LibInfo.r_LibFolderPath);
            }
        }
    }
}
