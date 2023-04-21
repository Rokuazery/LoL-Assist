using static LoLA.Utils.Logger.LogService;
using LoLA.Networking.LCU.Objects;
using System.Collections.Generic;
using LoLA.Networking.Extensions;
using LoLA.Networking.LCU.Enums;
using LoLA.Networking.LCU.Data;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LoLA.Utils.Logger;
using Newtonsoft.Json;
using System.Text;
using LoLA.Utils;
using System.Net;
using System.IO;
using LoLA.Data;
using System;

namespace LoLA.Networking.LCU
{
    public static class LCUWrapper
    {
        private static WebRequestEx _webRequestEx;
        public static async Task<bool> InitAsync()
        {
            if(_webRequestEx != null)
            {
                try
                {
                    HttpWebRequest request = _webRequestEx.CreateRequest("/riotclient/ux-state");
                    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                    string empty = Misc.ReadStream(response.GetResponseStream()).Trim('"');
                }
                catch { _webRequestEx = null; }
            }

            try
            {
                WebRequestEx webRequest = getAuth();
                if (webRequest != null)
                {
                    _webRequestEx = webRequest;
                    return true;
                }
            }
            catch(Exception ex) {
                Log(ex.Message, LogType.EROR);
            }
            return false;
        }

        private static WebRequestEx getAuth()
        {
            string lolPath = LeagueClient.GetLocation();
            if (lolPath == null) return null;

            Dictionary<string, string> argsDict = getArguments(lolPath);
            if (argsDict == null) return null;

            WebRequestEx authRequest = new WebRequestEx(argsDict["app-port"], argsDict["remote-auth-token"]);
            return authRequest;
        }

        private static Dictionary<string, string> getArguments(string lolPath)
        {
            DirectoryInfo lolDirectoryInfo = new DirectoryInfo(lolPath);

            if (!lolDirectoryInfo.Exists) Log("League of Legends directory not found", LogType.WARN);

            string lockfilePath = Path.Combine(lolDirectoryInfo.FullName, "lockfile");

            if (!File.Exists(lockfilePath)) Log("Lockfile not found", LogType.WARN);

            string lockfileContent;

            using(var fileStream = new FileStream(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                lockfileContent = Misc.ReadStream(fileStream);
            }

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

        #region API Request
        public static async Task<Phase> GetGamePhaseAsync()
        {
            if (!await InitAsync())
                return Phase.None;

            try
            {
                string phase = (await getDataRequestAsync(RequestMethod.GET, "/lol-gameflow/v1/gameflow-phase"))?.Trim('"');
                return (Phase)Enum.Parse(typeof(Phase), phase);
            } catch { return Phase.None; }
        }

        public static async Task<List<RunePage>> GetRunePagesAsync()
        {
            if (!await InitAsync())
                return null;

            string result = await getDataRequestAsync(RequestMethod.GET, "/lol-perks/v1/pages");
            return await Objects.JsonConverter.JsonToRunePagesAsync(result);
        }

        public static async Task<RunePage> GetCurrentRunePageAsync()
        {
            if (!await InitAsync())
                return null;

            string result = await getDataRequestAsync(RequestMethod.GET, "/lol-perks/v1/currentpage");
            return await Objects.JsonConverter.JsonToRunePageAsync(result);
        }

        public static async Task<bool> AddRunePageAsync(RunePage pageJson)
        {
            if (!await InitAsync())
                return false;

            byte[] data = Encoding.UTF8.GetBytes(await Objects.JsonConverter.RunePageToJsonAsync(pageJson));
            return await sendDataRequestAsync(RequestMethod.POST, "/lol-perks/v1/pages", data);
        }

        public static async Task<bool> DeleteRunePageAsync(ulong id) 
            => await getResponseRequestAsync(RequestMethod.DELETE, $"/lol-perks/v1/pages/{id}");
        public static async Task<bool> AcceptMatchmakingAsync() 
            => await getResponseRequestAsync(RequestMethod.POST, "/lol-matchmaking/v1/ready-check/accept");
        public static async Task<bool> DeclineMatchmakingAsync() 
            => await getResponseRequestAsync(RequestMethod.POST, "/lol-matchmaking/v1/ready-check/decline");

        public static async Task<SessionData> GetSessionDataAsync()
        {
            if (!await InitAsync())
                return null;

            var sessionData = new SessionData();
            sessionData.GameMode = await GetCurrentGameModeAsync();
            try
            {
                var json = await getDataRequestAsync(RequestMethod.GET, "/lol-gameflow/v1/session");
                var session = JObject.Parse(json);
                var gameDataPath = session["gameData"];
                var queuePath = gameDataPath["queue"];
     
                sessionData.Category = queuePath["category"].ToString();
                sessionData.Description = queuePath["description"].ToString();
                sessionData.IsRanked = queuePath["isRanked"].ToObject<bool>();

                return sessionData;

            }
            catch { return sessionData; }
        }

        public static async Task<GameMode> GetCurrentGameModeAsync()
        {
            if (!await InitAsync())
                return GameMode.NONE;

            try
            {
                var json = await getDataRequestAsync(RequestMethod.GET, "/lol-lobby/v2/lobby");

                var obj = JObject.Parse(json);
                var gameConfig = obj["gameConfig"] as JObject;
                return (GameMode)Enum.Parse(typeof(GameMode), gameConfig["gameMode"].ToString().ToUpper());
            }
            catch { return GameMode.NONE; }
        }

        public static async Task<string> GetCurrentChampionAsync()
        {
            if (!await InitAsync())
                return null;

            return await getDataRequestAsync(RequestMethod.GET, "/lol-champ-select-legacy/v1/current-champion");
        }

        public static async Task<string> GetCurrentChampionAsyncV2()
        {
            if (!await InitAsync())
                return string.Empty;

            string json = await getDataRequestAsync(RequestMethod.GET, "/lol-champ-select/v1/skin-selector-info");

            try
            {
                JObject obj = JObject.Parse(json);
                return obj == null ? string.Empty : obj["championName"].Value<string>();
            }
            catch { return string.Empty; }
        }

        public static async Task<Summoner> GetCurrentSummonerAsync()
        {
            if (!await InitAsync())
                return null;

            string result = await getDataRequestAsync(RequestMethod.GET, "/lol-summoner/v1/current-summoner");

            if (!string.IsNullOrEmpty(result))
                return await Objects.JsonConverter.JsonToSummonerAsync(result);

            return new Summoner();
        }

        public static async Task<JObject> GetCurrentSessionAsync()
        {
            if (!await InitAsync())
                return null;

            var currentSessionJson = await getDataRequestAsync(RequestMethod.GET, "/lol-champ-select/v1/session");

            return JsonConvert.DeserializeObject<JObject>(currentSessionJson);;
        }

        public static async Task<Matchmaking> GetMatchmakingInfo()
        {
            if (!await InitAsync())
                return null;

            if (await GetGamePhaseAsync() == Phase.ReadyCheck)
            {
                string json = await getDataRequestAsync(RequestMethod.GET, "/lol-matchmaking/v1/ready-check");
                return JsonConvert.DeserializeObject<Matchmaking>(json);
            }
            return null;
        }

        public static async Task<bool> SetSummonerSpellsAsync(Spell spell, GameMode gameMode = GameMode.CLASSIC)
        {
            if (!await InitAsync())
                return false;

            var json = "{\n" + $"\"spell1Id\": {int.Parse(spell.First)},\n\"spell2Id\": {int.Parse(spell.Second)}" + "\n}";

            var mySelection0 = "/lol-lobby-team-builder/champ-select/v1/session/my-selection";
            var mySelection1 = "/lol-champ-select/v1/session/my-selection";

            var isAramOrUrf = gameMode == GameMode.ARAM || gameMode == GameMode.ARURF;
            string urlRequest = isAramOrUrf ? mySelection0 : mySelection1;

            byte[] data = Encoding.UTF8.GetBytes(json);
            return await sendDataRequestAsync(RequestMethod.PATCH, urlRequest, data);
        }

        public static async Task<string[]> GetCurrentSpellsAsync()
        {
            var currentSession = await GetCurrentSessionAsync();
            string[] spells = new string[2];

            if (currentSession != null)
            {
                var currentSummoner = currentSession["myTeam"][0];
                spells[0] = DataConverter.SpellKeyToSpellName((int)currentSummoner["spell1Id"]);
                spells[1] = DataConverter.SpellKeyToSpellName((int)currentSummoner["spell2Id"]);
                return spells;
            }
            return null;
        }

        public static async Task<JArray> GetMessagesAsync(string id)
        {
            if (!await InitAsync())
                return null;

            var json = await getDataRequestAsync(RequestMethod.GET, $"/lol-chat/v1/conversations{id}/messages");
            return json != null ? JArray.Parse(json) : null;
        }

        public static async Task<JArray> GetConversationsAsync()
        {
            if (!await InitAsync())
                return null;

            var json = await getDataRequestAsync(RequestMethod.GET, "/lol-chat/v1/conversations");
            return json != null ? JArray.Parse(json) : null;
        }

        public static async Task SendMessageAsync(JArray conversations,string message, string type)
        {
            foreach (var conversation in conversations.Root)
            {
                if (conversation["type"].ToString() == type)
                {
                    var conversationId = conversation["id"].ToString();

                    var body = Encoding.UTF8.GetBytes("{\"body\":\"" + message + "\"}");
                    await sendDataRequestAsync(RequestMethod.POST, $"/lol-chat/v1/conversations/{conversationId}/messages", body);
                }
            }
        }
        #endregion

        #region Request

        private static  HttpWebRequest createRequest(RequestMethod requestMethod, string url)
        {
            HttpWebRequest request = _webRequestEx.CreateRequest(url);
            request.Method = requestMethod.ToString().ToUpper();

            return request;
        }

        private static async Task<bool> sendDataRequestAsync(RequestMethod requestMethod, string url, byte[] data = null)
        {
            try
            {
                var request = createRequest(requestMethod, url);

                Stream newStream = request.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();

                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.OK) return true;

                return false;
            }
            catch { return false; }
        }

        private static async Task<bool> getResponseRequestAsync(RequestMethod requestMethod, string url)
        {
            if (!await InitAsync())
                return false;

            try
            {
                var request = createRequest(requestMethod, url);

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
        private static async Task<string> getDataRequestAsync(RequestMethod requestMethod, string url)
        {
            try
            {
                var request = createRequest(requestMethod, url);

                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                return Misc.ReadStream(response.GetResponseStream());
            }
            catch { return null; }
        }
        #endregion
    }
}
