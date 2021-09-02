using LoLA;
using System.Threading;
using LoLA.LeagueClient;
using LoLA.Data.Objects;
using System.Diagnostics;
using System.ComponentModel;
using LoL_Assist_WAPP.Model;
using static LoL_Assist_WAPP.Model.LoLAWrapper;
using System.Threading.Tasks;
using System.Collections.Generic;

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
                    // Update The current champion for ChampionMonitor
                    ImportStatus = "Selecting a Champion...";
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

        Task<ChampionBuild> CurrentChampionBuild = null;
        private async void Champion_Changed(object sender, ChampionMonitor.ChampionChangedArgs e)
        {
            if (!string.IsNullOrEmpty(e.Name))
            {
                ChampionImage = null;
                ChampionName = string.Empty;

                while (IsBusy)
                    Thread.Sleep(ConfigM.config.MonitoringDelay);

                ChampionName = e.Name;

                bool RB_IsSuccessFull = false;
                bool SC_IsSuccessFull = false;

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

                        Parallel.ForEach(ImportTasks, async task => {
                            await task;
                        });
                    }

                    sw.Stop();

                    if (RB_IsSuccessFull && SC_IsSuccessFull)
                        ImportStatus = $"Builds Has Been Imported Successfully! Elapsed [{sw.ElapsedMilliseconds}ms]";
                    else ImportStatus = $"Uh Oh, Something Went Wrong. Try Deleting {Utils.FixedName(ChampionName)} Data Might Fix The Issue.";

                    IsBusy = false;
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
                Thread.Sleep(1500);
            }
        }

        public async void CheckLoL()
        {
            WorkingStatus = "Waiting for League Client...";
            while (!await Main.leagueClient.PrepareAsync())
                Thread.Sleep(1000);

            string DisplayName = null;

            while (string.IsNullOrEmpty(DisplayName))
                DisplayName = (await Main.leagueClient?.GetCurrentSummonerAsync())?.displayName;

            var Summoner = await Main.leagueClient?.GetCurrentSummonerAsync();
            ConnectionStatus = $"Logged as {DisplayName} | Lvl {Summoner.summonerLevel}";
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
