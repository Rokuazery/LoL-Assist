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
        private bool autoRunes;
        public bool AutoRunes
        {
            get { return autoRunes; }
            set
            {
                if (autoRunes != value)
                {
                    autoRunes = value;
                    ConfigModel.config.AutoRunes = value;
                    OnPropertyChanged(nameof(AutoRunes));
                }
            }
        }

        private bool autoSpells;
        public bool AutoSpells
        {
            get => autoSpells;
            set
            {
                if (autoSpells != value)
                {
                    autoSpells = value;
                    ConfigModel.config.AutoSpells = value;
                    OnPropertyChanged(nameof(AutoSpells));
                }
            }
        }

        private bool autoAccept;
        public bool AutoAccept
        {
            get => autoAccept;
            set
            {
                if (autoAccept != value)
                {
                    autoAccept = value;
                    ConfigModel.config.AutoAccept = value;
                    OnPropertyChanged(nameof(AutoAccept));
                }
            }
        }

        private bool flashPlaceMentToRight;
        public bool FlashPlacementToRight
        {
            get => flashPlaceMentToRight;
            set
            {
                if (flashPlaceMentToRight != value)
                {
                    flashPlaceMentToRight = value;
                    ConfigModel.config.FlashPlacementToRight = value;
                    OnPropertyChanged(nameof(FlashPlacementToRight));
                }
            }
        }

        private bool useLatestPatch;
        public bool UseLatestPatch
        {
            get => useLatestPatch;
            set
            {
                if (useLatestPatch != value)
                {
                    useLatestPatch = value;
                    ConfigModel.config.UseLatestPatch = value;
                    Global.Config.useLatestPatch = value;
                    OnPropertyChanged(nameof(UseLatestPatch));
                }
            }
        }

        private bool customRunesSpells;
        public bool CustomRunesSpells
        {
            get => customRunesSpells;
            set
            {
                if (customRunesSpells != value)
                {
                    customRunesSpells = value;
                    ConfigModel.config.CustomRunesSpells = value;
                    OnPropertyChanged(nameof(CustomRunesSpells));
                }
            }
        }
        #endregion

        #region Miscellaneous
        private bool lowSpecMode;
        public bool LowSpecMode
        {
            get => lowSpecMode;
            set
            {
                if (lowSpecMode != value)
                {
                    lowSpecMode = value;
                    ConfigModel.config.LowSpecMode = value;

                    if (ConfigModel.config.LowSpecMode)
                        ConfigModel.config.MonitoringDelay = 300;
                    else ConfigModel.config.MonitoringDelay = 250;

                    OnPropertyChanged(nameof(LowSpecMode));
                }
            }
        }

        private bool logging;
        public bool Logging
        {
            get => logging;
            set
            {
                if (logging != value)
                {
                    logging = value;
                    ConfigModel.config.Logging = value;
                    Global.Config.logging = value;
                    OnPropertyChanged(nameof(Logging));
                }
            }
        }

        private bool buildCache;
        public bool BuildCache
        {
            get => buildCache;
            set
            {
                if (buildCache != value)
                {
                    buildCache = value;
                    ConfigModel.config.BuildCache = value;
                    Global.Config.caching = value;
                    OnPropertyChanged(nameof(BuildCache));
                }
            }
        }

        private bool updateOnStartup;
        public bool UpdateOnStartup
        {
            get => updateOnStartup;
            set
            {
                if (updateOnStartup != value)
                {
                    updateOnStartup = value;
                    ConfigModel.config.UpdateOnStartup = value;
                    Global.Config.caching = value;
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
            var path = Global.libraryFolder;
            if(Directory.Exists(path)) Process.Start(path);
        }

        public void Update()
        {
            AutoRunes = ConfigModel.config.AutoRunes;
            AutoSpells = ConfigModel.config.AutoSpells;
            AutoAccept = ConfigModel.config.AutoAccept;
            FlashPlacementToRight = ConfigModel.config.FlashPlacementToRight;
            UseLatestPatch = ConfigModel.config.UseLatestPatch;
            Global.Config.useLatestPatch = UseLatestPatch;
            CustomRunesSpells = ConfigModel.config.CustomRunesSpells;

            Logging = ConfigModel.config.Logging;
            Global.Config.logging = Logging;
            LowSpecMode = ConfigModel.config.LowSpecMode;
            BuildCache = ConfigModel.config.BuildCache;
            Global.Config.caching = BuildCache;
            UpdateOnStartup = ConfigModel.config.UpdateOnStartup;
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
