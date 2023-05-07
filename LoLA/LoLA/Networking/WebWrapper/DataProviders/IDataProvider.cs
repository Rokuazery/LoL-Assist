using LoLA.Data.Enums;
using LoLA.Data;
using LoLA.Networking.LCU.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLA.Networking.WebWrapper.DataProviders
{
    public interface IDataProvider
    {
         Task<ChampionBuild> FetchDataAsync(string championId, GameMode gameMode, Role role = Role.RECOMENDED);
    }
}
