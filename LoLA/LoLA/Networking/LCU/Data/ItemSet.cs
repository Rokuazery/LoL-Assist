using System;

namespace LoLA.Networking.LCU.Data
{

    public class ItemSet
    {
        public int[] associatedChampions { get; set; }
        public int[] associatedMaps { get; set; }
        public Block[] blocks { get; set; }
        public string map { get; set; }
        public string mode { get; set; }
        public Preferreditemslot[] preferredItemSlots { get; set; }
        public int sortrank { get; set; }
        public string startedFrom { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string uid { get; set; }
    }

    public class Block
    {
        public string hideIfSummonerSpell { get; set; }
        public Item[] items { get; set; }
        public string showIfSummonerSpell { get; set; }
        public string type { get; set; }
    }

    public class Item
    {
        public int count { get; set; }
        public string id { get; set; }
    }

    public class Preferreditemslot
    {
        public string id { get; set; }
        public int preferredItemSlot { get; set; }
    }

}
