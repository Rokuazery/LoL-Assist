using LoL_Assist_WAPP.Utils;
using System.Reflection;
using LoLA.Utils.Logger;
using Newtonsoft.Json;
using System.Windows;
using LoLA.Data.Enums;
using System.IO;
using LoLA;

namespace LoL_Assist_WAPP.Models
{
    public static class ConfigModel
    {
        public static Config s_Config = new Config();
        public static Provider s_CurrentProvider = Provider.METAsrc;
        public static readonly Thickness r_MarginOpen = new Thickness(0, 0, 0, 0);
        public static readonly Thickness r_MarginClose = new Thickness(505, 0, 0, 0); // Main window width
        public static readonly string r_Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public const string RESOURCE_PATH = "pack://application:,,,/Resources/";

        public class Config
        {
            #region General
            public int UpdateDelay { get; set; } = 250;
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
            //public string Theme { get; set; } = "Default";
            public Provider Provider { get; set; } = Provider.METAsrc;
            #endregion

            #region AutoMessage
            public bool AutoMessage { get; set; } = false;
            public string Message { get; set; }
            public bool ClearMessageAfterSent { get; set; } = true;
            #endregion
        }

        private const string CONFIG_FILE_NAME = "LoLA Config.json";
        public static void LoadConfig(bool log = true)
        {
            if(log) Helper.Log("Loading in Configuration...", LogType.INFO);

            if (!File.Exists(CONFIG_FILE_NAME))
            {
                File.Create(CONFIG_FILE_NAME).Dispose();
                var json = JsonConvert.SerializeObject(s_Config);
                using var stream = new StreamWriter(CONFIG_FILE_NAME);
                stream.Write(json);
            }

            s_Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(CONFIG_FILE_NAME));
            GlobalConfig.s_Logging = s_Config.Logging;
            GlobalConfig.s_Caching = s_Config.BuildCache;
        }

        public static void SaveConfig(bool log = true)
        {
            if (log) Helper.Log("Saving configuration...", LogType.INFO);

            var json = JsonConvert.SerializeObject(s_Config, Formatting.Indented);

            if (File.Exists(CONFIG_FILE_NAME))
            {
                using var stream = new StreamWriter(CONFIG_FILE_NAME);
                stream.Write(json);
            }
            else LoadConfig();
        }
    }
}
