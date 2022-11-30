using static LoLA.Networking.WebWrapper.DataDragon.DataDragonWrapper;
using static LoLA.Networking.WebWrapper.DataProviders.Utils.Helper;
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

            var filePath = DataPath(championId, gameMode, role, Provider.METAsrc);
            if (IsBuildFileValid(championId, gameMode, role, Provider.METAsrc))
            {
                Log($"Loading in {Misc.FixedName(championId)} build data from {Provider.METAsrc}...", LogType.INFO);

                using (var stream = new StreamReader(filePath)) { jsonContent = stream.ReadToEnd(); }
                championBuild = JsonConvert.DeserializeObject<ChampionBuild>(jsonContent);
            }
            else
            {
                Log($"Downloading {Misc.FixedName(championId)} build data from {Provider.METAsrc}...", LogType.INFO);
                var html = await GetHtmlAsync(championId, gameMode, role);

                if (!string.IsNullOrEmpty(html))
                {
                    var document = new HtmlDocument();
                    document.LoadHtml(html);

                    Task<List<Rune>> runeTasks = null;
                    Task<List<Spell>> spellTasks = null;

                    List<Task> tasks = new List<Task> {
                        (runeTasks = GetRunesAsync(championData.id, championData.name, gameMode, role, document)),
                        (spellTasks = GetSpellCombosAsync(championData.id, gameMode, role, document))
                    };
                    Parallel.ForEach(tasks, async task => { await task; });

                    championBuild.Id = championData.id;
                    championBuild.Name = championData.name;

                    championBuild.Runes = await runeTasks;
                    championBuild.Spells = await spellTasks;

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

        public static async Task<List<Rune>> GetRunesAsync(string championId, string championName, GameMode gameMode, Role role,HtmlDocument document)
        {
            List<Rune> runes = new List<Rune>();
            HtmlNodeCollection runeToolTips = null;
            HtmlDocument runeContainerHtml = new HtmlDocument();

            try
            {
                Log("Loading in runes...", LogType.INFO);
                await Task.Run(() => {
                    var runeContainer = document.DocumentNode.SelectSingleNode($"//div[@id='{MetasrcClass.s_Key.Perks}']");
                    //Console.WriteLine(runeContainer.InnerHtml); // Debug

                    if (runeContainer == null) throw new Exception();

                    runeContainerHtml.LoadHtml(runeContainer.InnerHtml);
                    if (string.IsNullOrEmpty(runeContainerHtml.DocumentNode.InnerHtml))
                        File.Delete(DataPath(championId, gameMode, role, Provider.METAsrc));

                    runeToolTips = runeContainerHtml.DocumentNode.SelectNodes($"//div[@class='{MetasrcClass.s_Key.TipRB}']");
                });

                if (runeToolTips.Count == 0) throw new Exception();

                var runeIds = new List<int>();
                foreach (var runeToolTip in runeToolTips)
                    runeIds.Add(int.Parse(runeToolTip.GetAttributeValue(MetasrcClass.s_Key.SrcRB, "value")
                    .Replace(MetasrcClass.s_Key.RepRB, string.Empty)));

                foreach (var runeId in runeIds)
                    if (runeId == 0) throw new Exception();

                var rune = new Rune()
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
                runes.Add(rune);
            }
            catch (Exception)
            {
                Log("Failed to get rune build", LogType.EROR);
                File.Delete(DataPath(championId, gameMode, role, Provider.METAsrc));
            }
            return runes;
        }

        public static async Task<List<Spell>> GetSpellCombosAsync(string championId, GameMode gameMode, Role role, HtmlDocument document)
        {
            List<Spell> spells = new List<Spell>();
            HtmlDocument spellContainerHtml = new HtmlDocument();
            await Task.Run(() => {
                try
                {
                    Log("Loading in spell combo...", LogType.INFO);
                    var spellContainer = document.DocumentNode.SelectNodes($"//div[@class='{MetasrcClass.s_Key.Spells}']")[MetasrcClass.s_Key.IndexSP];

                    if (spellContainer == null || string.IsNullOrEmpty(spellContainer.InnerHtml))
                        File.Delete(DataPath(championId, gameMode, role, Provider.METAsrc));
                    spellContainerHtml.LoadHtml(spellContainer.InnerHtml);

                    var spellsImage = spellContainerHtml.DocumentNode.SelectNodes($"//img[@class='{MetasrcClass.s_Key.ImgSP}']");

                    var spell = new Spell();

                    spell.First = Path.GetFileNameWithoutExtension(spellsImage[MetasrcClass.s_Key.FirstSP].GetAttributeValue(MetasrcClass.s_Key.SrcSP, "value"));
                    spell.Second = Path.GetFileNameWithoutExtension(spellsImage[MetasrcClass.s_Key.SecondSP].GetAttributeValue(MetasrcClass.s_Key.SrcSP, "value"));
                    spells.Add(spell);
                }
                catch
                {
                    Log("Failed to get spell combo", LogType.EROR);
                    File.Delete(DataPath(championId, gameMode, role, Provider.METAsrc));
                }
            });
            return spells;
        }

        public static async Task<string> GetHtmlAsync(string championId, GameMode gameMode, Role role)
        {
            try
            {
                var roleTemp = role.ToString().ToLower();
                var currentRole = roleTemp == "recomended" ? string.Empty : roleTemp;

                var rawHtml = string.Empty;
                var decodedHtml = string.Empty;

                championId = championId.ToLower();

                Log("Fetching Html...", LogType.INFO);

                var lolPatch = GlobalConfig.s_LatestPatch ? GetPatchMM(0) : GetPatchMM(1);

                string url = null;

                switch (gameMode)
                {
                    case GameMode.ARAM:
                        url = $"{Protocol.HTTPS}www.metasrc.com/aram/{lolPatch}/champion/{championId}";
                        break;
                    case GameMode.URF:
                        url = $"{Protocol.HTTPS}www.metasrc.com/urf/champion/{championId}";
                        break;
                    case GameMode.ARURF:
                    case GameMode.CLASSIC:
                    case GameMode.PRACTICETOOL:
                        url = $"{Protocol.HTTPS}www.metasrc.com/5v5/{lolPatch}/champion/{championId}/{currentRole}";
                        break;
                    case GameMode.ONEFORALL:
                        url = $"{Protocol.HTTPS}www.metasrc.com/ofa/champion/{championId}";
                        break;
                    case GameMode.ULTBOOK:
                        url = $"{Protocol.HTTPS}www.metasrc.com/ultbook/champion/{championId}";
                        break;
                }
                
                rawHtml = url != null ? await WebEx.RunDownloadStringAsync(url) : throw new Exception();

                decodedHtml = HttpUtility.HtmlDecode(rawHtml);
                return decodedHtml;
            }
            catch (WebException webEx)
            {
                Log(webEx.Message, LogType.EROR);
            }
            catch
            {
                Log("NULL URL",LogType.EROR);
            }
            return null;
        }
    }
}
