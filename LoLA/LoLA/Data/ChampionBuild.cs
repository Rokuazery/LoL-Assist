using System.Collections.Generic;

namespace LoLA.Data
{
    public class ChampionBuild
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Role { get; set; } = "fill";

        public List<Rune> Runes = new List<Rune>();
        public List<Spell> Spells = new List<Spell>();
    }
}
