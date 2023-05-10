namespace LoLA.Data
{
    public class Spell
    {
        public string First { get; set; }
        public string Second { get; set; }

        public Spell Clone()
        {
            return this.MemberwiseClone() as Spell;
        }
    }
}
