using LoLA;
using System;
using System.Windows;
using System.Threading;
using LoLA.LeagueClient;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using static LoL_Assist_WAPP.Model.LoLAWrapper;

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
            PhaseMonitor.PhaseChanged += Phase_Changed;
        }

        private async void Phase_Changed(object sender, GameFlowMonitor.PhaseChangedArgs e)
        {
            switch (e.CurrentPhase)
            {
                case Phase.ReadyCheck:
                    await MatchFound();
                    break;
                case Phase.ChampSelect:
                    Dispatcher.Invoke(new Action(() => { mainWnd.Topmost = true; }));
                    break;
                default:
                    Dispatcher.Invoke(new Action(() => { MatchCanceled(); }));
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
                    Duration duration = new Duration(TimeSpan.FromSeconds(11));
                    DoubleAnimation dbAnimation = new DoubleAnimation(100, duration);
                    dbAnimation.Completed += (s, _) => { dbAnimation = null; pBar.BeginAnimation(ProgressBar.ValueProperty, null); };
                    pBar.BeginAnimation(ProgressBar.ValueProperty, dbAnimation);
                }

                mainWnd.Topmost = true;
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
                                else AcceptMatch();
                            }
                            else
                            {
                                aaStatus.Text = $"Auto Accept Is Disabled!";
                                aaStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 196, 12));
                            }
                        }

                        if (i == 11) MatchCanceled();

                        if (Model.ConfigM.config.LowSpecMode)
                            pBar.Value += 9.090909090909091;
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
            Utils.Animation.FadeOut(this, 200);
            mainWnd.Topmost = false;
            mainWnd.Activate(); // Activate the window to fix topmost
            IsDecided = false;
            IsFound = false;
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e) => AcceptMatch();
        private void DeclineBtn_Click(object sender, RoutedEventArgs e) => DeclineMatch();
    }
}
