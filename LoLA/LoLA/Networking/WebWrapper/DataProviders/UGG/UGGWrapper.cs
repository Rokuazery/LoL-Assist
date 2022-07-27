using static LoLA.Networking.WebWrapper.DataDragon.DataDragonWrapper;
using LoLA.Networking.WebWrapper.DataProviders.METAsrc;
using static LoLA.Utils.Logger.LogService;
using LoLA.Networking.Extensions;
using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LoLA.Utils.Logger;
using LoLA.Data.Enums;
using Newtonsoft.Json;
using System.Linq;
using LoLA.Utils;
using LoLA.Data;
using System.IO;

namespace LoLA.Networking.WebWrapper.DataProviders.UGG
{
    public static class UGGWrapper
    {
        private const string UGG_API_VERSION = "1.1";
        private const string UGG_LOL_VERSION = "12_12";
        private const string UGG_OVERVIEW_VERSION = "1.5.0";
        private const string UGG_API_URL = "stats2.u.gg/lol/";

        private const int OVERVIEW_WORLD = 12;
        private const int OVERVIEW_PLATINUM_PLUS = 10;
        private static string getOverviewUrl(string championKey, GameMode gameMode)
        {
            string gameModeUrlPath = string.Empty;

            switch (gameMode)
            {
                case GameMode.PRACTICETOOL:
                case GameMode.CLASSIC:
                    gameModeUrlPath = "ranked_solo_5x5";
                    break;
                case GameMode.ARAM:
                    gameModeUrlPath = "normal_aram";
                    break;  
            }

            var apiUrl = $"{Protocol.HTTPS}{UGG_API_URL}{UGG_API_VERSION}/overview/{UGG_LOL_VERSION}/{gameModeUrlPath}/{championKey}/{UGG_OVERVIEW_VERSION}.json";
            
            return apiUrl;
        }

        public static async Task<ChampionBuild> FetchDataAsync(string championId, GameMode gameMode, Role role = Role.RECOMENDED)
        {
            ChampionBuild championBuild = new ChampionBuild();
            var championData = s_Champions.Data[championId]; 
            var jsonContent = string.Empty;

            var filePath = DataPath(championId, gameMode, role);
            if (IsBuildFileValid(championId, gameMode, role))
            {
                Log($"Loading in {Misc.FixedName(championId)} build data from {Provider.UGG}...", LogType.INFO);
                using (var stream = new StreamReader(filePath)) { jsonContent = stream.ReadToEnd(); }
                championBuild = JsonConvert.DeserializeObject<ChampionBuild>(jsonContent);
            }
            else
            {
                Log($"Downloading {Misc.FixedName(championId)} build data from {Provider.UGG}...", LogType.INFO);

                var championJObject = await GetChampionDataAsync(championData.key, gameMode);

                if (championJObject != null)
                {
                    championBuild.Id = championData.id;
                    championBuild.Name = championData.name;
                    championBuild.Rune = GetRune(championData.name, role, championJObject);
                    championBuild.Spell = GetSpellCombo(role, championJObject);

                    jsonContent = JsonConvert.SerializeObject(championBuild);

                    if (!File.Exists(filePath))
                        File.Create(filePath).Dispose();

                    using var stream = new StreamWriter(filePath);
                    stream.Write(jsonContent);
                }
                else
                {
                    Log("JObject [0] NULL", LogType.WARN);
                    return null;
                }
            }

            Log("Build data fetched successfully", LogType.INFO);
            return championBuild;
        }

        public static async Task<JObject> GetChampionDataAsync(string championKey, GameMode gameMode)
        {
            var rawDataObject = await WebEx.DlDe<JObject>(getOverviewUrl(championKey, gameMode));
            var championJObject = (JObject)rawDataObject[OVERVIEW_WORLD.ToString()][OVERVIEW_PLATINUM_PLUS.ToString()];
            return championJObject;
        }

        public static Role[] GetPossibleRoles(JObject jObject)
        {
            JToken championData = jObject;
            int totalGames = championData.Sum(o => ((JProperty)o).Value[0][0][0].ToObject<int>());

            //Only count positions that make up for more than 10% of the champion's total played games
            return championData
                .Cast<JProperty>()
                .Select((o, i) => o.Value[0][0][0].ToObject<float>() / totalGames > 0.1f ? ((Role)i + 1) : Role.RECOMENDED)
                .Where(r => r != Role.RECOMENDED)
                .ToArray();
        }

        public static Rune GetRune(string championName, Role role, JObject jObject)
        {
            Rune rune = new Rune();
            var championData = jObject;

            var roleTemp = role;

            if (role == Role.RECOMENDED)
                role = GetPossibleRoles(jObject)[0];

            var root = championData[((int)role).ToString()].First();
            var perksRoot = root.First();
            var shardsRoot = root[8][2];

            int primaryPath = perksRoot[2].ToObject<int>();
            int secondaryPath = perksRoot[3].ToObject<int>();
            int[] runeIds = perksRoot[4].Select(p => p.ToObject<int>())  // perks
                .Concat(shardsRoot.Select(s => int.Parse(s.ToString()))) // shards
                .ToArray();

            rune = new Rune()
            {
                Name = string.Format("U.GG: {0} {1}", championName, roleTemp != Role.RECOMENDED ? $"[{role}]" : string.Empty),
                PrimaryPath = primaryPath,
                Keystone = runeIds[0],
                Slot1 = runeIds[1],
                Slot2 = runeIds[2],
                Slot3 = runeIds[3],

                SecondaryPath = secondaryPath,
                Slot4 = runeIds[4],
                Slot5 = runeIds[5],

                Shard1 = runeIds[6],
                Shard2 = runeIds[7],
                Shard3 = runeIds[8],
            };

            return rune;
        }

        public static Spell GetSpellCombo(Role role, JObject jObject)
        {
            var spell = new Spell();
            var championData = jObject;

            if (role == Role.RECOMENDED)
                role = GetPossibleRoles(jObject)[0];

            var root = championData[((int)role).ToString()].First();
            var spells = root[1][2];
            spell.First = DataConverter.SpellKeyToSpellId(spells[0].ToObject<int>());
            spell.Second = DataConverter.SpellKeyToSpellId(spells[1].ToObject<int>());

            return spell;
        }

        public static bool IsBuildFileValid(string championId, GameMode gameMode, Role role)
        {
            Log("Checking for build file...", LogType.INFO);

            string json;
            string filePath = DataPath(championId, gameMode, role);

            if (!GlobalConfig.s_Caching || !File.Exists(filePath))
                return false;

            using (var stream = new StreamReader(filePath)) { json = stream.ReadToEnd(); }

            if (string.IsNullOrEmpty(json))
                return false;

            try
            {
                var championBuild = JsonConvert.DeserializeObject<ChampionBuild>(json);

                if (championBuild.Rune == null) return false;

                if (string.IsNullOrEmpty(championBuild.Spell.First)
                && string.IsNullOrEmpty(championBuild.Spell.Second))
                    return false;
            }
            catch { return false; }

            return true;
        }

        public static string DataPath(string championId, GameMode gameMode, Role role)
        {
            int maxLength = 5;

            string version = GlobalConfig.s_LatestPatch ? s_Versions[0] : s_Versions[1];

            if (version.Length > maxLength)
                version = version.Substring(0, maxLength);

            string dir = Path.Combine(ChampionFolder(championId), version);
            Directory.CreateDirectory(dir);

            if (role != Role.RECOMENDED)
                return Path.Combine(dir, $"UGG-{championId} {gameMode} {role}.json");
            else
                return Path.Combine(dir, $"UGG-{championId} {gameMode}.json");
        }
    }
}
