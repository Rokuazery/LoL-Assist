﻿using LoL_Assist_WAPP.Model;
using System.ComponentModel;

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

        public ConfigViewModel()
        {
            ConfigM.LoadConfig();

            AutoRune = ConfigM.config.AutoRune;
            AutoSpells = ConfigM.config.AutoSpells;
            AutoAccept = ConfigM.config.AutoAccept;
            FlashPlacementToRight = ConfigM.config.FlashPlacementToRight;
            LowSpecMode = ConfigM.config.LowSpecMode;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            ConfigM.SaveConfig();
        }
    }
}
