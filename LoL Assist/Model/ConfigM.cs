using System.IO;
using Newtonsoft.Json;

namespace LoL_Assist_WAPP.Model
{
    public static class ConfigM
    {
        public static Config config = new Config(); 

        public class Config
        {
            public int MonitoringDelay { get; set; } = 300;
            public bool AutoRune { get; set; } = true;
            public bool AutoAccept { get; set; } = true;
            public bool AutoSpells { get; set; } = true;
            public bool LowSpecMode { get; set; } = false;
            public bool FlashPlacementToRight { get; set; } = false;
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
        }

        public static void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(config);

            if (File.Exists(ConfigFileName))
                File.WriteAllText(ConfigFileName, json);
            else LoadConfig();
        }
    }
}
