using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace LoLA.LCU.Objects
{
    public static class Converter
    {
        #region Json to Object
        public static async Task<RunePage> JsonToRunePageAsync(string json)
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<RunePage>(json));
        }

        public static async Task<List<RunePage>> JsonToRunePagesAsync(string json)
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<List<RunePage>>(json));
        }

        public static async Task<Summoner> JsonToSummonerAsync(string json)
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<Summoner>(json));
        }
        #endregion

        #region Object to Json
        public static async Task<string> RunePageToJsonAsync(RunePage runePage)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(runePage));
        }

        public static async Task<string> RunePagesToJsonAsync(List<RunePage> runePages)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(runePages));
        }

        public static async Task<string> SummonerToJsonAsync(Summoner summoner)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(summoner));
        }


        #endregion
    }
}
