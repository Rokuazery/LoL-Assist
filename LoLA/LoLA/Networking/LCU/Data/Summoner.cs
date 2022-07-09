using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Networking.LCU.Objects
{
    public class Summoner
    {
        public ulong accountId { get; set; }
        public string displayName { get; set; }
        public string internalName { get; set; }
        public bool nameChangeFlag { get; set; }
        public double percentCompleteForNextLevel { get; set; }
        public uint profileIconId { get; set; }
        public string puuid { get; set; }
        public RerollPoints rerollPoints { get; set; }
        public ulong summonerId { get; set; }
        public int summonerLevel { get; set; }
        public bool unnamed { get; set; }
        public double xpSinceLastLevel { get; set; }
        public double xpSinceNextLevel { get; set; }

    }
    public class RerollPoints
    {
        public int currentPoints { get; set; }
        public int maxRolls { get; set; }
        public int numberOfRolls { get; set; }
        public int pointsCostToRoll { get; set; }
        public int pointsToReroll { get; set; }
    }
}
