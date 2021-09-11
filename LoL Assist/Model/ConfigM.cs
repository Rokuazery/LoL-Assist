using System.IO;
using System.Reflection;
using System.Windows;
using LoLA;
using Newtonsoft.Json;

namespace LoL_Assist_WAPP.Model
{
    public static class ConfigM
    {
        public static Config config = new Config(); 
        public static Thickness marginOpen = new Thickness(0, 0, 0, 0);
        public static Thickness marginClose = new Thickness(465, 0, 0, 0);
        public static readonly string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public class Config
        {
            public int MonitoringDelay { get; set; } = 300;
            public bool AutoRune { get; set; } = true;
            public bool AutoAccept { get; set; } = true;
            public bool AutoSpells { get; set; } = true;
            public bool FlashPlacementToRight { get; set; } = false;

            public bool Logging { get; set; } = false;
            public bool LowSpecMode { get; set; } = false;
            public bool BuildCache { get; set; } = true;
        }

        private const string ConfigFileName = "LoLA Config.json";
        public static void LoadConfig()
        {
            if (!File.Exists(ConfigFileName))
            {
                File.Create(ConfigFileName).Dispose();
                var json = JsonConvert.SerializeObject(config);
                File.WriteAllText(ConfigFileName, json);
            }

            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFileName));
            Global.Config.logging = config.Logging;
            Global.Config.caching = config.BuildCache;
        }

        public static void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);

            if (File.Exists(ConfigFileName))
                File.WriteAllText(ConfigFileName, json);
            else LoadConfig();
        }
    }
}
