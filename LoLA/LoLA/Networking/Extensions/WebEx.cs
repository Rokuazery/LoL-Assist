using static LoLA.Utils.Logger.LogService;
using System.Threading.Tasks;
using LoLA.Networking.Model;
using LoLA.Utils.Logger;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System;

namespace LoLA.Networking.Extensions
{
    public static class WebEx
    {
        const int TIMEOUT = 5 * 60 * 1000;
        public static async Task<string> RunDownloadStringAsync(string Url)
        {
            try
            {
                using (var client = new WebClientEx())
                {
                    client.Timeout = TIMEOUT;
                    client.Headers.Add(HttpRequestHeader.Cookie, "security=true");
                    Log($"Downloading string from '{Url}'", LibInfo.NAME, LogType.DBUG);
                    var result = await client.DownloadStringTaskAsync(new Uri(Url));
                    return result;
                }
            }
            catch (WebException WebEx)
            {
                Log(WebEx.Message, WebEx.Source, LogType.EROR);
                return null;
            }
        }

        public static async Task<string> RunDownloadAysnc(WebModel webModel)
        {
            try
            {
                using (var client = new WebClientEx())
                {
                    client.Timeout = TIMEOUT;
                    client.Headers.Add(HttpRequestHeader.Cookie, "security=true");

                    if (!File.Exists(webModel.Path)
                    || string.IsNullOrEmpty(File.ReadAllText(webModel.Path)))
                    {
                        Log($"Downloading file from '{webModel.Url}' to '{webModel.Path}'", LibInfo.NAME, LogType.DBUG);
                        await client.DownloadFileTaskAsync(webModel.Url, webModel.Path);
                    }
                    else Log($"Loading in '{Path.GetFileName(webModel.Path)}' from '{webModel.Path}'", LibInfo.NAME, LogType.DBUG);
                }
            }
            catch (WebException WebEx) { Log(WebEx.Message, WebEx.Source, LogType.EROR); }
            return webModel.Path;
        }

        // DlDe = Download + Deserialize
        public static async Task<T> DlDeAndSaveToFile<T>(WebModel webModel)
        {
            string JsonContent = null;
            using (var stream = new StreamReader(await RunDownloadAysnc(webModel)))
            {
                JsonContent = stream.ReadToEnd();
            }
            var obj = JsonConvert.DeserializeObject<T>(JsonContent);
            return obj;
        }

        public static async Task<T> DlDe<T>(string url)
        {
            string JsonContent = await RunDownloadStringAsync(url);
            var obj = JsonConvert.DeserializeObject<T>(JsonContent);
            return obj;
        }
    }
}
