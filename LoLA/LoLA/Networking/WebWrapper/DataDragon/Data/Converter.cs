using LoLA.Networking.LCU.Objects;
using System.Collections.Generic;
using LoLA.Utils;
using LoLA.Data;

namespace LoLA.Networking.WebWrapper.DataDragon.Data
{
    public class Converter
    {
        #region perk

        public static string GetPerkDescriptionByName(string perkName)
        {
            foreach (var perk in DataDragonWrapper.s_Perks)
            {
                foreach (var slot in perk.slots)
                {
                    foreach (var rune in slot.runes)
                    {
                        if (perkName.ToLower() == rune.name.ToLower())
                            return Misc.StripHTML(rune.shortDesc);
                    }
                }
            }
            return null;
        }

        public static string GetPerkDescriptionById(int perkId)
        {
            foreach (var perk in DataDragonWrapper.s_Perks)
            {
                foreach (var slot in perk.slots)
                {
                    foreach (var rune in slot.runes)
                    {
                        if (perkId == rune.id)
                            return Misc.StripHTML(rune.shortDesc);
                    }
                }
            }
            return null;
        }

        public static int PerkNameToId(string perkName)
        {
            foreach (var perk in DataDragonWrapper.s_Perks)
            {
                foreach (var slot in perk.slots)
                {
                    foreach (var rune in slot.runes)
                    {
                        if (perkName.ToLower() == rune.name.ToLower())
                            return rune.id;
                    }
                }
            }
            return 0;
        }

        public static string PerkIdToName(int perkId)
        {
            foreach (var perk in DataDragonWrapper.s_Perks)
            {
                foreach (var slot in perk.slots)
                {
                    foreach (var rune in slot.runes)
                    {
                        if (perkId == rune.id)
                            return rune.name;
                    }
                }

            }
            return null;
        }

        #region path
        public static int PathNameToId(string perkName) 
            => (int)DataDragonWrapper.s_Perks.Find(match => match.name == perkName)?.id;

        public static string PathIdToName(int perkId)
            => DataDragonWrapper.s_Perks.Find(match => match.id == perkId)?.name;

        #endregion

        #region shard
        public static int ShardNameToId(string shardName)
            => shardName switch
            {
                "axe" => 5005,
                "circle" => 5003,
                "diamond" => 5008,
                "shield" => 5002,
                "time" => 5007,
                "heart" => 5001,
                _ => 0
            };

        public static int ShardNameToIdV2(string shardName)
            => shardName switch
            {
                "AttackSpeed" => 5005,
                "MagicRes" => 5003,
                "Adaptive" => 5008,
                "Armor" => 5002,
                "CDRScaling" => 5007,
                "HealthScaling" => 5001,
                _ => 0
            };

        public static string ShardIdToName(int shardId)
            => shardId switch
            {
                5005 => "axe",
                5003 => "circle",
                5008 => "diamond",
                5002 => "shield",
                5007 => "time",
                5001 => "heart",
                _ => null
            };
        #endregion

        public static RunePage RuneToRunePage(Rune rune)
        {
            var runePage = new RunePage
            {
                name = rune.Name,
                primaryStyleId = rune.PrimaryPath,
                subStyleId = rune.SecondaryPath,
                selectedPerkIds = new List<int> {
                  rune.Keystone
                , rune.Slot1
                , rune.Slot2
                , rune.Slot3
                , rune.Slot4
                , rune.Slot5
                , rune.Shard1
                , rune.Shard2
                , rune.Shard3 }
            };

            return runePage;
        }

        public static Rune RunePageToRune(RunePage runePage)
        {
            var rune = new Rune
            {
                Name = runePage.name,
                PrimaryPath = runePage.primaryStyleId,
                SecondaryPath = runePage.subStyleId,
                Keystone = runePage.selectedPerkIds[0],
                Slot1 = runePage.selectedPerkIds[1],
                Slot2 = runePage.selectedPerkIds[2],
                Slot3 = runePage.selectedPerkIds[3],
                Slot4 = runePage.selectedPerkIds[4],
                Slot5 = runePage.selectedPerkIds[5],
                Shard1 = runePage.selectedPerkIds[6],
                Shard2 = runePage.selectedPerkIds[7],
                Shard3 = runePage.selectedPerkIds[8]
            };

            return rune;
        }
        #endregion

        #region champion
        public static string ChampionIdToName(string championId)
        {
            foreach (var data in DataDragonWrapper.s_Champions.Data.Values)
            {
                if (championId == data.id)
                    return data.name;
            }
            return null;
        }

        public static string ChampionNameToId(string championName)
        {
            foreach (var data in DataDragonWrapper.s_Champions.Data.Values)
            {
                if (championName == data.name)
                    return data.id;
            }
            return null;
        }

        public static string ChampionKeyToId(string championKey)
        {
            foreach (var data in DataDragonWrapper.s_Champions.Data.Values)
            {
                if (championKey == data.key)
                    return data.id;
            }
            return null;
        }

        public static string ChampionKeyToName(string championKey)
        {
            foreach (var data in DataDragonWrapper.s_Champions.Data.Values)
            {
                if (championKey == data.key)
                    return data.name;
            }
            return null;
        }

        public static string ChampionNameToKey(string championName)
        {
            foreach (var data in DataDragonWrapper.s_Champions.Data.Values)
            {
                if (championName == data.name)
                    return data.key;
            }
            return null;
        }

        public static string ChampionIdToKey(string championId)
        {
            foreach (var data in DataDragonWrapper.s_Champions.Data.Values)
            {
                if (championId == data.id)
                    return data.key;
            }
            return null;
        }
        #endregion
    }
}
