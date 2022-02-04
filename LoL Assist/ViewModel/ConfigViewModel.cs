using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using LoLA;
using System.Windows.Input;
using System.Threading.Tasks;
using System;

namespace LoL_Assist_WAPP.ViewModel
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        #region General
        private bool _AutoRunes;
        public bool AutoRunes
        {
            get { return _AutoRunes; }
            set
            {
                if (_AutoRunes != value)
                {
                    _AutoRunes = value;
                    ConfigModel.config.AutoRunes = value;
                    OnPropertyChanged("AutoRune");
                }
            }
        }

        private bool _AutoSpells;
        public bool AutoSpells
        {
            get { return _AutoSpells; }
            set
            {
                if (_AutoSpells != value)
                {
                    _AutoSpells = value;
                    ConfigModel.config.AutoSpells = value;
                    OnPropertyChanged("AutoSpells");
                }
            }
        }

        private bool _AutoAccept;
        public bool AutoAccept
        {
            get { return _AutoAccept; }
            set
            {
                if (_AutoAccept != value)
                {
                    _AutoAccept = value;
                    ConfigModel.config.AutoAccept = value;
                    OnPropertyChanged("AutoAccept");
                }
            }
        }

        private bool _FlashPlacementToRight;
        public bool FlashPlacementToRight
        {
            get { return _FlashPlacementToRight; }
            set
            {
                if (_FlashPlacementToRight != value)
                {
                    _FlashPlacementToRight = value;
                    ConfigModel.config.FlashPlacementToRight = value;
                    OnPropertyChanged("FlashPlacementToRight");
                }
            }
        }

        private bool _UseLatestPatch;
        public bool UseLatestPatch
        {
            get { return _UseLatestPatch; }
            set
            {
                if (_UseLatestPatch != value)
                {
                    _UseLatestPatch = value;
                    ConfigModel.config.UseLatestPatch = value;
                    Global.Config.useLatestPatch = value;
                    OnPropertyChanged("UseLatestPatch");
                }
            }
        }

        private bool _CustomRunesSpells;
        public bool CustomRunesSpells
        {
            get { return _CustomRunesSpells; }
            set
            {
                if (_CustomRunesSpells != value)
                {
                    _CustomRunesSpells = value;
                    ConfigModel.config.CustomRunesSpells = value;
                    OnPropertyChanged("CustomRunesSpells");
                }
            }
        }
        #endregion

        #region Miscellaneous
        private bool _LowSpecMode;
        public bool LowSpecMode
        {
            get { return _LowSpecMode; }
            set
            {
                if (_LowSpecMode != value)
                {
                    _LowSpecMode = value;
                    ConfigModel.config.LowSpecMode = value;

                    if (ConfigModel.config.LowSpecMode)
                        ConfigModel.config.MonitoringDelay = 550;
                    else ConfigModel.config.MonitoringDelay = 300;

                    OnPropertyChanged("LowSpecMode");
                }
            }
        }

        private bool _Logging;
        public bool Logging
        {
            get { return _Logging; }
            set
            {
                if (_Logging != value)
                {
                    _Logging = value;
                    ConfigModel.config.Logging = value;
                    Global.Config.logging = value;
                    OnPropertyChanged("Logging");
                }
            }
        }

        private bool _BuildCache;
        public bool BuildCache
        {
            get { return _BuildCache; }
            set
            {
                if (_BuildCache != value)
                {
                    _BuildCache = value;
                    ConfigModel.config.BuildCache = value;
                    Global.Config.caching = value;
                    OnPropertyChanged("BuildCache");
                }
            }
        }

        private bool _UpdateOnStartup;
        public bool UpdateOnStartup
        {
            get { return _UpdateOnStartup; }
            set
            {
                if (_UpdateOnStartup != value)
                {
                    _UpdateOnStartup = value;
                    ConfigModel.config.UpdateOnStartup = value;
                    Global.Config.caching = value;
                    OnPropertyChanged("UpdateOnStartup");
                }
            }
        }
        #endregion

        public ConfigViewModel()
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
