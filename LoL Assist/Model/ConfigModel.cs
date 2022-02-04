using System.Reflection;
using Newtonsoft.Json;
using System.Windows;
using LoLA.Utils.Log;
using System.IO;
using LoLA;

namespace LoL_Assist_WAPP.Model
{
    public static class ConfigModel
    {
        public static Config config = new Config();
        public const string DefaultSource = "Metasrc.com";
        public static Thickness marginOpen = new Thickness(0, 0, 0, 0);
        public static Thickness marginClose = new Thickness(465, 0, 0, 0);
        public static readonly string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public class Config
        {
            #region General
            public int MonitoringDelay { get; set; } = 300;
            public bool AutoRunes { get; set; } = true;
            public bool AutoAccept { get; set; } = true;
            public bool AutoSpells { get; set; } = true;
            public bool UseLatestPatch { get; set; } = false;
            public bool CustomRunesSpells { get; set; } = true;
            public bool FlashPlacementToRight { get; set; } = false;
            #endregion

            #region Misc
            public bool Logging { get; set; } = false;
            public bool LowSpecMode { get; set; } = false;
            public bool BuildCache { get; set; } = true;
            public bool UpdateOnStartup { get; set; } = true;
            #endregion
        }

        private const string ConfigFileName = "LoLA Config.json";
        public static void LoadConfig(bool log = true)
        {
            if(log)
                Utils.Log("Loading in Configuration...", LogType.INFO);

            if (!File.Exists(ConfigFileName))
            {
                File.Create(ConfigFileName).Dispose();
                var json = JsonConvert.SerializeObject(config);
                using (var stream = new StreamWriter(ConfigFileName))
                {
                    stream.Write(json);
                }
            }

            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFileName));
            Global.Config.logging = config.Logging;
            Global.Config.caching = config.BuildCache;
        }

        public static void SaveConfig(bool log = true)
        {
            if (log)
                Utils.Log("Saving configuration...", LogType.INFO);

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);

            if (File.Exists(ConfigFileName))
            {
                using(var stream = new StreamWriter(ConfigFileName))
                {
                    stream.Write(json);
                }
            }
            else LoadConfig();
        }
    }
}
