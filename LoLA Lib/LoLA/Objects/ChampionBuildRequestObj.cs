using LoLA.LCU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Objects
{
    public class ChampionBuildRequestObj
    {
        public string ChampionId { get; set; }
        public string ChampionName { get; set; }
        public GameMode GameMode { get; set; }
        public string Role { get; set; }
    }
}
