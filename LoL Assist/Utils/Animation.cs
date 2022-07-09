using System.Windows.Media.Animation;
using LoL_Assist_WAPP.Model;
using System.Windows;
using System;

namespace LoL_Assist_WAPP.Utils
{
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

            if (ConfigModel.s_Config.LowSpecMode)
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

            if (ConfigModel.s_Config.LowSpecMode)
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

            if (ConfigModel.s_Config.LowSpecMode)
                ta.Duration = new Duration(TimeSpan.FromSeconds(0));

            Storyboard.SetTarget(ta, element);
            Storyboard.SetTargetProperty(ta, new PropertyPath("Margin"));

            sb.Children.Add(ta);
            sb.Begin(element);
            sb.Remove();
        }
    }
}
