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
        public ICommand ShowFolderInExCommand { get; }
        public ICommand CreateShortcutCommand { get; }

        public ConfigViewModel()
        {
            ShowFolderInExCommand = new Command(_ => ShowFolderExecute());
            CreateShortcutCommand = new Command(_ => CreateDesktopShortcut());
        }

        private void ShowFolderExecute()
        {
            var path = LibInfo.r_LibFolderPath;
            if (Directory.Exists(path)) Process.Start(path);
        }

        private bool CreateDesktopShortcut()
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
    }
}
