using System.Windows.Media.Animation;
using LoL_Assist_WAPP.Model;
using System.Windows;
using System;

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

        public static string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        public static class Animation
        {
            public static void FadeIn(FrameworkElement element, double duration = 250)
            {
                element.Visibility = Visibility.Visible;
                var sb = new Storyboard();
                DoubleAnimation fadeInAnimation = new DoubleAnimation() {
                    To = 1,
                    Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if (ConfigM.config.LowSpecMode)
                    fadeInAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(0));

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

            public static void FadeOut(FrameworkElement element, double duration = 250)
            {
                var sb = new Storyboard();
                DoubleAnimation fadeOutAnimation = new DoubleAnimation() {
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
                    FillBehavior = FillBehavior.HoldEnd
                };

                if ( ConfigM.config.LowSpecMode)
                    fadeOutAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(0));

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
                var ta = new ThicknessAnimation();
                ta.BeginTime = new TimeSpan(0);

                ta.From = thicknessFrom;
                ta.To = thicknessTo;
                ta.FillBehavior = FillBehavior.HoldEnd;
                ta.Duration = new Duration(TimeSpan.FromSeconds(duration));
                ta.Completed += (s, _) =>{
                    sb.Children.Clear();
                    sb = null;
                    ta = null;
                };

                if (ConfigM.config.LowSpecMode)
                    ta.Duration = new Duration(TimeSpan.FromSeconds(0));

                Storyboard.SetTarget(ta, element);
                Storyboard.SetTargetProperty(ta, new PropertyPath("Margin"));

                sb.Children.Add(ta);
                sb.Begin(element);
                sb.Remove();
            }
        }
    }
}
