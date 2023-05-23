using static LoLA.Utils.Logger.LogService;
using LoLA.Networking.Extensions;
using LoLA.Networking.Model;
using LoLA.Utils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoLA.Networking.WebWrapper.DataProviders.Utils;

namespace LoLA.Networking.WebWrapper.DataProviders.OPGG
{
    public class OPGGRankedRoot
    {
        public string token { get; set; }
        public OPGGRanked[] data { get; set; }

        public static OPGGRankedRoot instance;
        public static async Task InitializeOPGG()
        {
            Log("Initializing OP.GG...", LogType.INFO);

            var webModel = new WebModel() {
                Path = $"{LibInfo.r_LibFolderPath}\\OPGG_Ranked.json", 
                Url = "https://op.gg/api/v1.0/internal/bypass/champions/global/ranked" 
            };

            instance = await WebEx.DlDeAndSaveToFile<OPGGRankedRoot>(webModel);

            // download the op.gg access token.
            var html = await WebEx.RunDownloadStringAsync("https://www.op.gg/");
            instance.token = Helper.ExtractStringBetweenMarkers(html, "\"buildId\":\"", "\",\"assetPrefix\"");
            Log($"OP.GG token acquired [{instance.token}]", LogType.INFO);
        }
    }

    public class OPGGRanked
    {
        public int id { get; set; }
        public Position[] positions { get; set; }
    }

    public class Position
    {
        public string name { get; set; }
        public Stats stats { get; set; } 
    }

    public class Stats
    {
        public float win_rate { get; set; }
        public float pick_rate { get; set; }
        public float role_rate { get; set; }
        public float ban_rate { get; set; }
        public float kda { get; set; }
    }
}
