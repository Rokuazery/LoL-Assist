using static LoL_Assist_WAPP.Model.LoLAWrapper;
using static LoL_Assist_WAPP.Utils;
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
using LoLA.Utils.Log;
using System.Windows;
using LoLA.Objects;
using System.Text;
using System.Linq;
using LoLA.Enums;
using System.IO;
using LoLA.LCU;
using System;
using LoLA;

namespace LoL_Assist_WAPP.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Miscellaneous
        public ICommand DebugCommand { get; set; }
        public ICommand ManualImportCommand { get; set; }

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
        #endregion

        #region Champion data
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
        #endregion

        #region Status
        private string _GMode;
        public string GMode
        {
            get { return _GMode; }
            set
            {
                if (_GMode != value)
                {
                    _GMode = value;
                    OnPropertyChanged("GMode");
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

        private string _WarningStatus;
        public string WarningStatus
        {
            get { return _WarningStatus; }
            set
            {
                if (_WarningStatus != value)
                {
                    _WarningStatus = value;
                    OnPropertyChanged("WarningStatus");
                    if (string.IsNullOrEmpty(value))
                        WarningGridVisibility = Visibility.Collapsed;
                    else
                    {
                        WarningGridVisibility = Visibility.Visible;
                        WarningCount = string.Format("({0})", value.Split('\n').ToArray().Length.ToString());
                    }
                }
            }
        }

        private Visibility _WarningGridVisibility = Visibility.Collapsed;
        public Visibility WarningGridVisibility
        {
            get { return _WarningGridVisibility; }
            set
            {
                if (_WarningGridVisibility != value)
                {
                    _WarningGridVisibility = value;
                    OnPropertyChanged("WarningGridVisibility");
                }
            }
        }


        private string _WarningCount;
        public string WarningCount
        {
            get { return _WarningCount; }
            set
            {
                if (_WarningCount != value)
                {
                    _WarningCount = value;
                    OnPropertyChanged("WarningCount");
                    if (string.IsNullOrEmpty(value))
                        WarningGridVisibility = Visibility.Collapsed;
                    else WarningGridVisibility = Visibility.Visible;
                }
            }
        }
        #endregion

        private bool IsBusy = false;
        private bool IsLoLMonitoringPuased = false;
        private const string ProccName = "LeagueClient";
        private GameMode CurrentGameMode = GameMode.NONE;
        private Task<ChampionBD> CurrentChampionBuild = null; 
        private StringBuilder Warnings = new StringBuilder();
        private string cTime
        {
            get { return $"[{DateTime.Now:hh:mm:ss}]"; }
        }

        public MainViewModel()
        {
            GMode = $"Game Mode: {CurrentGameMode}";
            DebugCommand = new RelayCommand(async action => { await DebugExecute(); }, o => true);
            ManualImportCommand = new RelayCommand(ManualImportExecute, o => true);
            InitLoLA();
        }

        private async void InitLoLA()
        {
            Global.Config.debug = true;
            LogService.Clear();
            WorkingStatus = "Loading in configuration...";
            ConfigModel.LoadConfig();

            if(ConfigModel.config.UpdateOnStartup)
            {
                WorkingStatus = "Checking for updates...";
                await Update.Start();
            }

            WorkingStatus = "Downloading prerequisite...";
            while (!await Main.Init())
                Thread.Sleep(ConfigModel.config.MonitoringDelay * 2);

            WorkingStatus = "Initializing LoLA components...";
            Global.Config.dDragonPatch = DataDragonWrapper.patchVersions[0];

            phaseMonitor.InitPhaseMonitor(); 
            phaseMonitor.MonitorDelay = ConfigModel.config.MonitoringDelay;

            champMonitor.InitChampionMonitor();
            champMonitor.ChampionChanged += Champion_Changed;
            champMonitor.MonitorDelay = ConfigModel.config.MonitoringDelay;

            IsLoLMonitoringPuased = true;
            var lolMonitorThread = new Thread(LoLMonitor);
            lolMonitorThread.Start();

            phaseMonitor.PhaseChanged += Phase_Changed;
            ResetStatus();
        }

        private async void Phase_Changed(object sender, PhaseMonitor.PhaseChangedArgs e)
        {
            CurrentGameMode = await LCUWrapper.GetCurrentGameModeAsync();

            if (e.currentPhase != Phase.InProgress)
                GMode = $"Game Mode: {CurrentGameMode}";

            switch (e.currentPhase)
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

            //var page = await LCUWrapper.GetCurrentRunePageAsync();
            //Console.WriteLine($"Name: {page.name}");
            //foreach (var pIds in page.selectedPerkIds)
            //    Console.WriteLine(string.Format("ID: {0}", pIds));
        }

        private void Champion_Changed(object sender, ChampionMonitor.ChampionChangedArgs e) => ImportBuilds(e.championName);
        private async void ManualImportExecute(object e) => ImportBuilds(await LCUWrapper.GetCurrentChampionAsyncV2());

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
                Thread.Sleep(ConfigModel.config.MonitoringDelay * 2);
            }
        }

        public async void CheckLoL()
        {
            Summoner summonerInfo;
            WorkingStatus = "Waiting for League to start...";
            do
            {
                summonerInfo = await LCUWrapper.GetCurrentSummonerAsync();
                Thread.Sleep(ConfigModel.config.MonitoringDelay * 2);
            } while (string.IsNullOrEmpty(summonerInfo?.displayName));

            WorkingStatus = string.Empty;
            ConnectionStatus = $"Name: {summonerInfo.displayName} | Level: {summonerInfo.summonerLevel}";

            IsLoLMonitoringPuased = false;
        }

        private async void ImportBuilds(string championName)
        {
            if (!string.IsNullOrEmpty(championName))
            {
                ClearChamp(false);

                while (IsBusy)
                    Thread.Sleep(ConfigModel.config.MonitoringDelay);

                IsBusy = true;
                try
                {
                    bool RB_IsSuccessFull = false, SC_IsSuccessFull = false;
                    if (ConfigModel.config.AutoRunes || ConfigModel.config.AutoSpells)
                    {
                        List<Task> tasks = new List<Task>();

                        ChampionName = championName;
                        var CurrentChampionId = DataConverter.ChampionNameToId(ChampionName);

                        ImportStatus = $"Fetching Runes & Spells...";

                        Stopwatch sw = Stopwatch.StartNew();
                        sw.Start();

                        if(!ConfigModel.config.CustomRunesSpells)
                            tasks.Add(CurrentChampionBuild = Main.RequestBuildsData(CurrentChampionId, CurrentGameMode, BuildsProvider.Metasrc));
                        else
                        {
                            var defaultBuildConfig = new DefaultBuildConfig();
                            if (File.Exists(defConfigPath(CurrentChampionId))) 
                                defaultBuildConfig = getDefaultBuildConfig(CurrentChampionId);

                            var buildName = defaultBuildConfig.getDefaultConfig(CurrentGameMode);

                            var customBuildPath = Main.localBuild.BuildsFolder(CurrentChampionId, CurrentGameMode);
                            var fullPath = $"{customBuildPath}\\{defaultBuildConfig.getDefaultConfig(CurrentGameMode)}";

                            if (!File.Exists(fullPath))
                                defaultBuildConfig.resetDefaultConfig(CurrentGameMode);

                            if (buildName == ConfigModel.DefaultSource)
                                tasks.Add(CurrentChampionBuild = Main.RequestBuildsData(CurrentChampionId, CurrentGameMode, BuildsProvider.Metasrc));
                            else
                                tasks.Add(CurrentChampionBuild = Main.RequestBuildsData(CurrentChampionId, CurrentGameMode, 
                                BuildsProvider.Local, defaultBuildConfig.getDefaultConfig(CurrentGameMode)));
                        }

                        tasks.Add(Task.Run(() => ChampionImage = DataDragonWrapper.GetChampionImage(CurrentChampionId, Global.Config.dDragonPatch).Result));
                        Parallel.ForEach(tasks, task => { /*Console.WriteLine("PALA");*/ Task.Run(() => task).Wait(); });

                        SetImportStatus($"Importing {Utils.FixedName(ChampionName)} Runes & Spells...", LogType.INFO);

                        RuneObj RB = (await CurrentChampionBuild)?.rune;
                        SpellObj SC = (await CurrentChampionBuild)?.spell;

                        if (CurrentChampionBuild != null)
                        {
                            var currentPerks = await LCUWrapper.GetCurrentRunePageAsync();
                            List<Task> ImportTasks = new List<Task>();
                            if (ConfigModel.config.AutoRunes && RB != null)
                            {
                                ImportTasks.Add(SetRuneAsync(RB, currentPerks));
                                RB_IsSuccessFull = true;
                            }

                            if (ConfigModel.config.AutoSpells && SC != null)
                            {
                                ImportTasks.Add(ImportSpellsAsync(SC, CurrentGameMode));
                                SC_IsSuccessFull = true;
                            }

                            Parallel.ForEach(ImportTasks, async task => { await task; });
                            currentPerks = null;
                        }

                        //RB_IsSuccessFull = false;
                        //SC_IsSuccessFull = false;

                        if (!RB_IsSuccessFull)
                            Warnings.AppendLine($"{cTime} Failed to import {Utils.FixedName(ChampionName)} Runes.");
                        if (!SC_IsSuccessFull)
                            Warnings.AppendLine($"{cTime} Failed to import {Utils.FixedName(ChampionName)} Spells.");

                        sw.Stop();

                        if (!RB_IsSuccessFull && !SC_IsSuccessFull)
                            SetImportStatus($"Uh Oh, something went wrong.\ntry deleting {Utils.FixedName(ChampionName)} data might fix the issue", LogType.EROR);
                        else
                            SetImportStatus($"Runes & Spells has been imported successfully! Elapsed [{sw.ElapsedMilliseconds}ms]");

                        WarningStatus = Warnings.ToString().TrimEnd(Environment.NewLine.ToCharArray());
                    }
                }
                catch 
                {
                    WarningStatus = $"{cTime} Failed to import Runes & Spells.";
                    Warnings.AppendLine($"{cTime} Failed to import {Utils.FixedName(ChampionName)} Runes & Spells.");
                    SetImportStatus("Uh oh, LoL Assist failed to import Runes & Spells", LogType.EROR); 
                }
                IsBusy = false;
            }
        }


        public async Task DebugExecute()
        {
            LogService.Log(LogService.Model("clicked", "Debug", LogType.DBUG));
            await Main.RequestBuildsData("Aatrox", GameMode.URF, BuildsProvider.Metasrc);
        }

        #region Space junk
        private void SetImportStatus(string msg, LogType logType = LogType.INFO)
        {
            ImportStatus = msg;
            Utils.Log(msg, logType);
        }

        private void ClearChamp(bool hideManualImport = true)
        {
            ChampionImage = null;
            ChampionName = string.Empty;
            //WarningStatus = string.Empty;

            if (hideManualImport)
                ManualImportVisibility = Visibility.Hidden;
        }

        private void ResetStatus()
        {
            ClearChamp();
            ManualImportVisibility = Visibility.Hidden;
            ImportStatus = "Waiting for a game queue...";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
