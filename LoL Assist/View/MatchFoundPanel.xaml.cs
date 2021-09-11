using static LoL_Assist_WAPP.Model.LoLAWrapper;
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
        private Window mainWnd = new Window();
        public MatchFoundPanel(Window mainWindow)
        {
            InitializeComponent();

            mainWnd = mainWindow;

            Opacity = 0;
            Visibility = Visibility.Hidden;
            phaseMonitor.PhaseChanged += Phase_Changed;
        }

        private async void Phase_Changed(object sender, PhaseMonitor.PhaseChangedArgs e)
        {
            switch (e.CurrentPhase)
            {
                case Phase.ReadyCheck:
                    await MatchFound();
                    break;
                case Phase.ChampSelect:
                    Dispatcher.Invoke(new Action(() => { HideMatchFound(true); }));
                    break;
                default:
                    Dispatcher.Invoke(new Action(() => { HideMatchFound(); }));
                    break;
            }
        }

        bool IsFound = false, IsDecided = false;
        private async Task MatchFound()
        {
            IsFound = true;
            IsDecided = false;

            Dispatcher.Invoke(new Action(() => {
                aaStatus.Text = string.Empty;
                aaStatus.Foreground = (SolidColorBrush)Application.Current.Resources["FontSecondaryBrush"];

                Utils.Animation.FadeIn(this, 200);
                if (!Model.ConfigM.config.LowSpecMode)
                {
                    Duration duration = new Duration(TimeSpan.FromSeconds(10));
                    DoubleAnimation dbAnimation = new DoubleAnimation(100, duration);
                    dbAnimation.Completed += (s, _) => { dbAnimation = null; pBar.BeginAnimation(ProgressBar.ValueProperty, null); };
                    pBar.BeginAnimation(ProgressBar.ValueProperty, dbAnimation);
                }
                mainWnd.Topmost = true;
            }));

            int a = 5;
            await Task.Run(() => {
                while (IsFound) 
                {
                    var matchInfo = LCUWrapper.GetMatchmakingInfo().Result;
                    Dispatcher.Invoke(() => {
                        aCount.Text = $"{10 - matchInfo?.timer}s";
                        if (!IsDecided)
                        {
                            if(Model.ConfigM.config.AutoAccept)
                            {
                                if (!(a <= 1))
                                {
                                    a = a - (int)matchInfo?.timer;
                                    aaStatus.Text = $"Auto Accept In {a}s";
                                }
                                else Accept();
                            }
                            else
                            {
                                aaStatus.Text = $"Auto Accept Is Disabled!";
                                aaStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 196, 12));
                            }
                        }

                        if (matchInfo.playerResponse == "Accepted")
                        {
                            IsDecided = true;
                            aaStatus.Text = matchInfo.playerResponse;
                            aaStatus.Foreground = new SolidColorBrush(Color.FromRgb(41, 171, 135));
                        }
                        else if(matchInfo.playerResponse == "Declined")
                        {
                            IsDecided = true;
                            aaStatus.Text = matchInfo.playerResponse;
                            aaStatus.Foreground = new SolidColorBrush(Color.FromRgb(231, 72, 86));
                        }

                        if (matchInfo.timer == 10) HideMatchFound();

                        if (Model.ConfigM.config.LowSpecMode)
                            pBar.Value += 9.090909090909091;
                    });
                    Thread.Sleep(1000);
                }
            });
        }

        private async void Accept()
        {
            await LCUWrapper.AcceptMatchmakingAsync();
        }

        private async void Decline()
        {
            await LCUWrapper.DeclineMatchmakingAsync();
        }

        private void HideMatchFound(bool topmost = false)
        {
            pBar.BeginAnimation(ProgressBar.ValueProperty, null); // clear the animation
            Utils.Animation.FadeOut(this, 200);
            mainWnd.Topmost = topmost;

            if(IsFound)
                mainWnd.Activate(); // Activate the window to fix topmost

            IsDecided = false;
            IsFound = false;
            pBar.Value = 0;
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e) => Accept();
        private void DeclineBtn_Click(object sender, RoutedEventArgs e) => Decline();
    }
}
