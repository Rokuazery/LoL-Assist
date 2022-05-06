using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using LoL_Assist_WAPP.Model;
using System.Windows.Media;
using Newtonsoft.Json;
using LoLA.Utils.Log;
using System.Windows;
using System.IO;
using System;
using LoLA;

namespace LoL_Assist_WAPP
{
    public static class Utils
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
        public static void Log(string msg, LogType logType) => LogService.Log(LogService.Model(msg, Global.name + " - WAPP", logType));
        #endregion

        #region Read/Write Default Build Config

        public static string defConfigPath(string championId)
        {
            return Main.localBuild.BuildsFolder(championId) + "\\DefaultConfig.json";
        }

        public static void writeDefaultBuildConfig(string championId, DefaultBuildConfig config)
        {
            using (var streamWriter = new StreamWriter(defConfigPath(championId)))
            {
                var defConfig = JsonConvert.SerializeObject(config, Formatting.Indented);
                streamWriter.Write(defConfig);
            }
        }

        public static DefaultBuildConfig getDefaultBuildConfig(string championId)
        {
            var defaultConfig = new DefaultBuildConfig();
            using (var streamReader = new StreamReader(defConfigPath(championId)))
            {
                var json = streamReader.ReadToEnd();
                defaultConfig = JsonConvert.DeserializeObject<DefaultBuildConfig>(json);
            }
            return defaultConfig;
        }
        #endregion

        #region misc
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
            return $"pack://application:,,,/Resources/{name}.{format}";
        }

        public static string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        #endregion

        #region Animations
        public static class Animation
        {
            public static void FadeIn(FrameworkElement element, double duration = 0.2)
            {
                element.Visibility = Visibility.Visible;
                var sb = new Storyboard();
                DoubleAnimation fadeInAnimation = new DoubleAnimation()
                {
                    To = 1,
                    Duration = new Duration(TimeSpan.FromSeconds(duration)),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if (ConfigModel.config.LowSpecMode)
                    fadeInAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));

                Storyboard.SetTarget(fadeInAnimation, element);
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity", 1));
                fadeInAnimation.Completed += (s, _) => {
                    sb.Children.Clear();
                    sb = null;
                    fadeInAnimation = null;
                };
                sb.Children.Add(fadeInAnimation);
                sb.Begin(element);
                sb.Remove();
            }

            public static void FadeOut(FrameworkElement element, double duration = 0.2)
            {
                var sb = new Storyboard();
                DoubleAnimation fadeOutAnimation = new DoubleAnimation()
                {
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(duration)),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if (ConfigModel.config.LowSpecMode)
                    fadeOutAnimation.Duration = new Duration(TimeSpan.FromSeconds(0));

                Storyboard.SetTarget(fadeOutAnimation, element);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity", 0));
                fadeOutAnimation.Completed += (s, _) => {
                    element.Visibility = Visibility.Hidden;
                    sb.Children.Clear();
                    sb = null;
                    fadeOutAnimation = null;
                };
                sb.Children.Add(fadeOutAnimation);
                sb.Begin(element);
                sb.Remove();
            }

            public static void Margin(FrameworkElement element, Thickness thicknessFrom, Thickness thicknessTo, double duration = 0.2)
            {
                var sb = new Storyboard();
                var ta = new ThicknessAnimation
                {
                    BeginTime = new TimeSpan(0),
                    From = thicknessFrom,
                    To = thicknessTo,
                    FillBehavior = FillBehavior.HoldEnd,
                    Duration = new Duration(TimeSpan.FromSeconds(duration))
                };

                ta.Completed += (s, _) => {
                    sb.Children.Clear();
                    sb = null;
                    ta = null;
                };

                if (ConfigModel.config.LowSpecMode)
                    ta.Duration = new Duration(TimeSpan.FromSeconds(0));

                Storyboard.SetTarget(ta, element);
                Storyboard.SetTargetProperty(ta, new PropertyPath("Margin"));

                sb.Children.Add(ta);
                sb.Begin(element);
                sb.Remove();
            }
        }
        #endregion
    }
}
