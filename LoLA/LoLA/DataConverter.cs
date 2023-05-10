using static LoLA.Utils.Logger.LogService;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoLA.Utils.Logger;
using System;
using System.Xml.Linq;

namespace LoLA
{
    public static class DataConverter
    {
        public static string SpellIdToSpellKey(string id)
        {
            var spellIdToSpellKey = new Dictionary<string, string> {
                { "SummonerBarrier", "21" },
                { "SummonerBoost", "1" },
                { "SummonerExhaust", "3" },
                { "SummonerFlash", "4" },
                { "SummonerHaste", "6" },
                { "SummonerHeal", "7" },
                { "SummonerSmite", "11" },
                { "SummonerTeleport", "12" },
                { "SummonerMana", "13" },
                { "SummonerDot", "14" },
                { "SummonerSnowball", "32" }
            };

            if (string.IsNullOrEmpty(id) || !spellIdToSpellKey.ContainsKey(id)) return null;

            return spellIdToSpellKey[id];
        }

        public static string SpellKeyToSpellId(string key)
        {
            var spellKeyToSpellName = new Dictionary<string, string> {
                { "21", "SummonerBarrier" },
                { "1", "SummonerBoost" },
                { "3", "SummonerExhaust" },
                { "4", "SummonerFlash" },
                { "6", "SummonerHaste" },
                { "7", "SummonerHeal" },
                { "11", "SummonerSmite" },
                { "12", "SummonerTeleport" },
                { "13", "SummonerMana" },
                { "14", "SummonerDot" },
                { "32", "SummonerSnowball" }
            };

            if (!spellKeyToSpellName.ContainsKey(key))
                throw new ArgumentException($"Invalid spell key: {key}");

            return spellKeyToSpellName[key];
        }

        public static string SpellKeyToSpellName(string key)
        {
            var spellKeyToSpellName = new Dictionary<string, string> {
                { "21", "Barrier" },
                { "1", "Cleanse" },
                { "3", "Exhaust" },
                { "4", "Flash" },
                { "6", "Ghost" },
                { "7", "Heal" },
                { "11", "Smite" },
                { "12", "Teleport" },
                { "13", "Clarity" },
                { "14", "Ignite" },
                { "32", "Mark" }
            };

            if (!spellKeyToSpellName.ContainsKey(key))
                throw new ArgumentException($"Invalid spell key: {key}");

            return spellKeyToSpellName[key];
        }



        public static string SpellNameToSpellId(string name)
        {
            var spellNameToSpellId = new Dictionary<string, string> {
                { "Cleanse", "SummonerBoost" },
                { "Exhaust", "SummonerExhaust" },
                { "Flash", "SummonerFlash" },
                { "Ghost", "SummonerHaste" },
                { "Heal", "SummonerHeal" },
                { "Smite", "SummonerSmite" },
                { "Teleport", "SummonerTeleport" },
                { "Clarity", "SummonerMana" },
                { "Ignite", "SummonerDot" },
                { "Barrier", "SummonerBarrier" },
                { "Mark", "SummonerSnowball" }
            };

            if (!spellNameToSpellId.ContainsKey(name))
                throw new ArgumentException($"Invalid spell name: {name}");

            return spellNameToSpellId[name];
        }
        public static string SpellIdToSpellName(string id)
        {
            var spellIdToSpellName = new Dictionary<string, string> {
                { "SummonerBoost", "Cleanse" },
                { "SummonerExhaust", "Exhaust" },
                { "SummonerFlash", "Flash" },
                { "SummonerHaste", "Ghost" },
                { "SummonerHeal", "Heal" },
                { "SummonerSmite", "Smite" },
                { "SummonerTeleport", "Teleport" },
                { "SummonerMana", "Clarity" },
                { "SummonerDot", "Ignite" },
                { "SummonerBarrier", "Barrier" },
                { "SummonerSnowball", "Mark" }
            };

            if (!spellIdToSpellName.ContainsKey(id))
                throw new ArgumentException($"Invalid spell id: {id}");

            return spellIdToSpellName[id];
        }

        //#region Shards
        public static string ShardDescriptionToShardAlias(string description)
        {
            var shardDescriptionToShardAlias = new Dictionary<string, string> {
                { "10% bonus attack speed", "axe" },
                { "8 bonus magic resistance", "circle" },
                { "5.4 bonus AD or 9 AP (Adaptive)", "diamond" },
                { "6 bonus armor", "shield" },
                { "8 ability haste", "time" },
                { "15 − 90 (based on level) bonus health", "heart" }
            };

            if (!shardDescriptionToShardAlias.ContainsKey(description))
                throw new ArgumentException($"Invalid shard description: {description}");

            return shardDescriptionToShardAlias[description];
        }

        public static string ShardIdToShardDescription(int id)
        {
            if(id != 0)
            {
                var shardIdToShardDescription = new Dictionary<int, string> {
                { 5005, "10% bonus attack speed" },
                { 5003, "8 bonus magic resistance" },
                { 5008, "5.4 bonus AD or 9 AP (Adaptive)" },
                { 5002, "6 bonus armor" },
                { 5007, "8 ability haste" },
                { 5001, "15 − 90 (based on level) bonus health" }
                };

                if (!shardIdToShardDescription.ContainsKey(id))
                    throw new ArgumentException($"Invalid shard id: {id}");

                return shardIdToShardDescription[id];
            }
            return null;
        }

        public static int ShardDescriptionToShardId(string description)
        {
            var shardDescriptionToShardId = new Dictionary<string, int> {
                { "10% bonus attack speed", 5005 },
                { "8 bonus magic resistance", 5003 },
                { "5.4 bonus AD or 9 AP (Adaptive)", 5008 },
                { "6 bonus armor", 5002 },
                { "8 ability haste", 5007 },
                { "15 − 90 (based on level) bonus health", 5001 }
                };

            if (!shardDescriptionToShardId.ContainsKey(description))
                return 0;

            return shardDescriptionToShardId[description];
        }

        public static string ShardIdToShardAlias(int id)
        {
            var shardIdToShardAlias = new Dictionary<int, string> {
                { 5005, "axe" },
                { 5003, "circle" },
                { 5008, "diamond" },
                { 5002, "shield" },
                { 5007, "time" },
                { 5001, "heart" }
            };

            if (!shardIdToShardAlias.ContainsKey(id))
                throw new ArgumentException($"Invalid shard id: {id}");

            return shardIdToShardAlias[id];
        }

        public static int ShardAliasToShardId(string alias)
        {
            var shardAliasToShardId = new Dictionary<string, int> {
                { "axe", 5005 },
                { "circle", 5003 },
                { "diamond", 5008 },
                { "shield", 5002 },
                { "time", 5007 },
                { "heart", 5001 }
            };

            if (!shardAliasToShardId.ContainsKey(alias))
                throw new ArgumentException($"Invalid shard alias: {alias}");

            return shardAliasToShardId[alias];
        }


        public static readonly Dictionary<string, string> s_SpellNameToSpellKey = new Dictionary<string, string>();
        public static string SpellNameToSpellKey(string name)
        {
            if (string.IsNullOrEmpty(name) || !s_SpellNameToSpellKey.ContainsKey(name))
                throw new ArgumentException($"Invalid spell Name: {name}");

            return s_SpellNameToSpellKey[name];
        }
        //#endregion
        public static void Init()
        {
            Log("Initializing Data Converters...", LogType.INFO);
            // Needed for rune editor
            s_SpellNameToSpellKey.Add("Barrier", "21");
            s_SpellNameToSpellKey.Add("Cleanse", "1");
            s_SpellNameToSpellKey.Add("Exhaust", "3");
            s_SpellNameToSpellKey.Add("Flash", "4");
            s_SpellNameToSpellKey.Add("Ghost", "6");
            s_SpellNameToSpellKey.Add("Heal", "7");
            s_SpellNameToSpellKey.Add("Smite", "11");
            s_SpellNameToSpellKey.Add("Teleport", "12");
            s_SpellNameToSpellKey.Add("Clarity", "13");
            s_SpellNameToSpellKey.Add("Ignite", "14");
            s_SpellNameToSpellKey.Add("Mark", "32");
        }
    }
}
