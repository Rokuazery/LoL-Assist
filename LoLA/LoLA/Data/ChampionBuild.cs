namespace LoLA.Data
{
    public class ChampionBuild
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Role { get; set; } = "fill";

        public Rune Rune = new Rune();
        public Spell Spell = new Spell();
    }
}
