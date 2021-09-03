using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using LoL_Assist_WAPP.Model;

namespace LoL_Assist_WAPP
{
    public static class Utils
    {
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

        public static class Animation
        {
            public static void FadeIn(UserControl userControl, double duration = 250)
            {
                userControl.Visibility = Visibility.Visible;
                var sb = new Storyboard();
                DoubleAnimation fadeInAnimation = new DoubleAnimation() {
                    To = 1,
                    Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if (ConfigM.config.LowSpecMode)
                    fadeInAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(0));

                Storyboard.SetTarget(fadeInAnimation, userControl);
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity", 1));
                fadeInAnimation.Completed += (s, _) => { 
                   sb.Children.Clear();
                   sb = null; 
                   fadeInAnimation = null;
                };
                sb.Children.Add(fadeInAnimation);
                sb.Begin(userControl);
                sb.Remove();
            }

            public static void FadeOut(UserControl userControl, double duration = 250)
            {
                var sb = new Storyboard();
                DoubleAnimation fadeOutAnimation = new DoubleAnimation() {
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if ( ConfigM.config.LowSpecMode)
                    fadeOutAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(0));

                Storyboard.SetTarget(fadeOutAnimation, userControl);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity", 0));
                fadeOutAnimation.Completed += (s, _) => {
                    userControl.Visibility = Visibility.Hidden;
                    sb.Children.Clear();
                    sb = null;
                    fadeOutAnimation = null;
                };
                sb.Children.Add(fadeOutAnimation);
                sb.Begin(userControl);
                sb.Remove();
            }
        }
    }
}
