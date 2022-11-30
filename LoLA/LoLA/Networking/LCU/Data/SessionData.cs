using LoLA.Networking.LCU.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Networking.LCU.Data
{
    public class SessionData
    {
        public GameMode GameMode { get; set; } = GameMode.NONE;
        public string Category { get; set; }
        public string Description { get; set; }
        public bool IsRanked { get; set; }
    }
}
