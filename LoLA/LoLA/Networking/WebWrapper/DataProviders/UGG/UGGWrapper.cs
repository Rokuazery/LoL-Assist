﻿using static LoLA.Networking.WebWrapper.DataDragon.DataDragonWrapper;
using static LoLA.Networking.WebWrapper.DataProviders.Utils.Helper;
using static LoLA.Utils.Logger.LogService;
using System.Collections.Generic;
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
using System;

namespace LoLA.Networking.WebWrapper.DataProviders.UGG
{
    public class UGGWrapper: IDataProvider
    {
        // Get available roles/positions here https://stats2.u.gg/lol/1.5/champion_ranking/world/13_9/ranked_solo_5x5/platinum_plus/1.5.0.json
        private const string UGG_API_VERSION = "1.5";
        private const string UGG_OVERVIEW_VERSION = "1.5.0";
        private const string UGG_API_URL = "stats2.u.gg/lol/";

        private const int OVERVIEW_WORLD = 12;
        private const int OVERVIEW_PLATINUM_PLUS = 10;

        private string GetCurrentPatch()
            => GetPatchMM().Replace('.', '_');

        private string GetOverviewUrl(string championKey, GameMode gameMode)
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

            var apiUrl = $"{Protocol.HTTPS}{UGG_API_URL}{UGG_API_VERSION}/overview/{GetCurrentPatch()}/{gameModeUrlPath}/{championKey}/{UGG_OVERVIEW_VERSION}.json";

            #if DEBUG
            Log(apiUrl, LogType.DBUG);
            #endif
            return apiUrl;
        }

        public async Task<ChampionBuild> FetchDataAsync(string championId, GameMode gameMode, Role role = Role.RECOMENDED)
        {
            ChampionBuild championBuild = new ChampionBuild();
            var championData = s_Champions.Data[championId]; 
            var jsonContent = string.Empty;

            var filePath = DataPath(championId, gameMode, role, Provider.UGG);

            if (IsBuildFileValid(championId, gameMode, role, Provider.UGG))
            {
                Log($"Loading in {Misc.FixedName(championId)} build data from {Provider.UGG}...", LogType.INFO);
                using (var stream = new StreamReader(filePath)) { jsonContent = stream.ReadToEnd(); }
                championBuild = JsonConvert.DeserializeObject<ChampionBuild>(jsonContent);
            }
            else
            {
                Log($"Downloading {Misc.FixedName(championId)} build data from {Provider.UGG}...", LogType.INFO);
                var championJObject = await GetChampionDataAsync(championData.key, gameMode);

                if (championJObject == null)
                {
                    Log("JObject [0] NULL", LogType.WARN);
                    return null;
                }

                role = role == Role.RECOMENDED && IsClassicGameMode(gameMode)
                ? GetPossibleRoles(championJObject)[0] : role;

                championBuild.Id = championData.id;
                championBuild.Name = championData.name;
                championBuild.Runes = GetRunes(championData.name, gameMode, role, championJObject);
                championBuild.Spells = GetSpellCombos(gameMode, role, championJObject);
                championBuild.ChampionSkill = GetSkillOrder(gameMode, role, championJObject);

                jsonContent = JsonConvert.SerializeObject(championBuild);

                if (!File.Exists(filePath))
                    File.Create(filePath).Dispose();

                using (var stream = new StreamWriter(filePath))
                {
                    stream.Write(jsonContent);
                }
            }

            Log("Build data fetched successfully", LogType.INFO);
            return championBuild;
        }


        public async Task<JObject> GetChampionDataAsync(string championKey, GameMode gm)
        {
            var rawDataObject = await WebEx.DlDe<JObject>(GetOverviewUrl(championKey, gm));
            if (gm != GameMode.ARAM)
            {
                var championJObject = (JObject)rawDataObject[OVERVIEW_WORLD.ToString()][OVERVIEW_PLATINUM_PLUS.ToString()];
                return championJObject;
            }
            return rawDataObject;
        }

        public Role[] GetPossibleRoles(JObject jObject)
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

        public List<Rune> GetRunes(string championName, GameMode gm, Role role, JObject championData)
        {
            Log("Loading in runes", LogType.INFO);
            List<Rune> runes = new List<Rune>();

            if (gm != GameMode.ARAM)
            {
                var root = championData[((int)role).ToString()].FirstOrDefault();
                var rune = FilterRune(root, $"U.GG: {championName} [{role}]");
                runes.Add(rune); 
                return runes;
            }

            List<JToken> tokens = new List<JToken>();
            foreach (var item in championData.Children())
                item.ToList().ForEach(i => tokens.Add(i["8"]["6"]));

            int runeIndex = 0;

            foreach (var token in tokens)
            {
                var root = token.FirstOrDefault();
                var runeTemp = FilterRune(root, $"U.GG: {championName} ARAM[{runeIndex}]");

                runes.Add(runeTemp);
                runeIndex++;
            }

            if (runes.Count == 0)
            {
                Log("Runes [0] NULL", LogType.WARN);
                return null;
            }

            return runes;
        }

        private static Rune FilterRune(JToken root, string runeName)
        {
            Log("Filtering rune...", LogType.INFO);
            try
            {
                var rune = new Rune();

                var perksRoot = root.First();
                var shardsRoot = root[8][2];

                var primaryPath = perksRoot[2].ToObject<int>();
                var secondaryPath = perksRoot[3].ToObject<int>();
                var runeIds = perksRoot[4].Select(p => p.ToObject<int>())  // perks
                .Concat(shardsRoot.Select(s => int.Parse(s.ToString()))) // shards
                .ToArray();

                rune.Name = runeName;
                rune.PrimaryPath = primaryPath;
                rune.Keystone = runeIds[0];

                var primaryPerks = s_Perks.Where(p => p.id == rune.PrimaryPath).ToList();

                int[] primaryPerksFound = new int[2];
                int primaryPerkSlot = 1;

                // For some reason with the new API version of U.GG they randomize the index for the runeIds or I just don't know what's happening
                // Wonky way to fix 

                for (int i = 0; i < 3; i++)
                {
                    foreach (var path in primaryPerks)
                    {
                        foreach (var runeInfo in path.slots[primaryPerkSlot].runes)
                        {
                            if (runeIds.Contains(runeInfo.id))
                            {
                                switch (primaryPerkSlot)
                                {
                                    case 1:
                                        rune.Slot1 = runeInfo.id;
                                        break;
                                    case 2:
                                        rune.Slot2 = runeInfo.id;
                                        break;
                                    case 3:
                                        rune.Slot3 = runeInfo.id;
                                        break;
                                }
                                primaryPerkSlot++;
                                break;
                            }
                        }
                        if (primaryPerkSlot > i) break;
                    }
                }

                rune.SecondaryPath = secondaryPath;

                int secondaryPerkFound = 0;
                int tempSecondaryPerkFound = 0;

                var availableRunes = s_Perks.Where(p => p.id == rune.SecondaryPath)
                    .SelectMany(path => path.slots)
                    .SelectMany(slot => slot.runes)
                    .Where(runeInfo => runeIds.Contains(runeInfo.id) && runeInfo.id != tempSecondaryPerkFound);

                foreach (var runeInfo in availableRunes)
                {
                    if (secondaryPerkFound == 0)
                    {
                        tempSecondaryPerkFound = runeInfo.id;
                        rune.Slot4 = runeInfo.id;
                        secondaryPerkFound++;
                    }
                    else if (secondaryPerkFound == 1)
                    {
                        rune.Slot5 = runeInfo.id;
                        break;
                    }
                }

                rune.Shard1 = runeIds[6];
                rune.Shard2 = runeIds[7];
                rune.Shard3 = runeIds[8];

                return rune;
            }
            catch
            {
                Log("Failed to filter rune", LogType.EROR);
                return null;
            }
        }

        public List<Spell> GetSpellCombos(GameMode gm, Role role, JObject championData)
        {
            Log("Loading in spell combo", LogType.INFO);
            List<Spell> spells = new List<Spell>();
            if (gm != GameMode.ARAM)
            {
                var root = championData[((int)role).ToString()].FirstOrDefault();
                var spellRoots = root[1][2];

                var spellTemp = new Spell
                {
                    First = DataConverter.SpellKeyToSpellId(spellRoots[0].ToObject<string>()),
                    Second = DataConverter.SpellKeyToSpellId(spellRoots[1].ToObject<string>())
                };
                spells.Add(spellTemp);
            }
            else
            {
                List<JToken> tokens = new List<JToken>();
                foreach (var item in championData.Children())
                    item.ToList().ForEach(i => tokens.Add(i["8"]["6"]));


                foreach (var token in tokens)
                {
                    var spellTemp = new Spell();

                    var root = token.First();
                    var spellRoots = root[1][2];

                    spellTemp.First = DataConverter.SpellKeyToSpellId(spellRoots[0].ToObject<string>());
                    spellTemp.Second = DataConverter.SpellKeyToSpellId(spellRoots[1].ToObject<string>());

                    spells.Add(spellTemp);
                }

                if (spells.Count == 0)
                {
                    Log("Spells [0] NULL", LogType.WARN);
                    return null;
                }
            }

            return spells;
        }

        public ChampionSkill GetSkillOrder(GameMode gm, Role role, JObject championData)
        {
            Log("Loading in skill order", LogType.INFO);
            ChampionSkill championSkill = new ChampionSkill();

            var root = gm != GameMode.ARAM ? championData[((int)role).ToString()].FirstOrDefault() : championData.Children().FirstOrDefault();
            var champSkill = gm != GameMode.ARAM ? root[4] : root.FirstOrDefault()["8"]["6"][0][4];


            var order = champSkill[2].Select(o => o.ToString()).ToArray();
            championSkill.Order = new ChampSkill[order.Length];

            for (int i = 0; i < order.Length; i++)
            {
                championSkill.Order[i] = new ChampSkill()
                {
                    Index = i + 1,
                    Skill = order[i]
                };
            }
           
            championSkill.Priority = champSkill[3].ToString();
            return championSkill;
        }
    }
}
