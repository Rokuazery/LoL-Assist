using System;
using System.Collections.Generic;
using System.Linq;
using LoLA;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using static LoL_Assist_WAPP.Model.LoLAWrapper;
using LoLA.LeagueClient;
using System.Threading;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for MatchFoundPanel.xaml
    /// </summary>
    public partial class MatchFoundPanel : UserControl
    {
        public MatchFoundPanel()
        {
            InitializeComponent();
            PhaseMonitor.PhaseChanged += Phase_Changed;
        }

        private async void Phase_Changed(object sender, GameFlowMonitor.PhaseChangedArgs e)
        {
            if (e.CurrentPhase == Phase.ReadyCheck) await MatchFound();
            else Dispatcher.Invoke(new Action(() => { MatchCanceled(); }));
        }

        bool IsFound = false, IsDecided = false;
        private async Task MatchFound()
        {
            IsFound = true;
            IsDecided = false;

            Dispatcher.Invoke(new Action(() => {
                aaStatus.Text = string.Empty;
                aaStatus.Foreground = (SolidColorBrush)Application.Current.Resources["FontSecondaryBrush"];

                Utils.Animation.In(this, 200);
                Duration duration = new Duration(TimeSpan.FromSeconds(11));
                DoubleAnimation doubleanimation = new DoubleAnimation(100, duration);
                pBar.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
            }));

            int i = 0, a = 5;
            await Task.Run(() => {
                while (IsFound) 
                {
                    Dispatcher.Invoke(() => {
                        aCount.Text = $"{11 - i}s";
                        if (!IsDecided)
                        {
                            if(Model.ConfigM.config.AutoAccept)
                            {
                                if (a != 0)
                                {
                                    a--;
                                    aaStatus.Text = $"Auto Accept In {a}s";
                                }

                                if (a == 0) AcceptMatch();
                            }
                            else
                            {
                                aaStatus.Text = $"Auto Accept Is Disabled!";
                                aaStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 196, 12));
                            }
                        }

                        if (i == 11)
                        {
                            IsFound = false;
                            Utils.Animation.Out(this, 200);
                        }
                    });
                    
                    i++;
                    Thread.Sleep(1000);
                }
            });
        }

        private async void AcceptMatch()
        {
            await Main.leagueClient.AcceptMatchmakingAsync();

            IsDecided = true;
            aaStatus.Text = "ACCEPTED";
            aaStatus.Foreground = new SolidColorBrush(Color.FromRgb(41, 171, 135));
        }

        private async void DeclineMatch()
        {
            await Main.leagueClient.DeclineMatchmakingAsync();

            IsDecided = true;
            aaStatus.Text = "DECLINED";
            aaStatus.Foreground = new SolidColorBrush(Color.FromRgb(231, 72, 86));
        }

        private void MatchCanceled()
        {
            Utils.Animation.Out(this, 200);
            IsDecided = false;
            IsFound = false;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Visibility == Visibility.Visible)
            {
                Storyboard storyboard= new Storyboard();

                DoubleAnimation valueAnim = new DoubleAnimation()
                {
                    To = 100,
                    Duration = new Duration(TimeSpan.FromSeconds(10)),
                    FillBehavior = FillBehavior.HoldEnd
                };

                Storyboard.SetTarget(valueAnim, pBar);
                Storyboard.SetTargetProperty(valueAnim, new PropertyPath("Value", 100));
                storyboard.Begin(this);
            }
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e) => AcceptMatch();
        private void DeclineBtn_Click(object sender, RoutedEventArgs e) => DeclineMatch();
    }
}
