using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using LoLA;

namespace LoL_Assist_WAPP.ViewModel
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        private bool _AutoRune;
        public bool AutoRune
        {
            get { return _AutoRune; }
            set
            {
                if (_AutoRune != value)
                {
                    _AutoRune = value;
                    ConfigM.config.AutoRune = value;
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
                    ConfigM.config.AutoSpells = value;
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
                    ConfigM.config.AutoAccept = value;
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
                    ConfigM.config.FlashPlacementToRight = value;
                    OnPropertyChanged("FlashPlacementToRight");
                }
            }
        }

        private bool _LowSpecMode;
        public bool LowSpecMode
        {
            get { return _LowSpecMode; }
            set
            {
                if (_LowSpecMode != value)
                {
                    _LowSpecMode = value;
                    ConfigM.config.LowSpecMode = value;

                    if (ConfigM.config.LowSpecMode)
                        ConfigM.config.MonitoringDelay = 550;
                    else ConfigM.config.MonitoringDelay = 300;

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
                    ConfigM.config.Logging = value;
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
                    ConfigM.config.BuildCache = value;
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
                    ConfigM.config.UpdateOnStartup = value;
                    Global.Config.caching = value;
                    OnPropertyChanged("UpdateOnStartup");
                }
            }
        }

        public ConfigViewModel()
        {
            AutoRune = ConfigM.config.AutoRune;
            AutoSpells = ConfigM.config.AutoSpells;
            AutoAccept = ConfigM.config.AutoAccept;
            FlashPlacementToRight = ConfigM.config.FlashPlacementToRight;

            Logging = ConfigM.config.Logging;
            Global.Config.logging = Logging;
            LowSpecMode = ConfigM.config.LowSpecMode;
            BuildCache = ConfigM.config.BuildCache;
            Global.Config.caching = BuildCache;
            UpdateOnStartup = ConfigM.config.UpdateOnStartup;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            ConfigM.SaveConfig(false);
        }
    }
}
