using System.Windows.Controls;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading;
using LoLA.LCU.Events;
using LoLA.LCU;
using System;
using System.Windows;

namespace LoL_Assist_WAPP.ViewModel
{
    public class MatchFoundViewModel : INotifyPropertyChanged
    {
        #region Commands
        public ICommand AcceptMatchCommand { get; }
        public ICommand DeclineMatchCommand { get; }

        private ICommand hideUserControlCommand;
        public ICommand HideUserControlCommand
        {
            get
            {
                if (hideUserControlCommand == null)
                {
                    hideUserControlCommand = new Command(p => HideUserControl(p));
                }
                return hideUserControlCommand;
            }
        }
        #endregion

        private string timeoutTimer;
        public string TimeoutTimer
        {
            get => timeoutTimer;
            set
            {
                if (timeoutTimer != value)
                {
                    timeoutTimer = value;
                    OnPropertyChanged(nameof(TimeoutTimer));
                }
            }
        }

        private string acceptStatus = "Auto Accept in 5s";
        public string AcceptStatus
        {
            get => acceptStatus;
            set
            {
                if (acceptStatus != value)
                {
                    acceptStatus = value;
                    OnPropertyChanged(nameof(AcceptStatus));
                }
            }
        }

        private double? timeoutValue;
        public double? TimeoutValue
        {
            get => timeoutValue;
            set
            {
                if (timeoutValue != value)
                {
                    timeoutValue = value;
                    OnPropertyChanged(nameof(TimeoutValue));
                }
            }
        }

        private Visibility matchfoundVisibility;
        public Visibility MatchfoundVisibility
        {
            get => matchfoundVisibility;
            set
            {
                if (matchfoundVisibility != value)
                {
                    matchfoundVisibility = value;
                    OnPropertyChanged(nameof(MatchfoundVisibility));
                }
            }
        }

        private readonly int _autoAcceptTimer = 5;
        private bool _isMatchFound = false;
        private bool _isDecided = false;
        public MatchFoundViewModel()
        {
            MatchFound();
            AcceptMatchCommand = new Command(o => { Accept(); }); 
            DeclineMatchCommand = new Command(o => { Decline(); });
            LoLAWrapper.phaseMonitor.PhaseChanged += Phase_Changed;
        }

        private void Phase_Changed(object sender, PhaseMonitor.PhaseChangedArgs e) 
        {
            if (e.currentPhase != Phase.ReadyCheck) HideMatchFound();
        }

        private void HideMatchFound()
        {
            MatchfoundVisibility = Visibility.Collapsed;
            _isMatchFound = false;
            _isDecided = false;
        }

        private async void MatchFound()
        {
            await Task.Run(async() => {
                _isMatchFound = true;
                _isDecided = false;

                ConsoleBeep();

                while (_isMatchFound)
                {
                    var matchInfo = await LCUWrapper.GetMatchmakingInfo();
                    var timer = matchInfo?.timer == null ? 0 : (int)matchInfo.timer;

                    // update timer UI
                    TimeoutTimer = $"{10 - timer}s";
                    if (!_isDecided)
                    {
                        if (ConfigModel.config.AutoAccept)
                        {
                            if (_autoAcceptTimer <= timer) Accept();
                            else AcceptStatus = $"Auto Accept in {_autoAcceptTimer - timer}s";
                        } else AcceptStatus = "Auto Accept is disabled";
                    }

                    if (matchInfo?.playerResponse != "None")
                        AcceptStatus = matchInfo.playerResponse;

                    TimeoutValue = timer * 10;

                    //if (timer == 10) HideMatchFound();
                    Thread.Sleep(1000);
                }
            });
        }

        private async void Accept() => _isDecided = await LCUWrapper.AcceptMatchmakingAsync();
        private async void Decline() => _isDecided = await LCUWrapper.DeclineMatchmakingAsync();

        private void ConsoleBeep() // Beep sound
        {
            Console.Beep();
            Console.Beep();
        }

        private void HideUserControl(object p) => Utils.Animation.FadeOut(p as UserControl);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
