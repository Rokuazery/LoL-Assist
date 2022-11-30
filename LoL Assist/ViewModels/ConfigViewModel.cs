using LoL_Assist_WAPP.Commands;
using LoL_Assist_WAPP.Models;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System;
using LoLA;

namespace LoL_Assist_WAPP.ViewModels
{
    public class ConfigViewModel: ViewModelBase
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
                    ConfigModel.s_Config.UpdateDelay = ConfigModel.s_Config.LowSpecMode ? 300 : 250;
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

        public ICommand ShowFolderInExCommand { get; }
        public ICommand CreateShortcutCommand { get; }
        #endregion

        public ConfigViewModel()
        {
            Update();
            ShowFolderInExCommand = new Command(_ => showFolderExecute());
            CreateShortcutCommand = new Command(_ => createDesktopShortcut());
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

        private void showFolderExecute()
        {
            var path = LibInfo.r_LibFolderPath;
            if (Directory.Exists(path)) Process.Start(path);
        }

        private bool createDesktopShortcut()
        {
            var deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var fullDir = deskDir + @"\LoL Assist.lnk";
            if (!File.Exists(fullDir))
            {
                object shDesktop = "Desktop";
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                string shortcutAddress = fullDir;
                var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutAddress);
                shortcut.Description = "Shortcut for LoL Assist";
                shortcut.Hotkey = "Ctrl+Shift+A";
                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.WorkingDirectory = Directory.GetCurrentDirectory();
                shortcut.RelativePath = Application.ExecutablePath;
                shortcut.Save();
                return true;
            }
            else
                return false;
        }
        #region Space junk
        public override void OnPropertyChanged(string propertyName) 
            => ConfigModel.SaveConfig(false);
        #endregion
    }
}
