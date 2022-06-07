using LoLA.LCU;
using LoLA.WebAPIs.DataDragon.Objects;
using System;

namespace LoLA.Objects
{
    public class ChampionBD
    {
        public string name { get; set; }
        public string id { get; set; }
        public string role { get; set; } = "fill";

        public RuneObj rune = new RuneObj();
        public SpellObj spell = new SpellObj();
        public string[] skillOrder = new string[3];
    }
}
