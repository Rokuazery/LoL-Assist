using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LoLA.Utils.Logger;
using LoLA.LCU.Objects;
using Newtonsoft.Json;
using LoLA.Objects;
using System.Text;
using LoLA.Utils;
using System.Net;
using System.IO;
using System;

namespace LoLA.LCU
{
    public static class LCUWrapper
    {
        private static WebRequestExt s_webRequestEx;
        public static async Task<bool> InitAsync()
        {
            if (s_webRequestEx != null)
            {
                try
                {
                    HttpWebRequest request = s_webRequestEx.CreateRequest("/riotclient/ux-state");
                    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                    string empty = Misc.ReadStream(response.GetResponseStream()).Trim('"');
                    return true;
                }
                catch { s_webRequestEx = null; }
            }

            try
            {
                WebRequestExt webRequest = GetAuth();
                if (webRequest != null)
                {
                    s_webRequestEx = webRequest;
                    return true;
                }
            } catch { }

            return false;
        }

        private static WebRequestExt GetAuth()
        {
            string lolPath = LeagueClient.GetLocation();
            if (lolPath == null) return null;

            Dictionary<string, string> argsDict = GetArguments(lolPath);
            if (argsDict == null) return null;

            WebRequestExt authRequest = new WebRequestExt(argsDict["app-port"], argsDict["remote-auth-token"]);
            return authRequest;
        }

        private static Dictionary<string, string> GetArguments(string lolPath)
        {
            DirectoryInfo lolDirectoryInfo = new DirectoryInfo(lolPath);
            if (!lolDirectoryInfo.Exists)
                LogService.Log(LogService.Model("League of Legends directory not found", Global.name, LogType.WARN));

            string lockfilePath = Path.Combine(lolDirectoryInfo.FullName, "lockfile");
            if (!File.Exists(lockfilePath))
                LogService.Log(LogService.Model("Lockfile not found", Global.name, LogType.WARN));

            string lockfileContent;
            try
            {
                var fileStream = new FileStream(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                lockfileContent = Misc.ReadStream(fileStream);
            }
            catch { return null; }

            string[] parameters = lockfileContent.Split(':');

            Dictionary<string, string> argsDict = new Dictionary<string, string>
            {
                { "app-name", parameters[0] },
                { "app-pid", parameters[1] },
                { "app-port", parameters[2] },
                { "remote-auth-token", parameters[3] },
                { "remote-protocol", parameters[4] }
            };

            return argsDict;
        }

        public static async Task<Phase> GetGamePhaseAsync()
        {
            if (!await InitAsync())
                return Phase.None;

            try
            {
                string phase = (await GetDataRequestAsync("/lol-gameflow/v1/gameflow-phase"))?.Trim('"');
                return (Phase)Enum.Parse(typeof(Phase), phase);
            }
            catch { return Phase.None; }
        }

        public static async Task<List<RunePage>> GetRunePagesAsync()
        {
            if (!await InitAsync())
                return null;

            string result = await GetDataRequestAsync("/lol-perks/v1/pages");
            return await Converter.JsonToRunePagesAsync(result);
        }

        public static async Task<RunePage> GetCurrentRunePageAsync()
        {
            if (!await InitAsync())
                return null;

            string result = await GetDataRequestAsync("/lol-perks/v1/currentpage");
            return await Converter.JsonToRunePageAsync(result);
        }

        // Moved
        //public static async Task<bool> SetRune(RunePage Rune)
        //{
        //    var currentRunePage = await GetCurrentRunePageAsync();
        //    if (currentRunePage != null)
        //    {
        //        ulong selectedId = currentRunePage.id;
        //        if (currentRunePage.isDeletable)
        //            await DeleteRunePageAsync(selectedId);
        //    }
        //    if (!await AddRunePageAsync(Rune))
        //    {
        //        List<RunePage> pages = await GetRunePagesAsync();
        //        foreach (RunePage page in pages)
        //        {
        //            if (page.isDeletable && page.isActive)
        //            {
        //                Rune.order = 0;
        //                await DeleteRunePageAsync(page.id);
        //                return await AddRunePageAsync(Rune);
        //            }
        //        }
        //    }
        //    return false;
        //}

        public static async Task<bool> AddRunePageAsync(RunePage pageJson)
        {
            if (!await InitAsync())
                return false;

            byte[] data = Encoding.UTF8.GetBytes(await Converter.RunePageToJsonAsync(pageJson));
            return await SendDataRequestAsync("/lol-perks/v1/pages", "post", data);
        }

        public static async Task<bool> DeleteRunePageAsync(ulong id) => await GetResponseRequestAsync($"/lol-perks/v1/pages/{id}", "delete");
        public static async Task<bool> AcceptMatchmakingAsync() => await GetResponseRequestAsync("/lol-matchmaking/v1/ready-check/accept");
        public static async Task<bool> DeclineMatchmakingAsync() => await GetResponseRequestAsync("/lol-matchmaking/v1/ready-check/decline");


        public static async Task<GameMode> GetCurrentGameModeAsync()
        {
            if (!await InitAsync())
                return GameMode.NONE;

            try
            {
                string json = await GetDataRequestAsync("/lol-lobby/v2/lobby");
                JObject obj = JsonConvert.DeserializeObject<JObject>(json);
                JObject innerObj = obj["gameConfig"] as JObject;
                return (GameMode)Enum.Parse(typeof(GameMode), innerObj["gameMode"].ToString().ToUpper());
            }
            catch{ return GameMode.NONE;}
        }

        public static async Task<string> GetCurrentChampionAsync()
        {
            if (!await InitAsync())
                return null;

            return await GetDataRequestAsync("/lol-champ-select-legacy/v1/current-champion");
        }

        public static async Task<string> GetCurrentChampionAsyncV2()
        {
            if (!await InitAsync())
                return string.Empty;

            try
            {
                string json = await GetDataRequestAsync("/lol-champ-select/v1/skin-selector-info");
                JObject obj = JsonConvert.DeserializeObject<JObject>(json);
                return obj == null ? string.Empty : obj["championName"].Value<string>();
            }
            catch  { return string.Empty; }
        }

        public static async Task<Summoner> GetCurrentSummonerAsync()
        {
            if (!await InitAsync())
                return null;

            string result = await GetDataRequestAsync("/lol-summoner/v1/current-summoner");
            if (!string.IsNullOrEmpty(result))
                return await Converter.JsonToSummonerAsync(result);

            return new Summoner();
        }

        public static async Task<string> GetCurrentSessionAsync()
        {
            if (!await InitAsync())
                return null;

            return await GetDataRequestAsync("/lol-champ-select/v1/session");
        }   

        public static async Task<Matchmaking> GetMatchmakingInfo()
        {
            if (!await InitAsync())
                return null;

            if (await GetGamePhaseAsync() == Phase.ReadyCheck)
            {
                string json = await GetDataRequestAsync("/lol-matchmaking/v1/ready-check");
                return JsonConvert.DeserializeObject<Matchmaking>(json);
            }
            return null;
        }

        public static async Task<bool> SetSummonerSpells(SpellObj spells, GameMode gameMode = GameMode.CLASSIC)
        {
            if (!await InitAsync())
                return false;

            string urlRequest;
            var json = "{\n" + $"\"spell1Id\": {int.Parse(spells.Spell0)},\n\"spell2Id\": {int.Parse(spells.Spell1)}" + "\n}";

            if (gameMode == GameMode.ARAM || gameMode == GameMode.ARURF)
                urlRequest = "/lol-lobby-team-builder/champ-select/v1/session/my-selection";
            else
                urlRequest = "/lol-champ-select/v1/session/my-selection";

            byte[] data = Encoding.UTF8.GetBytes(json);
            return await SendDataRequestAsync(urlRequest, "patch", data);
        }

        public static async Task<string[]> GetCurrentSpells()
        {
            var currentSession = await GetCurrentSessionAsync();
            string[] spells = new string[2];

            if (!string.IsNullOrEmpty(currentSession))
            {
                JObject obj = JsonConvert.DeserializeObject<JObject>(currentSession);
                spells[0] = Dictionaries.SpellKeyToSpellName[(int)obj["myTeam"][0]["spell1Id"]];
                spells[1] = Dictionaries.SpellKeyToSpellName[(int)obj["myTeam"][0]["spell2Id"]];
                return spells;
            }
            return null;
        }

        #region Request
        private static async Task<bool> SendDataRequestAsync(string url, string method, byte[] data = null)
        {
            try
            {
                HttpWebRequest request = s_webRequestEx.CreateRequest(url);
                request.Method = method.ToUpper();

                Stream newStream = request.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();

                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.OK) return true;

                return false;
            }
            catch { return false; }
        }

        private static async Task<bool> GetResponseRequestAsync(string url, string method = "POST")
        {
            if (!await InitAsync())
                return false;

            try
            {
                HttpWebRequest request = s_webRequestEx.CreateRequest(url);
                request.Method = method.ToUpper();

                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

                switch (request.Method)
                {
                    case "POST":
                        if (response.StatusCode == HttpStatusCode.OK)
                            return true;
                        break;
                    case "DELETE":
                        if (response.StatusCode == HttpStatusCode.NoContent)
                            return true;
                        break;
                }
                return false;
            }
            catch { return false; }
        }

        private static async Task<string> GetDataRequestAsync(string url, string method = "GET") // return defined obj
        {
            try
            {
                HttpWebRequest request = s_webRequestEx.CreateRequest(url);
                request.Method = method.ToUpper();

                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                return Misc.ReadStream(response.GetResponseStream());
            }
            catch  { return null;  }
        }
        #endregion
    }
}
