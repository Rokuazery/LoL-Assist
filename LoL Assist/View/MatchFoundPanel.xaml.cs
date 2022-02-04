﻿using static LoL_Assist_WAPP.Model.LoLAWrapper;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Threading;
using LoLA.LCU.Objects;
using LoLA.LCU.Events;
using System.Windows;
using LoLA.LCU;
using System;
using LoLA;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for MatchFoundPanel.xaml
    /// </summary>
    public partial class MatchFoundPanel : UserControl
    {
        private readonly Window mainWnd = new Window();
        public MatchFoundPanel(Window mainWindow)
        {
            InitializeComponent();

            mainWnd = mainWindow;

            Opacity = 0;
            Visibility = Visibility.Hidden;
            phaseMonitor.PhaseChanged += Phase_Changed;
        }

        private void Phase_Changed(object sender, PhaseMonitor.PhaseChangedArgs e)
        {
            switch (e.currentPhase)
            {
                case Phase.ReadyCheck:
                    MatchFound();
                    break;
                case Phase.ChampSelect:
                    HideMatchFound(true);
                    break;
                default:
                    HideMatchFound();
                    break;
            }
        }

        bool IsFound = false, IsDecided = false;
        private async void MatchFound()
        {
            IsFound = true;
            IsDecided = false;

            Dispatcher.Invoke(new Action(() => {
                SetAaStatus(string.Empty, (Color)Application.Current.Resources["FontSecondaryColor"]);
                Utils.Animation.FadeIn(this);
                mainWnd.Topmost = true;
            }));

            int autoAcceptTimer = 5;
            while (IsFound)
            {
                var matchInfo = await LCUWrapper.GetMatchmakingInfo();
                var timer = matchInfo?.timer == null ? 0 : (int)matchInfo.timer;

                SetAaTime($"{10 - timer}s");
                if (!IsDecided)
                {
                    if (Model.ConfigModel.config.AutoAccept)
                    {
                        if (!(autoAcceptTimer <= timer))
                            SetAaStatus($"Auto Accept in {autoAcceptTimer - timer}s");
                        else Accept();
                    }
                    else SetAaStatus("Auto Accept is disabled!", Color.FromRgb(255, 196, 12));
                }

                if (matchInfo?.playerResponse == "Accepted")
                {
                    IsDecided = true;
                    SetAaStatus(matchInfo.playerResponse, Color.FromRgb(41, 171, 135));
                }
                else if (matchInfo?.playerResponse == "Declined")
                {
                    IsDecided = true;
                    SetAaStatus(matchInfo.playerResponse, Color.FromRgb(231, 72, 86));
                }

                if (Model.ConfigModel.config.LowSpecMode)
                    SetTimeoutBarValue(10 * timer);
                else
                {
                    Dispatcher.Invoke(() => {
                        Duration duration = new Duration(TimeSpan.FromSeconds(1));
                        DoubleAnimation dbAnimation = new DoubleAnimation(11 * timer, duration);
                        dbAnimation.Completed += (s, _) => { dbAnimation = null; };
                        pBar.BeginAnimation(RangeBase.ValueProperty, dbAnimation);
                    });
                }

                if (timer == 10) HideMatchFound();
                Thread.Sleep(1000);
            }
        }

        private async void Accept() => await LCUWrapper.AcceptMatchmakingAsync();
        private async void Decline() => await LCUWrapper.DeclineMatchmakingAsync(); 
        private void AcceptBtn_Click(object sender, RoutedEventArgs e) => Accept();
        private void DeclineBtn_Click(object sender, RoutedEventArgs e) => Decline();
        private void SetAaTime(string text) => Dispatcher.Invoke(() => { aCount.Text = text; });
        private void SetTimeoutBarValue(double value) => Dispatcher.Invoke(() => {  pBar.Value = value; });

        private void SetAaStatus(string text,Color color = default)
        {
            Dispatcher.Invoke(() => {
                aaStatus.Text = text;
                if (color == default)
                    aaStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                else
                    aaStatus.Foreground = new SolidColorBrush(color);
            });
        }

        private void HideMatchFound(bool topmost = false)
        {
            Dispatcher.Invoke(() => {
                pBar.BeginAnimation(RangeBase.ValueProperty, null); // clear the animation
                Utils.Animation.FadeOut(this);
                mainWnd.Topmost = topmost;

                if (IsFound) mainWnd.Activate(); // Activate the window to fix topmost

                IsDecided = false;
                IsFound = false;
                pBar.Value = 0;
            });
        }
    }
}
