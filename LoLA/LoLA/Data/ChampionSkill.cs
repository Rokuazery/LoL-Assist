using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Data
{
    public class ChampionSkill
    {
        public ChampSkill[] Order { get; set; }
        public string Priority { get; set; }
    }

    public class ChampSkill
    {
        public int Index { get; set; }
        public string Skill { get; set; }
    }
}
