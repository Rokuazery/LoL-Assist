using static LoL_Assist_WAPP.Model.LoLAWrapper;
using System.Collections.Generic;
using LoLA.WebAPIs.DataDragon;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using LoLA.LCU.Objects;
using LoLA.LCU.Events;
using LoLA.Objects;
using LoLA.Enums;
using LoLA.LCU;
using System;
using LoLA;

namespace LoL_Assist_WAPP.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _ChampionImage;
        public string ChampionImage
        {
            get { return _ChampionImage; }
            set
            {
                if (_ChampionImage != value)
                {
                    _ChampionImage = value;
                    OnPropertyChanged("ChampionImage");
                }
            }
        }

        private string _gMode;
        public string gMode
        {
            get { return _gMode; }
            set
            {
                if (_gMode != value)
                {
                    _gMode = value;
                    OnPropertyChanged("gMode");
                }
            }
        }

        private string _WorkingStatus;
        public string WorkingStatus
        {
            get { return _WorkingStatus; }
            set
            {
                if (_WorkingStatus != value)
                {
                    _WorkingStatus = value;
                    OnPropertyChanged("WorkingStatus");
                }
            }
        }

        private string _ConnectionStatus = "Disconnected.";
        public string ConnectionStatus
        {
            get { return _ConnectionStatus; }
            set
            {
                if (_ConnectionStatus != value)
                {
                    _ConnectionStatus = value;
                    OnPropertyChanged("ConnectionStatus");
                }
            }
        }

        private string _ChampionName;
        public string ChampionName
        {
            get { return _ChampionName; }
            set
            {
                if (_ChampionName != value)
                {
                    _ChampionName = value;
                    OnPropertyChanged("ChampionName");
                }
            }
        }

        private string _ImportStatus;
        public string ImportStatus
        {
            get { return _ImportStatus; }
            set
            {
                if (_ImportStatus != value)
                {
                    _ImportStatus = value;
                    OnPropertyChanged("ImportStatus");
                }
            }
        }

        private bool IsBusy = false;
        private bool IsLoLMonitoringPuased = false;
        private const string ProccName = "LeagueClient";
        private GameMode CurrentGameMode = GameMode.NONE;
        private Task<ChampionBD> CurrentChampionBuild = null;

        public MainViewModel()
        {
            InitLoLA();
        }

        private async void InitLoLA()
        {
            WorkingStatus = "Loading in configuration...";
            ConfigM.LoadConfig();

            WorkingStatus = "Downloading prerequisite...";
            while (!await Main.Init())
                Thread.Sleep(ConfigM.config.MonitoringDelay * 2);

            WorkingStatus = "Initializing LoLA components...";
            Global.Config.dDragonPatch = DataDragonWrapper.patchVersions[0];

            phaseMonitor.MonitorDelay = ConfigM.config.MonitoringDelay;
            phaseMonitor.IsMonitoring = true;
            phaseMonitor.InitPhaseMonitor();

            champMonitor.InitMonitor();
            champMonitor.IsMonitoring = true;
            champMonitor.ChampionChanged += Champion_Changed;

            IsLoLMonitoringPuased = true;
            var lolMonitorThread = new Thread(LoLMonitor);
            lolMonitorThread.Start();

            phaseMonitor.PhaseChanged += Phase_Changed;
            Phase_Changed(null, null); // Trigger init
        }

        private async void Phase_Changed(object sender, PhaseMonitor.PhaseChangedArgs e)
        {
            Phase phase = e == null ? Phase.None : e.CurrentPhase;

            if(phase != Phase.InProgress)
            {
                CurrentGameMode = await LCUWrapper.GetCurrentGameModeAsync();
                gMode = $"Game Mode: {CurrentGameMode}";
            }

            switch (phase)
            {
                case Phase.ChampSelect:
                    ImportStatus = "Selecting a champion...";
                    break;
                case Phase.WaitingForStats:
                    ResetStatus();
                    var summonerInfo = await LCUWrapper.GetCurrentSummonerAsync();
                    ConnectionStatus = $"{summonerInfo.displayName} | Lvl {summonerInfo.summonerLevel}";
                    break;
                case Phase.None:
                case Phase.Lobby:
                    ResetStatus();
                    break;
                case Phase.InProgress:
                    ImportStatus = "Game in progress...";
                    break;
                case Phase.Matchmaking:
                    ClearChamp();
                    ImportStatus = "Queueing...";
                    break;
            }
        }

        private void ClearChamp()
        {
            ChampionImage = null;
            ChampionName = string.Empty;
        }

        private void ResetStatus()
        {
            ClearChamp();
            ImportStatus = "Waiting for a game queue...";
        }

        private async void Champion_Changed(object sender, ChampionMonitor.ChampionChangedArgs e)
        {
            if (!string.IsNullOrEmpty(e.Name))
            {
                ClearChamp();

                ChampionName = e.Name;

                while (IsBusy)
                    Thread.Sleep(ConfigM.config.MonitoringDelay);

                bool RB_IsSuccessFull = false, SC_IsSuccessFull = false;
                if (ConfigM.config.AutoRune || ConfigM.config.AutoSpells)
                {
                    IsBusy = true;

                    var CurrentChampionId = DataConverter.ChampionNameToId(ChampionName);

                    ImportStatus = "Fetching Builds...";

                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();

                    var currentPerks = await LCUWrapper.GetCurrentRunePageAsync();

                    Task<string> champImage;

                    await Task.WhenAll(CurrentChampionBuild = Main.RequestBuildsData(CurrentChampionId, CurrentGameMode, BuildsProvider.Metasrc),
                    champImage = DataDragonWrapper.GetChampionImage(CurrentChampionId, Global.Config.dDragonPatch));

                    ChampionImage = await champImage;
                    ImportStatus = $"Importing {Utils.FixedName(ChampionName)} Builds...";

                    RuneObj RB = (await CurrentChampionBuild).rune;
                    SpellObj SC = (await CurrentChampionBuild).spell;

                    if (CurrentChampionBuild != null)
                    {
                        List<Task> ImportTasks = new List<Task>();

                        if (ConfigM.config.AutoRune && RB != null)
                        {
                            ImportTasks.Add(SetRuneAsync(RB, currentPerks));
                            RB_IsSuccessFull = true;
                        }

                        if (ConfigM.config.AutoSpells && SC != null)
                        {
                            ImportTasks.Add(ImportSpellsAsync(SC, CurrentGameMode));
                            SC_IsSuccessFull = true;
                        }

                        Parallel.ForEach(ImportTasks, async task => { await task; });
                    }

                    sw.Stop();

                    if (RB_IsSuccessFull && SC_IsSuccessFull)
                        ImportStatus = $"Builds has been imported successfully! Elapsed [{sw.ElapsedMilliseconds}ms]";
                    else ImportStatus = $"Uh Oh, something went wrong.\ntry deleting {Utils.FixedName(ChampionName)} data might fix the issue.";

                    IsBusy = false;
                    currentPerks = null;
                }
            }
        }

        public void LoLMonitor()
        {
            CheckLoL();

            while (true)
            {
                if (!IsLoLMonitoringPuased)
                {
                    if (Process.GetProcessesByName(ProccName).Length == 0)
                    {
                        ConnectionStatus = "Disconnected.";
                        IsLoLMonitoringPuased = true;
                        CheckLoL();
                    }
                }
                Thread.Sleep(ConfigM.config.MonitoringDelay * 3);
            }
        }

        public async void CheckLoL()
        {
            WorkingStatus = "Waiting for League to start...";
            while (!await LCUWrapper.InitAsync())
                Thread.Sleep(ConfigM.config.MonitoringDelay * 2);

            var summonerInfo = new Summoner();

            while (string.IsNullOrEmpty(summonerInfo?.displayName))
            {
                summonerInfo = await LCUWrapper.GetCurrentSummonerAsync();
                Thread.Sleep(ConfigM.config.MonitoringDelay * 2);
            }

            ConnectionStatus = $"{summonerInfo.displayName} | Lvl {summonerInfo.summonerLevel}";
            WorkingStatus = string.Empty;

            IsLoLMonitoringPuased = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
