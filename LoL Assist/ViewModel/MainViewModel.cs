using LoLA;
using System.Threading;
using LoLA.LeagueClient;
using LoLA.Data.Objects;
using System.Diagnostics;
using System.ComponentModel;
using LoL_Assist_WAPP.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using static LoL_Assist_WAPP.Model.LoLAWrapper;

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
        private GameMode CurrentGameMode = GameMode.NONE;
        private Task<ChampionBuild> CurrentChampionBuild = null;

        public MainViewModel()
        {
            InitLoLA();
        }

        private async void InitLoLA()
        {
            WorkingStatus = "Loading in Configuration...";
            ConfigM.LoadConfig();

            WorkingStatus = "Downloading Prerequisite...";
            while (!await Main.init())
                Thread.Sleep(ConfigM.config.MonitoringDelay * 2);

            WorkingStatus = "Initializing LoLA Components...";
            GlobalConfig.DataDragonPatch = Main.dataWrapper.Versions[0];
            PhaseMonitor.MonitorDelay = ConfigM.config.MonitoringDelay;
            PhaseMonitor.IsMonitoring = true;
            PhaseMonitor.Init_GameFlowMonitor();

            ChampMonitor.InitMonitor();
            ChampMonitor.IsMonitoring = true;
            ChampMonitor.ChampionChanged += Champion_Changed;

            IsLoLMonitoringPuased = true;
            var lolMonitorThread = new Thread(LoLMonitor);
            lolMonitorThread.Start();

            PhaseMonitor.PhaseChanged += Phase_Changed;
            Phase_Changed(null, null); // Trigger init
        }

        private async void Phase_Changed(object sender, GameFlowMonitor.PhaseChangedArgs e)
        {
            ChampMonitor.CurrentPhase = e == null ? Phase.None : e.CurrentPhase;
            if(ChampMonitor.CurrentPhase != Phase.InProgress)
            {
                CurrentGameMode = await Main.leagueClient.GetCurrentGameModeAsync();
                gMode = $"Game Mode: {CurrentGameMode}";
            }

            switch (ChampMonitor.CurrentPhase)
            {
                case Phase.ChampSelect:
                    ImportStatus = "Selecting a Champion...";
                    break;
                case Phase.WaitingForStats:
                    ResetStatus();
                    var summonerInfo = await Main.leagueClient.GetCurrentSummonerAsync();
                    ConnectionStatus = $"{summonerInfo.displayName} | Lvl {summonerInfo.summonerLevel}";
                    break;
                case Phase.None:
                case Phase.Lobby:
                    ResetStatus();
                    break;
                case Phase.InProgress:
                    ImportStatus = "Game in Progress...";
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
            ImportStatus = "Waiting For a Game Queue...";
        }

        private async void Champion_Changed(object sender, ChampionMonitor.ChampionChangedArgs e)
        {
            if (!string.IsNullOrEmpty(e.Name))
            {
                ChampionImage = null;
                ChampionName = string.Empty;

                while (IsBusy)
                    Thread.Sleep(ConfigM.config.MonitoringDelay);

                ChampionName = e.Name;

                bool RB_IsSuccessFull = false, SC_IsSuccessFull = false;
                if (ConfigM.config.AutoRune || ConfigM.config.AutoSpells)
                {
                    IsBusy = true;

                    var CurrentChampionId = DataConverter.ChampionNameToId(ChampionName, Main.dataWrapper.Champions);

                    ImportStatus = "Fetching Builds...";

                    Stopwatch sw = Stopwatch.StartNew();
                    sw.Start();

                    var currentPerks = await Main.leagueClient.GetCurrentRunePageAsync();

                    Task<string> ImageChampion;

                    await Task.WhenAll(CurrentChampionBuild = Main.RequestDataAsync(CurrentChampionId, StatRequest.Metasrc, GlobalConfig.DataDragonPatch, CurrentGameMode),
                    ImageChampion = Main.dataWrapper?.GetImage(CurrentChampionId, Main.ChampionInfo, "champion", 0, null, GlobalConfig.DataDragonPatch));

                    ChampionImage = await ImageChampion;
                    ImportStatus = $"Importing {Utils.FixedName(ChampionName)} Builds...";

                    RuneObj RB = (await CurrentChampionBuild).RuneBuild;
                    SpellObj SC = (await CurrentChampionBuild).SpellCombo;

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
                        ImportStatus = $"Builds Has Been Imported Successfully! Elapsed [{sw.ElapsedMilliseconds}ms]";
                    else ImportStatus = $"Uh Oh, Something Went Wrong. Try Deleting {Utils.FixedName(ChampionName)} Data Might Fix The Issue.";

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
                        ConnectionStatus = "Not Connected!";
                        IsLoLMonitoringPuased = true;
                        CheckLoL();
                    }
                }
                Thread.Sleep(ConfigM.config.MonitoringDelay * 3);
            }
        }

        public async void CheckLoL()
        {
            WorkingStatus = "Connecting to League Client...";
            while (!await Main.leagueClient.PrepareAsync())
                Thread.Sleep(ConfigM.config.MonitoringDelay * 2);

            var summonerInfo = new LoLA.LeagueClient.Objects.Summoner();

            while (string.IsNullOrEmpty(summonerInfo?.displayName))
            {
                summonerInfo = await Main.leagueClient.GetCurrentSummonerAsync();
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
