using System;

namespace LoLA.LCU
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
