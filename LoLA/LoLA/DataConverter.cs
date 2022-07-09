using static LoLA.Utils.Logger.LogService;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoLA.Utils.Logger;

namespace LoLA
{
    public static class DataConverter
    {
        public static Dictionary<string, int> s_SpellNameToSpellKey = new Dictionary<string, int>();

        public static int SpellIdToSpellKey(string id)
        {
            var spellIdToSpellKey = new Dictionary<string, int> {
                { "SummonerBarrier", 0 },
                { "SummonerBoost", 1 },
                { "SummonerExhaust", 3 },
                { "SummonerFlash", 4 },
                { "SummonerHaste", 6 },
                { "SummonerHeal", 7 },
                { "SummonerSmite", 11 },
                { "SummonerTeleport", 12 },
                { "SummonerMana", 13 },
                { "SummonerDot", 14 },
                { "SummonerSnowball", 32 }
            };
            return spellIdToSpellKey[id];
        }

        public static string SpellKeyToSpellId(int key)
        {
            var spellKeyToSpellName = new Dictionary<int, string> {
                { 0, "SummonerBarrier" },
                { 1, "SummonerBoost" },
                { 3, "SummonerExhaust" },
                { 4, "SummonerFlash" },
                { 6, "SummonerHaste" },
                { 7, "SummonerHeal" },
                { 11, "SummonerSmite" },
                { 12, "SummonerTeleport" },
                { 13, "SummonerMana" },
                { 14, "SummonerDot" },
                { 32, "SummonerSnowball" }
            };
            //SpellKeyToSpellName.Add(30, "To the King!");
            //SpellKeyToSpellName.Add(31, "Poro Toss");
            //SpellKeyToSpellName.Add(33, "Nexus Siege: Siege Weapon Slot1");
            //SpellKeyToSpellName.Add(34, "Nexus Siege: Siege Weapon Slot2");
            //SpellKeyToSpellName.Add(35, "Disabled Summoner Spells1");
            //SpellKeyToSpellName.Add(36, "Disabled Summoner Spells2");
            //SpellKeyToSpellName.Add(39, "Ultra (Rapidly Flung) Mark");
            return spellKeyToSpellName[key];
        }

        public static string SpellKeyToSpellName(int key)
        {
            var spellKeyToSpellName = new Dictionary<int, string> {
                { 0, "Barrier" },
                { 1, "Cleanse" },
                { 3, "Exhaust" },
                { 4, "Flash" },
                { 6, "Ghost" },
                { 7, "Heal" },
                { 11, "Smite" },
                { 12, "Teleport" },
                { 13, "Clarity" },
                { 14, "Ignite" },
                { 32, "Mark" }
            };
            //SpellKeyToSpellName.Add(30, "To the King!");
            //SpellKeyToSpellName.Add(31, "Poro Toss");
            //SpellKeyToSpellName.Add(33, "Nexus Siege: Siege Weapon Slot1");
            //SpellKeyToSpellName.Add(34, "Nexus Siege: Siege Weapon Slot2");
            //SpellKeyToSpellName.Add(35, "Disabled Summoner Spells1");
            //SpellKeyToSpellName.Add(36, "Disabled Summoner Spells2");
            //SpellKeyToSpellName.Add(39, "Ultra (Rapidly Flung) Mark");
            return spellKeyToSpellName[key];
        }

        public static int SpellNameToSpellKey(string name) => s_SpellNameToSpellKey[name];

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

                return shardIdToShardDescription[id];
            }
            return null;
        }

        public static int ShardDescriptionToShardId(string description)
        {
            if(!string.IsNullOrEmpty(description))
            {
                var shardDescriptionToShardId = new Dictionary<string, int> {
                { "10% bonus attack speed", 5005 },
                { "8 bonus magic resistance", 5003 },
                { "5.4 bonus AD or 9 AP (Adaptive)", 5008 },
                { "6 bonus armor", 5002 },
                { "8 ability haste", 5007 },
                { "15 − 90 (based on level) bonus health", 5001 }
                };

                return shardDescriptionToShardId[description];
            }
            return 0;
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

            return shardAliasToShardId[alias];
        }


        //#endregion
        public static void Init()
        {
            Log("Initializing Data Converters...", LogType.INFO);

            //    await Task.Run(() =>
            //    {
            //        s_SpellKeyToSpellName.Add(0, "Barrier");
            //        s_SpellKeyToSpellName.Add(1, "Cleanse");
            //        s_SpellKeyToSpellName.Add(3, "Exhaust");
            //        s_SpellKeyToSpellName.Add(4, "Flash");
            //        s_SpellKeyToSpellName.Add(6, "Ghost");
            //        s_SpellKeyToSpellName.Add(7, "Heal");
            //        s_SpellKeyToSpellName.Add(11, "Smite");
            //        s_SpellKeyToSpellName.Add(12, "Teleport");
            //        s_SpellKeyToSpellName.Add(13, "Clarity");
            //        s_SpellKeyToSpellName.Add(14, "Ignite");
            //        //SpellKeyToSpellName.Add(30, "To the King!");
            //        //SpellKeyToSpellName.Add(31, "Poro Toss");
            //        s_SpellKeyToSpellName.Add(32, "Mark");
            //        //SpellKeyToSpellName.Add(33, "Nexus Siege: Siege Weapon Slot1");
            //        //SpellKeyToSpellName.Add(34, "Nexus Siege: Siege Weapon Slot2");
            //        //SpellKeyToSpellName.Add(35, "Disabled Summoner Spells1");
            //        //SpellKeyToSpellName.Add(36, "Disabled Summoner Spells2");
            //        //SpellKeyToSpellName.Add(39, "Ultra (Rapidly Flung) Mark");

            s_SpellNameToSpellKey.Add("Barrier", 0);
                s_SpellNameToSpellKey.Add("Cleanse", 1);
                s_SpellNameToSpellKey.Add("Exhaust", 3);
                s_SpellNameToSpellKey.Add("Flash", 4);
                s_SpellNameToSpellKey.Add("Ghost", 6);
                s_SpellNameToSpellKey.Add("Heal", 7);
                s_SpellNameToSpellKey.Add("Smite", 11);
                s_SpellNameToSpellKey.Add("Teleport", 12);
                s_SpellNameToSpellKey.Add("Clarity", 13);
                s_SpellNameToSpellKey.Add("Ignite", 14);
                //SpellNameToSpellKey.Add("To the King!", 30);
                //SpellNameToSpellKey.Add("Poro Toss", 31);
                s_SpellNameToSpellKey.Add("Mark", 32);
                //SpellNameToSpellKey.Add("Nexus Siege: Siege Weapon Slot1", 33);
                //SpellNameToSpellKey.Add("Nexus Siege: Siege Weapon Slot2", 34);
                //SpellNameToSpellKey.Add("Disabled Summoner Spells1", 35);
                //SpellNameToSpellKey.Add("Disabled Summoner Spells2", 36);
                //SpellNameToSpellKey.Add("Ultra (Rapidly Flung) Mark", 39);

        //        s_SpellIdToSpellKey.Add("SummonerBarrier", 0);
        //        s_SpellIdToSpellKey.Add("SummonerBoost", 1);
        //        s_SpellIdToSpellKey.Add("SummonerExhaust", 3);
        //        s_SpellIdToSpellKey.Add("SummonerFlash", 4);
        //        s_SpellIdToSpellKey.Add("SummonerHaste", 6);
        //        s_SpellIdToSpellKey.Add("SummonerHeal", 7);
        //        s_SpellIdToSpellKey.Add("SummonerSmite", 11);
        //        s_SpellIdToSpellKey.Add("SummonerTeleport", 12);
        //        s_SpellIdToSpellKey.Add("SummonerMana", 13);
        //        s_SpellIdToSpellKey.Add("SummonerDot", 14);
        //        s_SpellIdToSpellKey.Add("SummonerSnowball", 32);

        //        s_SpellNameToSpellId.Add("Cleanse", "SummonerBoost");
        //        s_SpellNameToSpellId.Add("Exhaust", "SummonerExhaust");
        //        s_SpellNameToSpellId.Add("Flash", "SummonerFlash");
        //        s_SpellNameToSpellId.Add("Ghost", "SummonerHaste");
        //        s_SpellNameToSpellId.Add("Heal", "SummonerHeal");
        //        s_SpellNameToSpellId.Add("Smite", "SummonerSmite");
        //        s_SpellNameToSpellId.Add("Teleport", "SummonerTeleport");
        //        s_SpellNameToSpellId.Add("Clarity", "SummonerMana");
        //        s_SpellNameToSpellId.Add("Ignite", "SummonerDot");
        //        s_SpellNameToSpellId.Add("Barrier", "SummonerBarrier");
        //        s_SpellNameToSpellId.Add("Mark", "SummonerSnowball");

        //        s_SpellIDToSpellName.Add("SummonerBoost", "Cleanse");
        //        s_SpellIDToSpellName.Add("SummonerExhaust", "Exhaust");
        //        s_SpellIDToSpellName.Add("SummonerFlash", "Flash");
        //        s_SpellIDToSpellName.Add("SummonerHaste", "Ghost");
        //        s_SpellIDToSpellName.Add("SummonerHeal", "Heal");
        //        s_SpellIDToSpellName.Add("SummonerSmite", "Smite");
        //        s_SpellIDToSpellName.Add("SummonerTeleport", "Teleport");
        //        s_SpellIDToSpellName.Add("SummonerMana", "Clarity");
        //        s_SpellIDToSpellName.Add("SummonerDot", "Ignite");
        //        s_SpellIDToSpellName.Add("SummonerBarrier", "Barrier");
        //        s_SpellIDToSpellName.Add("SummonerSnowball", "Mark");

        //        //DescriptionToShard.Add("10% bonus attack speed", "axe");
        //        //DescriptionToShard.Add("8 bonus magic resistance", "circle");
        //        //DescriptionToShard.Add("5.4 bonus Attack Damage or 9 Ability Power (Adaptive)", "diamond");
        //        //DescriptionToShard.Add("6 bonus armor", "shield");
        //        //DescriptionToShard.Add("8 ability haste", "time");
        //        //DescriptionToShard.Add("15 − 90 (based on level) bonus health", "heart");

        //        s_ShardAliasToShardId.Add("axe", 5005);
        //        s_ShardAliasToShardId.Add("circle", 5003);
        //        s_ShardAliasToShardId.Add("diamond", 5008);
        //        s_ShardAliasToShardId.Add("shield", 5002);
        //        s_ShardAliasToShardId.Add("time", 5007);
        //        s_ShardAliasToShardId.Add("heart", 5001);

        //        s_ShardIdToShardAlias.Add(5005, "axe");
        //        s_ShardIdToShardAlias.Add(5003, "circle");
        //        s_ShardIdToShardAlias.Add(5008, "diamond");
        //        s_ShardIdToShardAlias.Add(5002, "shield");
        //        s_ShardIdToShardAlias.Add(5007, "time");
        //        s_ShardIdToShardAlias.Add(5001, "heart");

        //        s_ShardDescriptionToShardAlias.Add("10% bonus attack speed", "axe");
        //        s_ShardDescriptionToShardAlias.Add("8 bonus magic resistance", "circle");
        //        s_ShardDescriptionToShardAlias.Add("5.4 bonus AD or 9 AP (Adaptive)", "diamond");
        //        s_ShardDescriptionToShardAlias.Add("6 bonus armor", "shield");
        //        s_ShardDescriptionToShardAlias.Add("8 ability haste", "time");
        //        s_ShardDescriptionToShardAlias.Add("15 − 90 (based on level) bonus health", "heart");

        //        s_ShardDescriptionToShardId.Add("10% bonus attack speed", 5005);
        //        s_ShardDescriptionToShardId.Add("8 bonus magic resistance", 5003);
        //        s_ShardDescriptionToShardId.Add("5.4 bonus AD or 9 AP (Adaptive)", 5008);
        //        s_ShardDescriptionToShardId.Add("6 bonus armor", 5002);
        //        s_ShardDescriptionToShardId.Add("8 ability haste", 5007);
        //        s_ShardDescriptionToShardId.Add("15 − 90 (based on level) bonus health", 5001);

        //        s_ShardIdToShardDescription.Add(5005, "10% bonus attack speed");
        //        s_ShardIdToShardDescription.Add(5003, "8 bonus magic resistance");
        //        s_ShardIdToShardDescription.Add(5008, "5.4 bonus AD or 9 AP (Adaptive)");
        //        s_ShardIdToShardDescription.Add(5002, "6 bonus armor");
        //        s_ShardIdToShardDescription.Add(5007, "8 ability haste");
        //        s_ShardIdToShardDescription.Add(5001, "15 − 90 (based on level) bonus health");
        //    });
        }
    }
}
