using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using LoLA.Data.Enums;
using LoLA.Data;
using System;

namespace LoLA.Networking.WebWrapper.DataProviders
{
    public interface IDataProvider
    {
         Task<ChampionBuild> FetchDataAsync(string championId, GameMode gameMode, Role role = Role.RECOMENDED);
    }
}
