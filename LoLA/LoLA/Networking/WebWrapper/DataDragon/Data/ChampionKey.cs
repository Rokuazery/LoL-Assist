using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Networking.WebWrapper.DataDragon.Data
{
    public class ChampionKey
    {
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public Image image = new Image();
        public string lore { get; set; }
        public string blurp { get; set; }
        public List<SpellInfo> spells = new List<SpellInfo>();
        public Passive passive = new Passive();
    }
}
