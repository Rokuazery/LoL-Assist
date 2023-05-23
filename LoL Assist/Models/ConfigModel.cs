using LoL_Assist_WAPP.ViewModels;
using System.Reflection;
using LoLA.Data.Enums;
using Newtonsoft.Json;
using System.Windows;
using System.IO;
using LoLA;

namespace LoL_Assist_WAPP.Models
{
    public static class ConfigModel
    {
        public static Config s_Config = new Config();
        public static Provider s_CurrentProvider = Provider.OPGG;
        public static readonly Thickness r_MarginOpen = new Thickness(0, 0, 0, 0);
        public static readonly Thickness r_MarginClose = new Thickness(505, 0, 0, 0); // Main window width
        public static readonly string r_Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public const string RESOURCE_PATH = "pack://application:,,,/Resources/";

        public class Config: ViewModelBase
        {
            #region General
            public int UpdateDelay { get; set; } = 250;

            private bool _autoRunes = true;
            public bool AutoRunes
            {
                get => _autoRunes;
                set
                {
                    if (_autoRunes != value)
                    {
                        _autoRunes = value;
                        OnPropertyChanged(nameof(AutoRunes));
                    }
                }
            }

            private bool _autoAccept = true;
            public bool AutoAccept
            {
                get => _autoAccept;
                set
                {
                    if (_autoAccept != value)
                    {
                        _autoAccept = value;
                        OnPropertyChanged(nameof(AutoAccept));
                    }
                }
            }

            private bool _autoSpells = true;
            public bool AutoSpells
            {
                get => _autoSpells;
                set
                {
                    if (_autoSpells != value)
                    {
                        _autoSpells = value;
                        OnPropertyChanged(nameof(AutoSpells));
                    }
                }
            }

            private bool _useLatestPatch = false;
            public bool UseLatestPatch
            {
                get => _useLatestPatch;
                set
                {
                    if (_useLatestPatch != value)
                    {
                        _useLatestPatch = value;
                        OnPropertyChanged(nameof(UseLatestPatch));
                    }
                }
            }

            private bool _flashPlacementToRight = false;
            public bool FlashPlacementToRight
            {
                get => _flashPlacementToRight;
                set
                {
                    if (_flashPlacementToRight != value)
                    {
                        _flashPlacementToRight = value;
                        OnPropertyChanged(nameof(FlashPlacementToRight));
                    }
                }
            }

            #endregion

            #region Misc
            private bool _logging = true;
            public bool Logging
            {
                get => _logging;
                set
                {
                    if (_logging != value)
                    {
                        _logging = value;
                        OnPropertyChanged(nameof(Logging));
                    }
                }
            }

            private bool _lowSpecMode = false;
            public bool LowSpecMode
            {
                get => _lowSpecMode;
                set
                {
                    if (_lowSpecMode != value)
                    {
                        _lowSpecMode = value;
                        OnPropertyChanged(nameof(LowSpecMode));
                    }
                }
            }

            private bool _buildCache = true;
            public bool BuildCache
            {
                get => _buildCache;
                set
                {
                    if (_buildCache != value)
                    {
                        _buildCache = value;
                        OnPropertyChanged(nameof(BuildCache));
                    }
                }
            }

            private bool _updateOnStartup = true;
            public bool UpdateOnStartup
            {
                get => _updateOnStartup;
                set
                {
                    if (_updateOnStartup != value)
                    {
                        _updateOnStartup = value;
                        OnPropertyChanged(nameof(UpdateOnStartup));
                    }
                }
            }
           
            public Provider Provider { get; set; } = Provider.OPGG;
            #endregion

            #region AutoMessage
            private bool _autoMessage = false;
            public bool AutoMessage
            {
                get => _autoMessage;
                set
                {
                    if (_autoMessage != value)
                    {
                        _autoMessage = value;
                        OnPropertyChanged(nameof(AutoMessage));
                    }
                }
            }

            private string _message;
            public string Message
            {
                get => _message;
                set
                {
                    if (_message != value)
                    {
                        _message = value;
                        OnPropertyChanged(nameof(Message));
                    }
                }
            }

            private bool _clearMessageAfterSent = true;
            public bool ClearMessageAfterSent
            {
                get => _clearMessageAfterSent;
                set
                {
                    if (_clearMessageAfterSent != value)
                    {
                        _clearMessageAfterSent = value;
                        OnPropertyChanged(nameof(ClearMessageAfterSent));
                    }
                }
            }

            #endregion

            public override void OnPropertyChanged(string propertyName)
            {
                if (!firstLoad)
                    SaveConfig();
            }
                
        }

        private const string CONFIG_FILE_NAME = "LoLA Config.json";

        private static bool firstLoad = true;
        public static void LoadConfig()
        {
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

            firstLoad = false;
        }

        public static void SaveConfig()
        {
            if (!File.Exists(CONFIG_FILE_NAME))
            {
                LoadConfig();
                return;
            }

            var json = JsonConvert.SerializeObject(s_Config, Formatting.Indented);
            using var stream = new StreamWriter(CONFIG_FILE_NAME);
            stream.Write(json);
        }
    }
}
