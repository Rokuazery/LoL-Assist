using static LoL_Assist_WAPP.Model.LoLAWrapper;
using System.Collections.Generic;
using LoLA.WebAPIs.DataDragon;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using System.Threading;
using LoLA.LCU.Objects;
using LoLA.LCU.Events;
using System.Windows;
using LoLA.Objects;
using LoLA.Enums;
using LoLA.Utils;
using LoLA.LCU;
using System;
using LoLA;

namespace LoL_Assist_WAPP.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ICommand ManualImportCommand { get; set; }

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

        private Visibility _ManualImportVisibility = Visibility.Hidden;
        public Visibility ManualImportVisibility
        {
            get { return _ManualImportVisibility; }
            set
            {
                if (_ManualImportVisibility != value)
                {
                    _ManualImportVisibility = value;
                    OnPropertyChanged("ManualImportVisibility");
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
            ManualImportCommand = new RelayCommand(ManualImportExecute, o => true);
            InitLoLA();
        }

        private async void InitLoLA()
        {
            Log.Clear();
            WorkingStatus = "Loading in configuration...";
            ConfigM.LoadConfig();

            if(ConfigM.config.UpdateOnStartup)
            {
                WorkingStatus = "Checking for updates...";
                await Update.Start();
            }

            WorkingStatus = "Downloading prerequisite...";
            while (!await Main.Init())
                Thread.Sleep(ConfigM.config.MonitoringDelay * 2);

            WorkingStatus = "Initializing LoLA components...";
            Global.Config.dDragonPatch = DataDragonWrapper.patchVersions[0];

            phaseMonitor.InitPhaseMonitor(); 
            phaseMonitor.MonitorDelay = ConfigM.config.MonitoringDelay;

            champMonitor.InitChampionMonitor();
            champMonitor.ChampionChanged += Champion_Changed;
            champMonitor.MonitorDelay = ConfigM.config.MonitoringDelay;

            IsLoLMonitoringPuased = true;
            var lolMonitorThread = new Thread(LoLMonitor);
            lolMonitorThread.Start();

            phaseMonitor.PhaseChanged += Phase_Changed;
            ResetStatus();
        }

        private async void Phase_Changed(object sender, PhaseMonitor.PhaseChangedArgs e)
        {
            Phase phase = e == null ? Phase.None : e.CurrentPhase;
            CurrentGameMode = await LCUWrapper.GetCurrentGameModeAsync();

            if (phase != Phase.InProgress)
                gMode = $"Game Mode: {CurrentGameMode}";

            switch (phase)
            {
                case Phase.ChampSelect:
                    ImportStatus = "Selecting a champion...";
                    ManualImportVisibility = Visibility.Visible;
                    break;
                case Phase.None:
                case Phase.Lobby:
                case Phase.WaitingForStats:
                    ResetStatus();
                    break;
                case Phase.EndOfGame:
                    ResetStatus();
                    var summonerInfo = await LCUWrapper.GetCurrentSummonerAsync();
                    ConnectionStatus = $"{summonerInfo.displayName} | Lvl {summonerInfo.summonerLevel}";
                    break;
                case Phase.InProgress:
                    ImportStatus = "Game in progress...";
                    ManualImportVisibility = Visibility.Hidden;
                    break;
                case Phase.Matchmaking:
                    ClearChamp();
                    ImportStatus = "Queueing...";
                    break;
            }
        }

        private void Champion_Changed(object sender, ChampionMonitor.ChampionChangedArgs e) => ImportBuilds(e.Name);
        private async void ManualImportExecute(object e) => ImportBuilds(await LCUWrapper.GetCurrentChampionAsyncV2());

        private void ClearChamp(bool hideManualImport = true)
        {
            ChampionImage = null;
            ChampionName = string.Empty;

            if(hideManualImport)
                ManualImportVisibility = Visibility.Hidden;
        }

        private void ResetStatus()
        {
            ClearChamp();
            ManualImportVisibility = Visibility.Hidden;
            ImportStatus = "Waiting for a game queue...";
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
                Thread.Sleep(ConfigM.config.MonitoringDelay * 2);
            }
        }

        public async void CheckLoL()
        {
            WorkingStatus = "Waiting for League to start...";
            var summonerInfo = new Summoner();
            while (string.IsNullOrEmpty(summonerInfo?.displayName))
            {
                summonerInfo = await LCUWrapper.GetCurrentSummonerAsync();
                Thread.Sleep(ConfigM.config.MonitoringDelay * 2);
            }

            WorkingStatus = string.Empty;
            ConnectionStatus = $"{summonerInfo.displayName} | Lvl {summonerInfo.summonerLevel}";

            IsLoLMonitoringPuased = false;
        }

        private async void ImportBuilds(string championName)
        {
            if (!string.IsNullOrEmpty(championName))
            {
                ClearChamp(false);

                while (IsBusy)
                    Thread.Sleep(ConfigM.config.MonitoringDelay);

                IsBusy = true;
                try
                {
                    bool RB_IsSuccessFull = false, SC_IsSuccessFull = false;
                    if (ConfigM.config.AutoRune || ConfigM.config.AutoSpells)
                    {
                        Task<string> champImage;
                        List<Task> tasks = new List<Task>();

                        ChampionName = championName;
                        var CurrentChampionId = DataConverter.ChampionNameToId(ChampionName);

                        ImportStatus = $"Fetching Builds...";

                        Stopwatch sw = Stopwatch.StartNew();
                        sw.Start();

                        tasks.Add(CurrentChampionBuild = Main.RequestBuildsData(CurrentChampionId, CurrentGameMode, BuildsProvider.Metasrc));
                        tasks.Add(champImage = DataDragonWrapper.GetChampionImage(CurrentChampionId, Global.Config.dDragonPatch));
                        Parallel.ForEach(tasks, async task => { await task; });

                        ChampionImage = await champImage;

                        ImportStatus = $"Importing {Utils.FixedName(ChampionName)} Builds...";
                        Log.Append(ImportStatus = $"Importing {Utils.FixedName(ChampionName)} Builds...");

                        RuneObj RB = (await CurrentChampionBuild)?.rune;
                        SpellObj SC = (await CurrentChampionBuild)?.spell;

                        if (CurrentChampionBuild != null)
                        {
                            var currentPerks = await LCUWrapper.GetCurrentRunePageAsync();
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
                            currentPerks = null;
                        }

                        sw.Stop();

                        if (RB_IsSuccessFull && SC_IsSuccessFull)
                            ImportStatus = $"Builds has been imported successfully! Elapsed [{sw.ElapsedMilliseconds}ms]";
                        else ImportStatus = $"Uh Oh, something went wrong.\ntry deleting {Utils.FixedName(ChampionName)} data might fix the issue.";

                    }
                }
                catch 
                {
                    Log.Append("Uh oh, something went wrong with LoL Assist.");
                }
                IsBusy = false;
            }
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
