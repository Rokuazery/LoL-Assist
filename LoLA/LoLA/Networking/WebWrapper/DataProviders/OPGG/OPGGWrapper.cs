using static LoLA.Networking.WebWrapper.DataDragon.DataDragonWrapper;
using static LoLA.Networking.WebWrapper.DataProviders.Utils.Helper;
using static LoLA.Utils.Logger.LogService;
using System.Collections.Generic;
using LoLA.Networking.Extensions;
using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using LoLA.Utils.Logger;
using LoLA.Data.Enums;
using Newtonsoft.Json;
using System.Linq;
using LoLA.Utils;
using LoLA.Data;
using System.IO;
using System;

namespace LoLA.Networking.WebWrapper.DataProviders.OPGG
{
    public class OPGGWrapper : IDataProvider
    {
        public async Task<ChampionBuild> FetchDataAsync(string championId, GameMode gameMode, Role role = Role.RECOMENDED)
        {
            ChampionBuild championBuild = new ChampionBuild();
            var championData = s_Champions.Data[championId];
            var jsonContent = string.Empty;

            if(role == Role.RECOMENDED && IsClassicGameMode(gameMode))
            {
                // Shouldn't be any error here so no need to null check
                var recommendedRole = OPGGRankedRoot.instance.data.SingleOrDefault(d => d.id.ToString() == championData.key);
                role = (Role)Enum.Parse(typeof(Role), recommendedRole.positions[0].name.ToUpper());
            }

            var filePath = DataPath(championId, gameMode, role, Provider.OPGG);
            if (IsBuildFileValid(championId, gameMode, role, Provider.OPGG))
            {
                Log($"Loading in {Misc.FixedName(championId)} build data from {Provider.OPGG}...", LogType.INFO);
                using (var stream = new StreamReader(filePath)) { jsonContent = stream.ReadToEnd(); }
                championBuild = JsonConvert.DeserializeObject<ChampionBuild>(jsonContent);
            }
            else
            {
                Log($"Downloading {Misc.FixedName(championId)} build data from {Provider.OPGG}...", LogType.INFO);
                OPGGChampionData rawChampionData = await GetChampionDataAsync(championId, gameMode, role);

                if(rawChampionData == null)
                {
                    Log("OPGGChampionData [0] NULL", LogType.WARN);
                    return null;
                }

                championBuild.Id = championData.id;
                championBuild.Name = championData.name;
                championBuild.Runes = GetRunes(championData.name, gameMode, role, rawChampionData);
                championBuild.Spells = GetSpellCombos(rawChampionData);
                championBuild.ChampionSkill = GetSkillOrder(rawChampionData);

                jsonContent = JsonConvert.SerializeObject(championBuild);

                if (!File.Exists(filePath))
                    File.Create(filePath).Dispose();

                using (var stream = new StreamWriter(filePath))
                {
                    stream.Write(jsonContent);
                }
            }

            return championBuild;
        }

        public List<Rune> GetRunes(string championName, GameMode gameMode, Role role, OPGGChampionData championData)
        {
            Log("Loading in runes", LogType.INFO);
            List<Rune> result = new List<Rune>();
            foreach (var rune in championData.pageProps.data.runes)
            {
                if(rune == null) continue;

                var runeTemp = new Rune()
                {
                    Name = $"OP.GG: {championName} {(IsClassicGameMode(gameMode) ? $"[{role}]" : null)} PICK RATE: {rune.pick_rate * 100}%",
                    PrimaryPath = rune.primary_page_id,
                    Keystone = rune.primary_rune_ids[0],
                    Slot1 = rune.primary_rune_ids[1],
                    Slot2 = rune.primary_rune_ids[2],
                    Slot3 = rune.primary_rune_ids[3],
                    SecondaryPath = rune.secondary_page_id,
                    Slot4 = rune.secondary_rune_ids[0],
                    Slot5 = rune.secondary_rune_ids[1],
                    Shard1 = rune.stat_mod_ids[0],
                    Shard2 = rune.stat_mod_ids[1],
                    Shard3 = rune.stat_mod_ids[2]
                };
                result.Add(runeTemp);
            }

            return result;
        }

        public List<Spell> GetSpellCombos(OPGGChampionData championData)
        {
            Log("Loading in spell combo", LogType.INFO);
            List<Spell> result = new List<Spell>();
            foreach (var spell in championData.pageProps.data.summoner_spells)
            {
                if (spell == null) continue;

                var spellTemp = new Spell()
                {
                    First = DataConverter.SpellKeyToSpellId(spell.ids[0].ToString()),
                    Second = DataConverter.SpellKeyToSpellId(spell.ids[1].ToString()),
                };
                result.Add(spellTemp);
            }

            return result;
        }

        public ChampionSkill GetSkillOrder(OPGGChampionData championData)
        {
            Log("Loading in skill order", LogType.INFO);
            ChampionSkill championSkill = new ChampionSkill();

            foreach (var skill in championData.pageProps.data.skill_masteries[0].ids)
                championSkill.Priority += skill.ToString();

            var order = championData.pageProps.data.skill_masteries[0].builds[0].order;
            championSkill.Order = new ChampSkill[order.Length];

            for (int i = 0; i < championSkill.Priority.Length; i++)
            {
                championSkill.Order[i] = new ChampSkill()
                {
                    Index = i + 1,
                    Skill = championSkill.Priority[i].ToString()
                };
            }

            for (int i = championSkill.Priority.Length; i < order.Length; i++)
            {
                championSkill.Order[i] = new ChampSkill()
                {
                    Index = i + 1,
                    Skill = order[i]
                };
            }

            return championSkill;
        }

        public async Task<OPGGChampionData> GetChampionDataAsync(string championId, GameMode gm, Role role)
        {
            var championData = await WebEx.DlDe<OPGGChampionData>(getAPIUrl(championId, gm, role));
            return championData;
        }

        private string getAPIUrl(string championId, GameMode gameMode, Role role)
        {
            if (string.IsNullOrEmpty(OPGGRankedRoot.instance.token)) throw new Exception("op.gg token is empty");

            var urlMainPath = $"https://www.op.gg/_next/data/{OPGGRankedRoot.instance.token}";
            switch (gameMode)
            {
                case GameMode.PRACTICETOOL:
                case GameMode.CLASSIC:
                    var roleStr = Enum.GetName(typeof(Role), role).ToLower();
                    urlMainPath += $"/champions/{championId}/{roleStr}/build.json?champion={championId}&position={roleStr}";
                    break;
                case GameMode.ARAM:
                case GameMode.URF:
                    urlMainPath += $"/modes/{(gameMode == GameMode.ARAM ? "aram" : "urf")}/{championId}/build.json?region=global&champion={championId}";
                    break;
            }
            #if DEBUG
                Log(urlMainPath, LogType.DBUG);
            #endif
            return urlMainPath;
        }
    }
}
