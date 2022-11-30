using LoLA.Networking.WebWrapper.DataProviders.METAsrc;
using LoLA.Networking.WebWrapper.DataProviders.UGG;
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
            ChampionBuild championBuild = new ChampionBuild();

            switch (provider)
            {
                case Provider.Local:
                    var buildName = LocalBuild.GetLocalBuildName(championId, gameMode);
                    championBuild = LocalBuild.FetchData(championId, Path.GetFileNameWithoutExtension(buildName), gameMode);
                    break;
                case Provider.METAsrc:
                    championBuild = await MetasrcWrapper.FetchDataAsync(championId, gameMode, role);
                    break;
                case Provider.UGG:
                    if(gameMode == GameMode.CLASSIC || gameMode == GameMode.PRACTICETOOL || gameMode == GameMode.ARAM)
                        championBuild = await UGGWrapper.FetchDataAsync(championId, gameMode, role, index);
                    else
                        championBuild = await MetasrcWrapper.FetchDataAsync(championId, gameMode, role);
                    break;
                case Provider.Mobafire:
                    break;
            }
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
