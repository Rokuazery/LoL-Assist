using LoLA;
using System.Threading;
using LoLA.LeagueClient;
using LoLA.Data.Objects;
using System.Diagnostics;
using System.ComponentModel;
using LoL_Assist_WAPP.Model;
using static LoL_Assist_WAPP.Model.LoLAWrapper;

namespace LoL_Assist_WAPP.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _AppName;
        public string AppName
        {
            get { return _AppName; }
            set
            {
                if (_AppName != value)
                {
                    _AppName = value;
                    OnPropertyChanged("AppName");
                }
            }
        }

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

        private string _ConnectionStatus = "Not Connected!";
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
        public GameFlowMonitor PhaseMonitor = new GameFlowMonitor(Main.leagueClient);
        public ChampionMonitor ChampMonitor = new ChampionMonitor(Main.leagueClient);

        public MainViewModel()
        {
            AppName = "LoL Assist";
            InitLoLA();
        }

        private async void InitLoLA()
        {
            WorkingStatus = "Loading in Configuration...";
            ConfigM.LoadConfig();

            WorkingStatus = "Downloading Prerequisite...";
            while (!await Main.init())
                Thread.Sleep(600);

            WorkingStatus = "Initializing LoLA Components...";
            GlobalConfig.DataDragonPatch = Main.dataWrapper.Versions[0];
            PhaseMonitor.MonitorDelay = ConfigM.config.MonitoringDelay;
            PhaseMonitor.IsMonitoring = true;
            PhaseMonitor.Init_GameFlowMonitor();

            ChampMonitor.InitMonitor();
            ChampMonitor.IsMonitoring = true;
            ChampMonitor.ChampionChanged += Champion_Changed;

            IsLoLMonitoringPuased = true;
            new Thread(LoLMonitor).Start();

            ImportStatus = "Waiting For a Game Queue...";
            PhaseMonitor.PhaseChanged += Phase_Changed;
        }

        private async void Phase_Changed(object sender, GameFlowMonitor.PhaseChangedArgs e)
        {
            switch (e.CurrentPhase)
            {
                case Phase.ChampSelect:
                    // Update The current champion for ChampionMonitor
                    ImportStatus = "Selecting a Champion...";
                    ChampMonitor.CurrentPhase = e.CurrentPhase; 
                    break;
                case Phase.ReadyCheck:
                    if(ConfigM.config.AutoAccept)
                        await Main.leagueClient.AcceptMatchmakingAsync();
                    ImportStatus = "Match Accepted!";
                    break;
                case Phase.None:
                case Phase.WaitingForStats:
                case Phase.Lobby:
                    // Update status here
                    ChampionImage = null;
                    ChampionName = string.Empty;
                    ImportStatus = "Waiting For a Game Queue...";
                    break;
                case Phase.InProgress:
                    ImportStatus = "Game in Progress...";
                    break;
                case Phase.Matchmaking:
                    ImportStatus = "Queueing...";
                    break;
            }
        }

        ChampionBuild CurrentChampionBuild = null;
        private async void Champion_Changed(object sender, ChampionMonitor.ChampionChangedArgs e)
        {
            ChampionImage = null;
            ChampionName = string.Empty;
            while (IsBusy)
                Thread.Sleep(ConfigM.config.MonitoringDelay);

            ChampionName = e.Name;

            bool IsSuccessFull = false;
            if (ConfigM.config.AutoRune || ConfigM.config.AutoSpells)
            {
                ChampionName = e.Name;
                var CurrentChampionId = DataConverter.ChampionNameToId(e.Name, Main.dataWrapper.Champions);

                ImportStatus = "Fetching Builds...";

                IsBusy = true;

                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();
                var currentPerks = await Main.leagueClient.GetCurrentRunePageAsync();
                var CurrentGameMode = await Main.leagueClient.GetCurrentGameModeAsync();

                CurrentChampionBuild = await Main.RequestDataAsync(CurrentChampionId, StatRequest.Metasrc, GlobalConfig.DataDragonPatch, CurrentGameMode);

                ImportStatus = $"Importing {Utils.FixedName(e.Name)} Builds...";

                if (ConfigM.config.AutoRune)
                {
                    await SetRune(CurrentChampionBuild.RuneBuild, currentPerks, true);
                    IsSuccessFull = true;
                }

                if (ConfigM.config.AutoSpells)
                {
                    ImportSpells(CurrentChampionBuild.SpellCombo, CurrentGameMode);
                    IsSuccessFull = true;
                }

                sw.Stop();

                if (IsSuccessFull)
                    ImportStatus = $"Builds Has Been Imported Successfully! Elapsed [{sw.ElapsedMilliseconds}ms]";
                else
                    ImportStatus = "No Builds Found For The Current Champion. Sorry D:";

                ChampionImage = await Main.dataWrapper.GetImage(CurrentChampionId, Main.ChampionInfo, "champion", 0, null, GlobalConfig.DataDragonPatch);
                IsBusy = false;
            }
        }

        private void LoLMonitor()
        {
            CheckLoL();

            while (true)
            {
                if (!IsLoLMonitoringPuased)
                {
                    if (Process.GetProcessesByName(ProccName).Length == 0)
                    {
                        ConnectionStatus = "Not Connected!";
                        IsLoLMonitoringPuased = true;
                        CheckLoL();
                    }
                }
                Thread.Sleep(1500);
            }
        }

        public async void CheckLoL()
        {
            WorkingStatus = "Connecting to League Client...";
            while (!await Main.leagueClient.PrepareAsync())
                Thread.Sleep(1000);

            string DisplayName = null;

            while (string.IsNullOrEmpty(DisplayName))
                DisplayName = (await Main.leagueClient?.GetCurrentSummonerAsync())?.displayName;

            ConnectionStatus = $"Logged as {DisplayName}";
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
