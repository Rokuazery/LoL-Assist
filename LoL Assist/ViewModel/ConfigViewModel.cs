using System.Collections.Generic;
using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using LoLA;

namespace LoL_Assist_WAPP.ViewModel
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        #region General
        private bool _autoRunes;
        public bool AutoRunes
        {
            get { return _autoRunes; }
            set
            {
                if (_autoRunes != value)
                {
                    _autoRunes = value;
                    ConfigModel.s_Config.AutoRunes = value;
                    OnPropertyChanged(nameof(AutoRunes));
                }
            }
        }

        private bool _autoSpells;
        public bool AutoSpells
        {
            get => _autoSpells;
            set
            {
                if (_autoSpells != value)
                {
                    _autoSpells = value;
                    ConfigModel.s_Config.AutoSpells = value;
                    OnPropertyChanged(nameof(AutoSpells));
                }
            }
        }

        private bool _autoAccept;
        public bool AutoAccept
        {
            get => _autoAccept;
            set
            {
                if (_autoAccept != value)
                {
                    _autoAccept = value;
                    ConfigModel.s_Config.AutoAccept = value;
                    OnPropertyChanged(nameof(AutoAccept));
                }
            }
        }

        private bool _flashPlaceMentToRight;
        public bool FlashPlacementToRight
        {
            get => _flashPlaceMentToRight;
            set
            {
                if (_flashPlaceMentToRight != value)
                {
                    _flashPlaceMentToRight = value;
                    ConfigModel.s_Config.FlashPlacementToRight = value;
                    OnPropertyChanged(nameof(FlashPlacementToRight));
                }
            }
        }

        private bool _useLatestPatch;
        public bool UseLatestPatch
        {
            get => _useLatestPatch;
            set
            {
                if (_useLatestPatch != value)
                {
                    _useLatestPatch = value;
                    ConfigModel.s_Config.UseLatestPatch = value;
                    GlobalConfig.s_LatestPatch = value;
                    OnPropertyChanged(nameof(UseLatestPatch));
                }
            }
        }

        private bool _customRunesSpells;
        public bool CustomRunesSpells
        {
            get => _customRunesSpells;
            set
            {
                if (_customRunesSpells != value)
                {
                    _customRunesSpells = value;
                    ConfigModel.s_Config.CustomRunesSpells = value;
                    OnPropertyChanged(nameof(CustomRunesSpells));
                }
            }
        }
        #endregion

        #region Miscellaneous
        private bool _lowSpecMode;
        public bool LowSpecMode
        {
            get => _lowSpecMode;
            set
            {
                if (_lowSpecMode != value)
                {
                    _lowSpecMode = value;
                    ConfigModel.s_Config.LowSpecMode = value;
                    ConfigModel.s_Config.MonitoringDelay = ConfigModel.s_Config.LowSpecMode ? 300 : 250;
                    OnPropertyChanged(nameof(LowSpecMode));
                }
            }
        }

        private bool _logging;
        public bool Logging
        {
            get => _logging;
            set
            {
                if (_logging != value)
                {
                    _logging = value;
                    ConfigModel.s_Config.Logging = value;
                    GlobalConfig.s_Logging = value;
                    OnPropertyChanged(nameof(Logging));
                }
            }
        }

        private bool _buildCache;
        public bool BuildCache
        {
            get => _buildCache;
            set
            {
                if (_buildCache != value)
                {
                    _buildCache = value;
                    ConfigModel.s_Config.BuildCache = value;
                    GlobalConfig.s_Caching = value;
                    OnPropertyChanged(nameof(BuildCache));
                }
            }
        }

        private bool _updateOnStartup;
        public bool UpdateOnStartup
        {
            get => _updateOnStartup;
            set
            {
                if (_updateOnStartup != value)
                {
                    _updateOnStartup = value;
                    ConfigModel.s_Config.UpdateOnStartup = value;
                    GlobalConfig.s_Caching = value;
                    OnPropertyChanged(nameof(UpdateOnStartup));
                }
            }
        }

        //private List<string> themeList = new List<string>();
        //public List<string> ThemeList
        //{
        //    get => themeList;
        //    set
        //    {
        //        if (themeList != value)
        //        {
        //            themeList = value;
        //            OnPropertyChanged(nameof(SelectedTheme));
        //        }
        //    }
        //}

        //private string selectedTheme;
        //public string SelectedTheme
        //{
        //    get => selectedTheme;
        //    set
        //    {
        //        if (selectedTheme != value)
        //        {
        //            selectedTheme = value;
        //            ConfigModel.config.Theme = value;
        //            Utils.SwitchTheme(ConfigModel.config.Theme);
        //            OnPropertyChanged(nameof(SelectedTheme));
        //        }
        //    }
        //}

        public ICommand ShowFolderInExCommand { get; }
        #endregion

        public ConfigViewModel()
        {
            Update();
            ShowFolderInExCommand = new Command(action => ShowFolderExecute());
        }

        private void ShowFolderExecute()
        {
            var path = LibInfo.r_LibFolderPath;
            if(Directory.Exists(path)) Process.Start(path);
        }

        public void Update()
        {
            AutoRunes = ConfigModel.s_Config.AutoRunes;
            AutoSpells = ConfigModel.s_Config.AutoSpells;
            AutoAccept = ConfigModel.s_Config.AutoAccept;
            FlashPlacementToRight = ConfigModel.s_Config.FlashPlacementToRight;
            UseLatestPatch = ConfigModel.s_Config.UseLatestPatch;
            GlobalConfig.s_LatestPatch = UseLatestPatch;
            CustomRunesSpells = ConfigModel.s_Config.CustomRunesSpells;

            Logging = ConfigModel.s_Config.Logging;
            GlobalConfig.s_Logging = Logging;
            LowSpecMode = ConfigModel.s_Config.LowSpecMode;
            BuildCache = ConfigModel.s_Config.BuildCache;
            GlobalConfig.s_Caching = BuildCache;
            UpdateOnStartup = ConfigModel.s_Config.UpdateOnStartup;
        }
        #region Space junk
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ConfigModel.SaveConfig(false);
        }
        #endregion
    }
}
