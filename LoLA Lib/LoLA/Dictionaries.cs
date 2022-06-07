using System.Collections.Generic;
using System.Threading.Tasks;
using LoLA.Utils.Logger;

namespace LoLA
{
    public static class Dictionaries
    {
        public static Dictionary<string, int> SpellIdToSpellKey = new Dictionary<string, int>();
        public static Dictionary<int, string> SpellKeyToSpellName = new Dictionary<int, string>();
        public static Dictionary<string, int> SpellNameToSpellKey = new Dictionary<string, int>();
        public static Dictionary<string, string> SpellNameToSpellID = new Dictionary<string, string>();
        public static Dictionary<string, string> SpellIDToSpellName = new Dictionary<string, string>();
        public static Dictionary<string, string> SpellIdToSpellName = new Dictionary<string, string>();

        public static Dictionary<int, string> ShardIdToShardDescription = new Dictionary<int, string>();
        public static Dictionary<int, string> ShardIdToShardAlias = new Dictionary<int, string>();
        public static Dictionary<string, int> ShardAliasToShardId = new Dictionary<string, int>();
        public static Dictionary<string, int> ShardDescToShardId = new Dictionary<string, int>();

        public static async Task DcsInitAsync()
        {
            LogService.Log(LogService.Model("Initializing Dictionaries...", Global.name, LogType.INFO));

            await Task.Run(() => {
                SpellKeyToSpellName.Add(0, "Barrier");
                SpellKeyToSpellName.Add(1, "Cleanse");
                SpellKeyToSpellName.Add(3, "Exhaust");
                SpellKeyToSpellName.Add(4, "Flash");
                SpellKeyToSpellName.Add(6, "Ghost");
                SpellKeyToSpellName.Add(7, "Heal");
                SpellKeyToSpellName.Add(11, "Smite");
                SpellKeyToSpellName.Add(12, "Teleport");
                SpellKeyToSpellName.Add(13, "Clarity");
                SpellKeyToSpellName.Add(14, "Ignite");
                //SpellKeyToSpellName.Add(30, "To the King!");
                //SpellKeyToSpellName.Add(31, "Poro Toss");
                SpellKeyToSpellName.Add(32, "Mark");
                //SpellKeyToSpellName.Add(33, "Nexus Siege: Siege Weapon Slot1");
                //SpellKeyToSpellName.Add(34, "Nexus Siege: Siege Weapon Slot2");
                //SpellKeyToSpellName.Add(35, "Disabled Summoner Spells1");
                //SpellKeyToSpellName.Add(36, "Disabled Summoner Spells2");
                //SpellKeyToSpellName.Add(39, "Ultra (Rapidly Flung) Mark");

                SpellNameToSpellKey.Add("Barrier", 0);
                SpellNameToSpellKey.Add("Cleanse", 1);
                SpellNameToSpellKey.Add("Exhaust", 3);
                SpellNameToSpellKey.Add("Flash", 4);
                SpellNameToSpellKey.Add("Ghost", 6);
                SpellNameToSpellKey.Add("Heal", 7);
                SpellNameToSpellKey.Add("Smite", 11);
                SpellNameToSpellKey.Add("Teleport", 12);
                SpellNameToSpellKey.Add("Clarity", 13);
                SpellNameToSpellKey.Add("Ignite", 14);
                //SpellNameToSpellKey.Add("To the King!", 30);
                //SpellNameToSpellKey.Add("Poro Toss", 31);
                SpellNameToSpellKey.Add("Mark", 32);
                //SpellNameToSpellKey.Add("Nexus Siege: Siege Weapon Slot1", 33);
                //SpellNameToSpellKey.Add("Nexus Siege: Siege Weapon Slot2", 34);
                //SpellNameToSpellKey.Add("Disabled Summoner Spells1", 35);
                //SpellNameToSpellKey.Add("Disabled Summoner Spells2", 36);
                //SpellNameToSpellKey.Add("Ultra (Rapidly Flung) Mark", 39);

                SpellIdToSpellKey.Add("SummonerBarrier", 0);
                SpellIdToSpellKey.Add("SummonerBoost", 1);
                SpellIdToSpellKey.Add("SummonerExhaust", 3);
                SpellIdToSpellKey.Add("SummonerFlash", 4);
                SpellIdToSpellKey.Add("SummonerHaste", 6);
                SpellIdToSpellKey.Add("SummonerHeal", 7);
                SpellIdToSpellKey.Add("SummonerSmite", 11);
                SpellIdToSpellKey.Add("SummonerTeleport", 12);
                SpellIdToSpellKey.Add("SummonerMana", 13);
                SpellIdToSpellKey.Add("SummonerDot", 14);
                SpellIdToSpellKey.Add("SummonerSnowball", 32);

                SpellNameToSpellID.Add("Cleanse", "SummonerBoost");
                SpellNameToSpellID.Add("Exhaust", "SummonerExhaust");
                SpellNameToSpellID.Add("Flash", "SummonerFlash");
                SpellNameToSpellID.Add("Ghost", "SummonerHaste");
                SpellNameToSpellID.Add("Heal", "SummonerHeal");
                SpellNameToSpellID.Add("Smite", "SummonerSmite");
                SpellNameToSpellID.Add("Teleport", "SummonerTeleport");
                SpellNameToSpellID.Add("Clarity", "SummonerMana");
                SpellNameToSpellID.Add("Ignite", "SummonerDot");
                SpellNameToSpellID.Add("Barrier", "SummonerBarrier");
                SpellNameToSpellID.Add("Mark", "SummonerSnowball");

                SpellIDToSpellName.Add("SummonerBoost", "Cleanse");
                SpellIDToSpellName.Add("SummonerExhaust", "Exhaust");
                SpellIDToSpellName.Add("SummonerFlash", "Flash");
                SpellIDToSpellName.Add("SummonerHaste", "Ghost");
                SpellIDToSpellName.Add("SummonerHeal", "Heal");
                SpellIDToSpellName.Add("SummonerSmite", "Smite");
                SpellIDToSpellName.Add("SummonerTeleport", "Teleport");
                SpellIDToSpellName.Add("SummonerMana", "Clarity");
                SpellIDToSpellName.Add("SummonerDot", "Ignite");
                SpellIDToSpellName.Add("SummonerBarrier", "Barrier");
                SpellIDToSpellName.Add("SummonerSnowball", "Mark");

                //DescriptionToShard.Add("10% bonus attack speed", "axe");
                //DescriptionToShard.Add("8 bonus magic resistance", "circle");
                //DescriptionToShard.Add("5.4 bonus Attack Damage or 9 Ability Power (Adaptive)", "diamond");
                //DescriptionToShard.Add("6 bonus armor", "shield");
                //DescriptionToShard.Add("8 ability haste", "time");
                //DescriptionToShard.Add("15 − 90 (based on level) bonus health", "heart");

                ShardAliasToShardId.Add("axe", 5005);
                ShardAliasToShardId.Add("circle", 5003);
                ShardAliasToShardId.Add("diamond", 5008);
                ShardAliasToShardId.Add("shield", 5002);
                ShardAliasToShardId.Add("time", 5007);
                ShardAliasToShardId.Add("heart", 5001);

                ShardIdToShardAlias.Add(5005, "axe");
                ShardIdToShardAlias.Add(5003, "circle");
                ShardIdToShardAlias.Add(5008, "diamond");
                ShardIdToShardAlias.Add(5002, "shield");
                ShardIdToShardAlias.Add(5007, "time");
                ShardIdToShardAlias.Add(5001, "heart");

                ShardDescToShardId.Add("10% bonus attack speed", 5005);
                ShardDescToShardId.Add("8 bonus magic resistance", 5003);
                ShardDescToShardId.Add("5.4 bonus Attack Damage or 9 Ability Power (Adaptive)", 5008);
                ShardDescToShardId.Add("6 bonus armor", 5002);
                ShardDescToShardId.Add("8 ability haste", 5007);
                ShardDescToShardId.Add("15 − 90 (based on level) bonus health", 5001);

                ShardIdToShardDescription.Add(5005, "10% bonus attack speed");
                ShardIdToShardDescription.Add(5003, "8 bonus magic resistance");
                ShardIdToShardDescription.Add(5008, "5.4 bonus Attack Damage or 9 Ability Power (Adaptive)");
                ShardIdToShardDescription.Add(5002, "6 bonus armor");
                ShardIdToShardDescription.Add(5007, "8 ability haste");
                ShardIdToShardDescription.Add(5001, "15 − 90 (based on level) bonus health");
            });
        }
    }
}
