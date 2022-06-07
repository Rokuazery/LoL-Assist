using System.Threading.Tasks;
using Newtonsoft.Json;
using LoLA.Utils.Logger;
using System.Net;
using System.IO;
using System;

namespace LoLA.Utils
{
    public static class WebExt
    {
        private static int _timeout = 5 * 60 * 1000;
        public static async Task<string> RunDownloadStringAsync(string Url)
        {
            try
            {
                using (var client = new WebClientEx())
                {
                    client.Timeout = _timeout;
                    client.Headers.Add(HttpRequestHeader.Cookie, "security=true");
                    LogService.Log(LogService.Model($"Downloading string from '{Url}'", Global.name, LogType.DBUG));
                    var result = await client.DownloadStringTaskAsync(new Uri(Url)); 
                    return result;
                }
            }
            catch (WebException WebEx)
            {
                LogService.Log(LogService.Model(WebEx.Message, WebEx.Source, LogType.EROR));
                return null;
            }
        }

        public static async Task<string> RunDownloadAysnc(WebModel webModel)
        {
            try
            {
                using(var client = new WebClientEx())
                {
                    client.Timeout = _timeout;
                    client.Headers.Add(HttpRequestHeader.Cookie, "security=true");

                    if (!File.Exists(webModel.path)
                    || string.IsNullOrEmpty(File.ReadAllText(webModel.path)))
                    {
                        LogService.Log(LogService.Model($"Downloading file from '{webModel.url}' to '{webModel.path}'", Global.name, LogType.DBUG));
                        await client.DownloadFileTaskAsync(webModel.url, webModel.path);
                    }
                    else LogService.Log(LogService.Model($"Loading in '{Path.GetFileName(webModel.path)}' from '{webModel.path}'", Global.name, LogType.DBUG));
                }
            }
            catch (WebException WebEx) { LogService.Log(LogService.Model(WebEx.Message, WebEx.Source, LogType.EROR)); }
            return webModel.path;
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

    public class WebModel
    {
        public string path;
        public string url;
    }
}
