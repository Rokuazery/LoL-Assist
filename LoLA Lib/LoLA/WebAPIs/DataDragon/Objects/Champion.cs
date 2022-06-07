using System.Collections.Generic;
using System;


namespace LoLA.WebAPIs.DataDragon.Objects
{
    public class Champion
    {
        public string type { get; set; }
        public string format { get; set; }
        public string version { get; set; }
        public Dictionary<string, ChampionKey> data { get; set; }
    }

    public class ChampionKey
    {
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public Image image = new Image();
        public string lore { get; set; }
        public string blurp { get; set; }
        public List<Spell> spells = new List<Spell>();
        public Passive passive = new Passive();
    }

    public class ChampionImage
    {
        public string full { get; set; }
        public string sprite { get; set; }
        public string group { get; set; }
    }
    public class Spell
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string tooltip { get; set; }
        public Image image = new Image();
    }

    public class Passive
    {
        public string name { get; set; }
        public string description { get; set; }
        public Image image = new Image();
    }
}
