using static LoLA.Networking.WebWrapper.DataDragon.DataDragonWrapper;
using static LoLA.Utils.Logger.LogService;
using System.Collections.Generic;
using LoLA.Networking.Extensions;
using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using LoLA.Utils.Logger;
using HtmlAgilityPack;
using Newtonsoft.Json;
using LoLA.Data.Enums;
using LoLA.Utils;
using System.Web;
using System.Net;
using LoLA.Data;
using System.IO;
using System;

namespace LoLA.Networking.WebWrapper.DataProviders.METAsrc
{
    public static class MetasrcWrapper
    {
        public static async Task<ChampionBuild> FetchDataAsync(string championId, GameMode gameMode, Role role = Role.RECOMENDED)
        {
            ChampionBuild championBuild = new ChampionBuild();
            var championData = s_Champions.Data[championId];
            var jsonContent = string.Empty;

            var filePath = DataPath(championId, gameMode, role);
            if (IsBuildFileValid(championId, gameMode, role))
            {
                Log($"Loading in {Misc.FixedName(championId)} build data from {Provider.METAsrc}...", LogType.INFO);

                using (var stream = new StreamReader(filePath)) { jsonContent = stream.ReadToEnd(); }
                championBuild = JsonConvert.DeserializeObject<ChampionBuild>(jsonContent);
            }
            else
            {
                Log($"Downloading {Misc.FixedName(championId)} build data from {Provider.METAsrc}...", LogType.INFO);
                string html = await GetHtmlAsync(championId, gameMode, role);

                if (!string.IsNullOrEmpty(html))
                {
                    var document = new HtmlDocument();
                    document.LoadHtml(html);

                    Task<Rune> rune = null;
                    Task<Spell> spell = null;

                    List<Task> tasks = new List<Task> {
                        (rune = GetRuneBuildAsync(championData.id, championData.name, gameMode, role, document)),
                        (spell = GetSpellComboAsync(championData.id, gameMode, role, document))
                    };
                    Parallel.ForEach(tasks, async task => { await task; });

                    championBuild.Id = championData.id;
                    championBuild.Name = championData.name;

                    championBuild.Rune = await rune;
                    championBuild.Spell = await spell;

                    jsonContent = JsonConvert.SerializeObject(championBuild);

                    if (!File.Exists(filePath))
                        File.Create(filePath).Dispose();

                    using var stream = new StreamWriter(filePath);
                    stream.Write(jsonContent);
                }
                else
                {
                    Log("Html [0] NULL", LogType.WARN);
                    return null;
                }
            }

            Log("Build data fetched successfully", LogType.INFO);
            return championBuild;
        }

        public static async Task<Rune> GetRuneBuildAsync(string championId, string championName, GameMode gameMode, Role role,HtmlDocument document)
        {
            Rune rune = null;
            HtmlNodeCollection runeToolTips = null;
            HtmlDocument runeContainerHtml = new HtmlDocument();

            try
            {
                await Task.Run(() => {
                    var runeContainer = document.DocumentNode.SelectSingleNode($"//div[@id='{MetasrcClass.s_Key.Perks}']");
                    //Console.WriteLine(runeContainer.InnerHtml); // Debug

                    if (runeContainer == null) throw new Exception();

                    runeContainerHtml.LoadHtml(runeContainer.InnerHtml);
                    if (string.IsNullOrEmpty(runeContainerHtml.DocumentNode.InnerHtml))
                        File.Delete(DataPath(championId, gameMode, role));

                    runeToolTips = runeContainerHtml.DocumentNode.SelectNodes($"//div[@class='{MetasrcClass.s_Key.TipRB}']");
                });

                if (runeToolTips.Count == 0) return rune;

                var runeIds = new List<int>();
                foreach (var runeToolTip in runeToolTips)
                    runeIds.Add(int.Parse(runeToolTip.GetAttributeValue(MetasrcClass.s_Key.SrcRB, "value")
                    .Replace(MetasrcClass.s_Key.RepRB, string.Empty)));

                foreach (var runeId in runeIds)
                    if (runeId == 0) return rune;

                rune = new Rune()
                {
                    Name = string.Format("METAsrc: {0} {1}", championName, role != Role.RECOMENDED ? $"[{role}]" : string.Empty),
                    PrimaryPath = runeIds[0],
                    Keystone = runeIds[1],
                    Slot1 = runeIds[2],
                    Slot2 = runeIds[3],
                    Slot3 = runeIds[4],

                    SecondaryPath = runeIds[5],
                    Slot4 = runeIds[6],
                    Slot5 = runeIds[7],

                    Shard1 = runeIds[8],
                    Shard2 = runeIds[9],
                    Shard3 = runeIds[10],
                };
            }
            catch (Exception)
            {
                Log("Failed to get rune build", LogType.EROR);
                File.Delete(DataPath(championId, gameMode, role));
            }
            return rune;
        }

        public static async Task<Spell> GetSpellComboAsync(string championId, GameMode gameMode, Role role, HtmlDocument document)
        {
            Spell spell = new Spell();
            HtmlDocument spellContainerHtml = new HtmlDocument();
            await Task.Run(() => {
                try
                {
                    var spellContainer = document.DocumentNode.SelectNodes($"//div[@class='{MetasrcClass.s_Key.Spells}']")[MetasrcClass.s_Key.IndexSP];

                    if (spellContainer == null || string.IsNullOrEmpty(spellContainer.InnerHtml))
                        File.Delete(DataPath(championId, gameMode, role));
                    spellContainerHtml.LoadHtml(spellContainer.InnerHtml);

                    var Spells = spellContainerHtml.DocumentNode.SelectNodes($"//img[@class='{MetasrcClass.s_Key.ImgSP}']");
                    spell.First = Path.GetFileNameWithoutExtension(Spells[MetasrcClass.s_Key.FirstSP].GetAttributeValue(MetasrcClass.s_Key.SrcSP, "value"));
                    spell.Second = Path.GetFileNameWithoutExtension(Spells[MetasrcClass.s_Key.SecondSP].GetAttributeValue(MetasrcClass.s_Key.SrcSP, "value"));
                }
                catch
                {
                    Log("Failed to get spell combo", LogType.EROR);
                    File.Delete(DataPath(championId, gameMode, role));
                }
            });
            return spell;
        }

        public static async Task<string> GetHtmlAsync(string championId, GameMode gameMode, Role role)
        {
            try
            {
                var roleTemp = role.ToString().ToLower();
                var currentRole = roleTemp == "recomended" ? string.Empty : roleTemp;

                string rawHtml = string.Empty;
                string decodedHtml = string.Empty;

                const int maxLength = 5;

                championId = championId.ToLower();

                Log("Fetching Html...", LogType.INFO);

                string version = GlobalConfig.s_LatestPatch ? s_Versions[0] : s_Versions[1];

                if (version.Length > maxLength)
                    version = version.Substring(0, maxLength);

                switch (gameMode)
                {
                    case GameMode.ARAM:
                        rawHtml = await WebEx.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/aram/{version}/champion/{championId}");
                        break;
                    case GameMode.URF:
                        rawHtml = await WebEx.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/urf/champion/{championId}");
                        break;
                    case GameMode.ARURF:
                    case GameMode.CLASSIC:
                    case GameMode.PRACTICETOOL:
                        rawHtml = await WebEx.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/5v5/{version}/champion/{championId}/{currentRole}");
                        break;
                    case GameMode.ONEFORALL:
                        rawHtml = await WebEx.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/ofa/champion/{championId}");
                        break;
                    case GameMode.ULTBOOK:
                        rawHtml = await WebEx.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/ultbook/champion/{championId}");
                        break;
                }
                decodedHtml = HttpUtility.HtmlDecode(rawHtml);
                return decodedHtml;
            }
            catch (WebException webEx)
            {
                Log(webEx.Message, LogType.EROR);
                return null;
            }
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

            if(role != Role.RECOMENDED)
                return Path.Combine(dir, $"METAsrc-{championId} {gameMode} {role}.json");
            else
                return Path.Combine(dir, $"METAsrc-{championId} {gameMode}.json");
        }
    }
}
