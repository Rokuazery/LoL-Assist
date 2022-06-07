using LoLA.WebAPIs.DataDragon.Objects;
using System.Collections.Generic;
using LoLA.WebAPIs.DataDragon;
using LoLA.Objects;
using System;

namespace LoLA.LCU.Objects
{
    public static class DataConverter
    {
        #region perk
        public static int PerkNameToId(string PerkName)
        {
            foreach (var perk in DataDragonWrapper.perks)
            {
                foreach (var slot in perk.slots)
                {
                    foreach (var rune in slot.runes)
                    {
                        if (PerkName.ToLower() == rune.name.ToLower())
                            return rune.id;
                    }
                }

            }
            return 0;
        }

        public static string PerkIdToName(int PerkID)
        {
            foreach (var perk in DataDragonWrapper.perks)
            {
                foreach (var slot in perk.slots)
                {
                    foreach (var rune in slot.runes)
                    {
                        if (PerkID == rune.id)
                            return rune.name;
                    }
                }

            }
            return null;
        }

        #region path
        public static int PathNameToId(string PerkName)
        {
            foreach (var item in DataDragonWrapper.perks)
            {
                if (PerkName == item.name)
                    return item.id;
            }
            return 0;
        }

        public static string PathIdToName(int PerkID)
        {
            foreach (var item in DataDragonWrapper.perks)
            {
                if (PerkID == item.id)
                    return item.name;
            }
            return null;
        }
        #endregion

        #region shard
        public static int ShardNameToId(string ShardName)
        {
            switch (ShardName)
            {
                case "axe":
                    return 5005;
                case "circle":
                    return 5003;
                case "diamond":
                    return 5008;
                case "shield":
                    return 5002;
                case "time":
                    return 5007;
                case "heart":
                    return 5001;
            }
            return 0;
        }

        public static int ShardNameToIdV2(string ShardName)
        {
            switch (ShardName)
            {
                case "AttackSpeed":
                    return 5005;
                case "MagicRes":
                    return 5003;
                case "Adaptive":
                    return 5008;
                case "Armor":
                    return 5002;
                case "CDRScaling":
                    return 5007;
                case "HealthScaling":
                    return 5001;
            }
            return 0;
        }

        public static string ShardIdToName(int ShardId)
        {
            switch (ShardId)
            {
                case 5005:
                    return "axe";
                case 5003:
                    return "circle";
                case 5008:
                    return "diamond";
                case 5002:
                    return "shield";
                case 5007:
                    return "time";
                case 5001:
                    return "heart";
            }
            return null;
        }
        #endregion


        public static RunePage RuneBuildToRunePage(RuneObj runeBuild)
        {
            var runePage = new RunePage
            {
                name = runeBuild.Name,
                primaryStyleId = runeBuild.Path0,
                subStyleId = runeBuild.Path1,
                selectedPerkIds = new List<int> { runeBuild.Keystone
                , runeBuild.Slot1
                , runeBuild.Slot2
                , runeBuild.Slot3
                , runeBuild.Slot4
                , runeBuild.Slot5
                , runeBuild.Shard0
                , runeBuild.Shard1
                , runeBuild.Shard2 }
            };

            return runePage;
        }
        #endregion

        #region champion
        public static string ChampionIdToName(string ChampionID)
        {
            foreach (var data in DataDragonWrapper.Champions.Data.Values)
            {
                if (ChampionID == data.id)
                    return data.name;
            }
            return null;
        }

        public static string ChampionNameToId(string ChampionName)
        {
            foreach (var data in DataDragonWrapper.Champions.Data.Values)
            {
                if (ChampionName == data.name)
                    return data.id;
            }
            return null;
        }

        public static string ChampionKeyToId(string ChampionKey)
        {
            foreach (var data in DataDragonWrapper.Champions.Data.Values)
            {
                if (ChampionKey == data.key)
                    return data.id;
            }
            return null;
        }


        public static string ChampionKeyToName(string ChampionKey)
        {
            foreach (var data in DataDragonWrapper.Champions.Data.Values)
            {
                if (ChampionKey == data.key)
                    return data.name;
            }
            return null;
        }

        public static string ChampionNameToKey(string ChampionName)
        {
            foreach (var data in DataDragonWrapper.Champions.Data.Values)
            {
                if (ChampionName == data.name)
                    return data.key;
            }
            return null;
        }

        public static string ChampionIdToKey(string ChampionID)
        {
            foreach (var data in DataDragonWrapper.Champions.Data.Values)
            {
                if (ChampionID == data.id)
                    return data.key;
            }
            return null;
        }
        #endregion
    }
}
