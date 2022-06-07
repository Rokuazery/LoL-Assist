using static LoLA.WebAPIs.DataDragon.DataDragonWrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using LoLA.Utils.Logger;
using LoLA.Objects;
using LoLA.Utils;
using System.Web;
using System.Net;
using System.IO;
using LoLA.LCU;
using System;

namespace LoLA.WebAPIs.Metasrc
{
    public class MetasrcWrapper
    {
        public readonly MetasrcClass metaClass = new MetasrcClass();
        private readonly HtmlDocument pDocument = new HtmlDocument();

        public async Task<ChampionBD> FetchDataAsync(string championId, GameMode gameMode, string role = "")
        {
            ChampionBD championBuild = new ChampionBD();
            var cInfo = Champions.Data[championId];
            string jsonContent = string.Empty;
            var bd = "build data...";

            string filePath = DataPath(championId, gameMode, role);
            if (IsBuildFileOK(championId, gameMode, role))
            {
                Log($"Loading in {Misc.FixedName(championId)} {bd}", Global.name, LogType.INFO);

                using (var stream = new StreamReader(filePath)) { jsonContent = stream.ReadToEnd(); }
                championBuild = JsonConvert.DeserializeObject<ChampionBD>(jsonContent);
            }
            else
            {
                Log($"Downloading {Misc.FixedName(championId)} {bd}", Global.name, LogType.INFO);
                string html = await GetHtmlAsync(championId, gameMode, role);

                if (!string.IsNullOrEmpty(html))
                {
                    //Console.WriteLine(html); // Debug only
                    pDocument.LoadHtml(html);

                    Task<RuneObj> rune = null;
                    Task<SpellObj> spell = null;

                    List<Task> tasks = new List<Task>
                    {
                        (rune = GetRuneBuildAsync(cInfo.id, cInfo.name, gameMode, role)),
                        (spell = GetSpellComboAsync(cInfo.id, gameMode, role))
                    };
                    Parallel.ForEach(tasks, async task => { await task; });

                    championBuild.name = cInfo.name;
                    championBuild.id = cInfo.id;

                    championBuild.rune = await rune;
                    championBuild.spell = await spell;

                    jsonContent = JsonConvert.SerializeObject(championBuild, Formatting.Indented);

                    if (!File.Exists(filePath))
                        File.Create(filePath).Dispose();

                    using (var stream = new StreamWriter(filePath)) { stream.Write(jsonContent); }
                }
                else
                {
                    Log("Html [0] NULL", Global.name, LogType.WARN);
                    return null;
                }
            }

            Log("Build data fetched successfully", Global.name, LogType.INFO);
            return championBuild;
        }

        private async Task<RuneObj> GetRuneBuildAsync(string championId, string championName, GameMode gameMode, string role)
        {
            RuneObj rune = null;
            HtmlNodeCollection runeToolTips = null;
            HtmlDocument runeContainerHtml = new HtmlDocument();

            try
            {
                await Task.Run(() => {
                    //var runeContainer = SelectNode(pDocument.DocumentNode, gameMode, metaClass.key.IndexRB);
                    var runeContainer = pDocument.DocumentNode.SelectSingleNode($"//div[@id='{metaClass.key.Perks}']");
                    //Console.WriteLine(runeContainer.InnerHtml); // Debug

                    if (runeContainer == null) throw new Exception();
                    runeContainerHtml.LoadHtml(runeContainer.InnerHtml);
                    if (string.IsNullOrEmpty(runeContainerHtml.DocumentNode.InnerHtml))
                        File.Delete(DataPath(championId, gameMode, role));

                    runeToolTips = runeContainerHtml.DocumentNode.SelectNodes($"//div[@class='{metaClass.key.TipRB}']");
                });

                if (runeToolTips.Count == 0) return rune;

                var RuneIds = new List<int>();
                foreach (var runeToolTip in runeToolTips)
                    RuneIds.Add(int.Parse(runeToolTip.GetAttributeValue(metaClass.key.SrcRB, "value")
                    .Replace(metaClass.key.RepRB, string.Empty)));

                foreach (var runeId in RuneIds)
                    if (runeId == 0) return rune;

                rune = new RuneObj()
                {
                    Name = string.Format("LoLA: {0} {1}", championName, !string.IsNullOrEmpty(role) ? $"[{role.ToUpper()}]" : string.Empty),
                    Path0 = RuneIds[0],
                    Keystone = RuneIds[1],
                    Slot1 = RuneIds[2],
                    Slot2 = RuneIds[3],
                    Slot3 = RuneIds[4],

                    Path1 = RuneIds[5],
                    Slot4 = RuneIds[6],
                    Slot5 = RuneIds[7],

                    Shard0 = RuneIds[8],
                    Shard1 = RuneIds[9],
                    Shard2 = RuneIds[10],
                };
            }
            catch(Exception)
            {
                Log("Failed to get rune build", Global.name, LogType.EROR);
                File.Delete(DataPath(championId, gameMode, role));
            }
            return rune;
        }

        private async Task<SpellObj> GetSpellComboAsync(string championId, GameMode gameMode, string role)
        {
            SpellObj spell = new SpellObj();
            HtmlDocument spellContainerHtml = new HtmlDocument();
            await Task.Run(() => {
                try
                {
                    var spellContainer = pDocument.DocumentNode.SelectNodes($"//div[@class='{metaClass.key.Spells}']")[metaClass.key.IndexSP];
                    //Console.WriteLine(spellContainer.InnerHtml); // Debug

                    if (spellContainer == null || string.IsNullOrEmpty(spellContainer.InnerHtml))
                        File.Delete(DataPath(championId, gameMode, role));
                    spellContainerHtml.LoadHtml(spellContainer.InnerHtml);

                    var Spells = spellContainerHtml.DocumentNode.SelectNodes($"//img[@class='{metaClass.key.ImgSP}']");
                    spell.Spell0 = Path.GetFileNameWithoutExtension(Spells[metaClass.key.FirstSP].GetAttributeValue(metaClass.key.SrcSP, "value"));
                    spell.Spell1 = Path.GetFileNameWithoutExtension(Spells[metaClass.key.SecondSP].GetAttributeValue(metaClass.key.SrcSP, "value"));
                }
                catch
                {
                    Log("Failed to get spell combo", Global.name, LogType.EROR);
                    File.Delete(DataPath(championId, gameMode, role));
                }
            });
            return spell;
        }

        //private HtmlNode SelectNode(HtmlNode node, GameMode gm, int index)
        //{
        //    try
        //    {
        //        string classKey = null;
        //        switch (gm)
        //        {
        //            case GameMode.ARAM:
        //                classKey = $"//div[@id='{metaClass.key.ARAM}']";
        //                break;
        //            case GameMode.ARURF:
        //            case GameMode.CLASSIC:
        //            case GameMode.PRACTICETOOL:
        //                classKey = $"//div[@id='{metaClass.key.CLASSIC}']";
        //                break;
        //            case GameMode.URF:
        //            case GameMode.ULTBOOK:
        //            case GameMode.ONEFORALL:
        //                classKey = $"//div[@id='{metaClass.key.MISC}']";
        //                break;
        //        }

        //        var nodes = node.SelectNodes(classKey);
        //        //if (Global.Config.debug)
        //        //{
        //        //    LogService.Log(LogService.Model("--Nodes------------", Global.name, LogType.INFO));
        //        //    foreach (var n in nodes)
        //        //        LogService.Log(LogService.Model("node: " + n.InnerHtml.Replace("\n", string.Empty), Global.name, LogType.DBUG));
        //        //    LogService.Log(LogService.Model("-------------------", Global.name, LogType.INFO));
        //        //}

        //        return nodes[index];
        //    }
        //    catch
        //    {
        //        LogService.Log(LogService.Model("key not found 'SelectNode'/NULL", Global.name, LogType.EROR));
        //        return null;
        //    }
        //}

        private async Task<string> GetHtmlAsync(string championId, GameMode gameMode, string role)
        {
            try
            {
                string rawHtml = string.Empty;
                string decodedHtml = string.Empty;

                const int maxLength = 5;
                string version = null;

                var champId = championId.ToLower();

                Log("Fetching Html...", Global.name, LogType.INFO);

                if (Global.Config.useLatestPatch)
                    version = patches[0];
                else version = patches[1];

                if (version.Length > maxLength)
                    version = version.Substring(0, maxLength);

                switch (gameMode)
                {
                    case GameMode.ARAM:
                        rawHtml = await WebExt.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/aram/{version}/champion/{champId}");
                        break;          
                    case GameMode.URF:
                        rawHtml = await WebExt.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/urf/champion/{champId}");
                        break;
                    case GameMode.ARURF:
                    case GameMode.CLASSIC:
                    case GameMode.PRACTICETOOL:
                        rawHtml = await WebExt.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/5v5/{version}/champion/{champId}/{role}");
                        break;
                    case GameMode.ONEFORALL:
                        rawHtml = await WebExt.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/ofa/champion/{champId}");
                        break;
                    case GameMode.ULTBOOK:
                        rawHtml = await WebExt.RunDownloadStringAsync($"{Protocol.HTTPS}www.metasrc.com/ultbook/champion/{champId}");
                        break;
                }
                decodedHtml = HttpUtility.HtmlDecode(rawHtml);
                //Console.WriteLine(decodedHtml);
                return decodedHtml;
            }
            catch (WebException webEx)
            {
                Log(webEx.Message, webEx.Source, LogType.EROR);
                return null;
            }
        }

        public bool IsBuildFileOK(string championId, GameMode gameMode,string role)
        {
            Log("Checking for build file...", Global.name, LogType.INFO);

            string Json;
            string filePath = DataPath(championId, gameMode, role);

            if (!Global.Config.caching || !File.Exists(filePath))
                return false;

            using(var stream = new StreamReader(filePath)) { Json = stream.ReadToEnd(); }
            if (string.IsNullOrEmpty(Json))
                return false;

            try
            {
                var championBuild = JsonConvert.DeserializeObject<ChampionBD>(Json);
                if (championBuild.rune == null) return false;
                if (string.IsNullOrEmpty(championBuild.spell.Spell0) 
                && string.IsNullOrEmpty(championBuild.spell.Spell1))
                    return false;   
            } catch { return false;  }

            return true;
        }

        public string DataPath(string championId, GameMode gameMode, string role)
        {
            int maxLength = 5;
            string version;

            if (Global.Config.useLatestPatch)
                version = patches[0];
            else version = patches[1];

            if (version.Length > maxLength)
                version = version.Substring(0, maxLength);

            string dir = Path.Combine(ChampionFolder(championId), version);
            Directory.CreateDirectory(dir);

            if(string.IsNullOrEmpty(role))
                return Path.Combine(dir, $"Metasrc-{championId} {gameMode}.json");

            return Path.Combine(dir, $"Metasrc-{championId} {gameMode} {role}.json");
        }

        #region Logs
        private void Log(string log, string source, LogType type) => LogService.Log(LogService.Model(log, Global.name, type));
        #endregion
    }
}
