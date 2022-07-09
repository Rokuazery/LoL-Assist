using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Networking.LCU.Enums
{
    public enum Phase
    {
        None,
        Lobby,
        Matchmaking,
        ReadyCheck,
        ChampSelect,
        InProgress,
        WaitingForStats,
        EndOfGame
    }
}
