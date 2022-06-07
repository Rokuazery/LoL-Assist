using System.Collections.Generic;
using System;

namespace LoLA.WebAPIs.DataDragon.Objects
{
    public class Perk
    {
        public int id { get; set; }
        public string key { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public List<Slots> slots = new List<Slots>();
    }

    public class Slots
    {
        public List<Rune> runes = new List<Rune>();
    }

    public class Rune
    {
        public int id { get; set; }
        public string key { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string shortDesc { get; set; }
        public string longDesc { get; set; }
    }
}
