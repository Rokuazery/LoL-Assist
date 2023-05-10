using LoLA.Networking.LCU.Events;
using LoLA.Networking.LCU.Enums;
using LoL_Assist_WAPP.Commands;
using System.Windows.Controls;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Models;
using System.ComponentModel;
using System.Windows.Input;
using LoLA.Networking.LCU;
using System.Threading;
using System.Windows;
using System;

namespace LoL_Assist_WAPP.ViewModels
{
    public class MatchFoundViewModel: ViewModelBase
    {
        #region Commands
        public ICommand AcceptMatchCommand { get; }
        public ICommand DeclineMatchCommand { get; }

        private ICommand _hideUserControlCommand;
        public ICommand HideUserControlCommand
        {
            get
            {
                if (_hideUserControlCommand == null)
                {
                    _hideUserControlCommand = new Command(p => HideUserControl(p));
                }
                return _hideUserControlCommand;
            }
        }
        #endregion

        private string _timeoutTimer;
        public string TimeoutTimer
        {
            get => _timeoutTimer;
            set
            {
                if (_timeoutTimer != value)
                {
                    _timeoutTimer = value;
                    OnPropertyChanged(nameof(TimeoutTimer));
                }
            }
        }

        private string _acceptStatus = "Auto Accept in 5s";
        public string AcceptStatus
        {
            get => _acceptStatus;
            set
            {
                if (_acceptStatus != value)
                {
                    _acceptStatus = value;
                    OnPropertyChanged(nameof(AcceptStatus));
                }
            }
        }

        private double? _timeoutValue;
        public double? TimeoutValue
        {
            get => _timeoutValue;
            set
            {
                if (_timeoutValue != value)
                {
                    _timeoutValue = value;
                    OnPropertyChanged(nameof(TimeoutValue));
                }
            }
        }

        private Visibility _matchfoundVisibility;
        public Visibility MatchfoundVisibility
        {
            get => _matchfoundVisibility;
            set
            {
                if (_matchfoundVisibility != value)
                {
                    _matchfoundVisibility = value;
                    OnPropertyChanged(nameof(MatchfoundVisibility));
                }
            }
        }

        private readonly int r_autoAcceptTimer = 5; // seconds
        private bool isMatchFound = false;
        private bool isDecided = false;

        public MatchFoundViewModel()
        {
            MatchFound();
            AcceptMatchCommand = new Command(_ => { Accept(); }); 
            DeclineMatchCommand = new Command(_ => { Decline(); });
            LoLAWrapper.s_PhaseMonitor.PhaseChanged += Phase_Changed;
        }

        private void Phase_Changed(object sender, PhaseMonitor.PhaseChangedArgs e) 
        {
            if (e.currentPhase != Phase.ReadyCheck) CloseMatchFound();
        }

        private void CloseMatchFound()
        {
            LoLAWrapper.s_PhaseMonitor.PhaseChanged -= Phase_Changed;
            MatchfoundVisibility = Visibility.Collapsed;
            isMatchFound = false;
            isDecided = false;
        }

        private async void MatchFound()
        {
            isMatchFound = true;
            isDecided = false;

            ConsoleBeep();

            while (isMatchFound)
            {
                var matchInfo = await LCUWrapper.GetMatchmakingInfo();
                var timer = matchInfo?.timer == null ? 0 : (int)matchInfo.timer;

                // update UI timer
                TimeoutTimer = $"{10 - timer}s";
                if (!isDecided)
                {
                    if (ConfigModel.s_Config.AutoAccept)
                    {
                        if (r_autoAcceptTimer <= timer) Accept();
                        else AcceptStatus = $"Auto Accept in {r_autoAcceptTimer - timer}s";
                    }
                    else AcceptStatus = "Auto Accept is disabled";
                }

                if (matchInfo?.playerResponse != "None")
                    AcceptStatus = matchInfo?.playerResponse;

                TimeoutValue = timer * 10;

                await Task.Delay(500);
            }
        }

        private async void Accept() => isDecided = await LCUWrapper.AcceptMatchmakingAsync();
        private async void Decline() => isDecided = await LCUWrapper.DeclineMatchmakingAsync();

        private void ConsoleBeep() // Beep sound
        {
            Console.Beep();
            Console.Beep();
        }

        private void HideUserControl(object p) => Utils.Animation.FadeOut(p as UserControl);
    }
}
