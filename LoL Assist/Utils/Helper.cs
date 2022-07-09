using System.Runtime.InteropServices;
using LoL_Assist_WAPP.Model;
using LoLA.Utils.Logger;
using Newtonsoft.Json;
using System.IO;
using System;
using LoLA;
using System.Windows;

namespace LoL_Assist_WAPP.Utils
{
    public static class Helper
    {
        #region console
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        #endregion

        #region LogService
        public static void Log(string msg, LogType logType) => LogService.Log(LogService.Model(msg, LibInfo.NAME + " - WAPP", logType));
        #endregion

        #region misc
        public static void ShowBuildEditorWindow()
        {
            var window = new BuildEditorWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        //public static void SwitchTheme(string theme = "Default")
        //{
        //    switch(theme)
        //    {
        //        case "League of Legends":
        //            Application.Current.Resources["aPrimary"] = ColorConverter.ConvertFromString("#C99D3E");
        //            Application.Current.Resources["aSecondary"] = ColorConverter.ConvertFromString("#785A28");
        //            Application.Current.Resources["aTritary"] = ColorConverter.ConvertFromString("#EEE3D2");
        //            Application.Current.Resources["bPrimary"] = ColorConverter.ConvertFromString("#1A2832");
        //            Application.Current.Resources["bSecondary"] = ColorConverter.ConvertFromString("#010A13");
        //            Application.Current.Resources["CheckMark"] = Colors.White;
        //            Application.Current.Resources["title"] = Colors.White;

        //            Application.Current.Resources["FontPrimaryColor"] = Colors.Silver;
        //            Application.Current.Resources["FontSecondaryColor"] = Colors.Gray;
        //            Application.Current.Resources["FontTritaryColor"] = ColorConverter.ConvertFromString("#969284");
        //            break;
        //        default:
        //            Application.Current.Resources["aPrimary"] = ColorConverter.ConvertFromString("#7160E8");
        //            Application.Current.Resources["aSecondary"] = ColorConverter.ConvertFromString("#403582");
        //            Application.Current.Resources["aTritary"] = ColorConverter.ConvertFromString("#A093EF");
        //            Application.Current.Resources["bPrimary"] = ColorConverter.ConvertFromString("#303030");
        //            Application.Current.Resources["bSecondary"] = ColorConverter.ConvertFromString("#18191C");
        //            Application.Current.Resources["CheckMark"] = Colors.White;
        //            Application.Current.Resources["title"] = Colors.White;

        //            Application.Current.Resources["FontPrimaryColor"] = Colors.Silver;
        //            Application.Current.Resources["FontSecondaryColor"] = Colors.Gray;
        //            Application.Current.Resources["FontTritaryColor"] = ColorConverter.ConvertFromString("#969284");
        //            break;
        //    }
        //    //System.Windows.Forms.Application.Restart();
        //}

        public static ItemImageModel ItemImage(string text, string image)
        {
            var model = new ItemImageModel()
            {
                Text = text,
                Image = image
            };
            return model;
        }

        public static string FixedName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (name.Contains("'"))
                    return name;
                else return name + "'s";
            }
            return "Jeff's";
        }
        public static string ImageSrc(string name, string format = "png")
        {
            return $"{ConfigModel.RESOURCE_PATH}{name}.{format}";
        }

        public static string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        #endregion
    }
}
