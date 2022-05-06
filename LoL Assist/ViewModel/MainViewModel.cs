using static LoL_Assist_WAPP.Model.LoLAWrapper;
using static LoL_Assist_WAPP.Utils;
using System.Collections.Generic;
using LoLA.WebAPIs.DataDragon;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
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
        private const string disconnected = "Disconnected";
        #region Miscellaneous
        public ICommand DebugCommand { get; }
        public ICommand ManualImportCommand { get; }

        private Visibility manualImportVisibility = Visibility.Collapsed;
        public Visibility ManualImportVisibility
        {
            get => manualImportVisibility;
            set
            {
                if (manualImportVisibility != value)
                {
                    manualImportVisibility = value;
                    OnPropertyChanged(nameof(ManualImportVisibility));
                }
            }
        }

        private SolidColorBrush connectionStatusForeground;

        public SolidColorBrush ConnectionStatusForeground
        {
            get => connectionStatusForeground;
            set
            {
                if (connectionStatusForeground != value)
                {
                    connectionStatusForeground = value;
                    OnPropertyChanged(nameof(ConnectionStatusForeground));
                }
            }
        }

        #endregion

        #region Champion data
        private string championImage;
        public string ChampionImage
        {
            get => championImage;
            set
            {
                if (championImage != value)
                {
                    championImage = value;
                    OnPropertyChanged(nameof(ChampionImage));
                }
            }
        }

        private string championName;
        public string ChampionName
        {
            get => championName;
            set
            {
                if (championName != value)
                {
                    championName = value;
                    OnPropertyChanged(nameof(ChampionName));
                }
            }
        }
        #endregion

        #region Status
        private string gMode;
        public string GMode
        {
            get => gMode;
            set
            {
                if (gMode != value)
                {
                    gMode = value;
                    OnPropertyChanged(nameof(GMode));
                }
            }
        }

        private string workingStatus;
        public string WorkingStatus
        {
            get => workingStatus;
            set
            {
                if (workingStatus != value)
                {
                    workingStatus = value;
                    OnPropertyChanged(nameof(WorkingStatus));
                }
            }
        }

        private string connectionStatus = disconnected;
        public string ConnectionStatus
        {
            get => connectionStatus;
            set
            {
                if (connectionStatus != value)
                {
                    connectionStatus = value;
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        private string importStatus;
        public string ImportStatus
        {
            get => importStatus;
            set
            {
                if (importStatus != value)
                {
                    importStatus = value;
                    OnPropertyChanged(nameof(ImportStatus));
                }
            }
        }

        private string warningStatus;
        public string WarningStatus
        {
            get => warningStatus;
            set
            {
                if (warningStatus != value)
                {
                    warningStatus = value;
                    OnPropertyChanged(nameof(WarningStatus));
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

        private Visibility warningGridVisibility = Visibility.Collapsed;
        public Visibility WarningGridVisibility
        {
            get => warningGridVisibility;
            set
            {
                if (warningGridVisibility != value)
                {
                    warningGridVisibility = value;
                    OnPropertyChanged(nameof(WarningGridVisibility));
                }
            }
        }


        private string warningCount;
        public string WarningCount
        {
            get => warningCount;
            set
            {
                if (warningCount != value)
                {
                    warningCount = value;
                    OnPropertyChanged(nameof(WarningCount));
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
        private string CurrentTime
        {
            get { return $"[{DateTime.Now:hh:mm:ss}]"; }
        }

        public MainViewModel()
        {
            GMode = $"Game Mode: {CurrentGameMode}";
            DebugCommand = new Command(async action => { await DebugExecute(); });
            ManualImportCommand = new Command(ManualImportExecute);
            InitLoLA();
        }

        private async void InitLoLA()
        {
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

            await LCUWrapper.GetRunePagesAsync();
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
                    ConnectionStatus = await GetSummonerInfo();
                    break;
                case Phase.InProgress:
                    ImportStatus = "Game in progress...";
                    ManualImportVisibility = Visibility.Collapsed;
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
                        ConnectionStatus = disconnected;
                        IsLoLMonitoringPuased = true;
                        CheckLoL();
                    }
                }
                Thread.Sleep(ConfigModel.config.MonitoringDelay * 2);
            }
        }

        public async void CheckLoL()
        {
            string summonerInfo;
            WorkingStatus = "Waiting for League to start...";
            do
            {
                summonerInfo = await GetSummonerInfo();
                Thread.Sleep(ConfigModel.config.MonitoringDelay * 2);
            } while (string.IsNullOrEmpty(summonerInfo));

            WorkingStatus = string.Empty;
            ConnectionStatus = summonerInfo;

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
                bool[] IsErr = { false, false };
                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    if (ConfigModel.config.AutoRunes || ConfigModel.config.AutoSpells)
                    {
                        List<Task> tasks = new List<Task>();

                        ChampionName = championName;
                        var CurrentChampionId = DataConverter.ChampionNameToId(ChampionName);

                        SetImportStatus("Fetching Runes & Spells...");

                        sw.Start();

                        if(!ConfigModel.config.CustomRunesSpells)
                            tasks.Add(CurrentChampionBuild = Main.RequestBuildsData(CurrentChampionId, CurrentGameMode, BuildsProvider.Metasrc));
                        else
                        {
                            var buildName = GetLocalBuildName(CurrentChampionId, CurrentGameMode);

                            if (buildName == ConfigModel.DefaultSource)
                                tasks.Add(CurrentChampionBuild = Main.RequestBuildsData(CurrentChampionId, CurrentGameMode, BuildsProvider.Metasrc));
                            else
                                tasks.Add(CurrentChampionBuild = Main.RequestBuildsData(CurrentChampionId, CurrentGameMode, 
                                BuildsProvider.Local, Path.GetFileNameWithoutExtension(buildName)));
                        }

                        tasks.Add(Task.Run(() => ChampionImage = DataDragonWrapper.GetChampionImage(CurrentChampionId, Global.Config.dDragonPatch).Result));
                        Parallel.ForEach(tasks, task => { Task.Run(() => task).Wait(); });

                        SetImportStatus($"Importing {FixedName(ChampionName)} Runes & Spells...");

                        RuneObj RB = (await CurrentChampionBuild)?.rune;
                        SpellObj SC = (await CurrentChampionBuild)?.spell;

                        if (CurrentChampionBuild != null)
                        {
                            var currentPerks = await LCUWrapper.GetCurrentRunePageAsync();
                            List<Task> ImportTasks = new List<Task>();
                            if (ConfigModel.config.AutoRunes)
                                ImportTasks.Add(SetRuneAsync(RB, currentPerks));

                            if (ConfigModel.config.AutoSpells)
                                ImportTasks.Add(ImportSpellsAsync(SC, CurrentGameMode));

                            int i = 0;
                            Parallel.ForEach(ImportTasks, async task => { 
                                try  {  await task; IsErr[i] = false; } 
                                catch { IsErr[i] = true; }  i++; 
                            });
                            currentPerks = null;
                        }
                    }
                }
                catch 
                {
                    WarningStatus = $"{CurrentTime} Failed to import Runes & Spells.";
                    Warnings.AppendLine($"{CurrentTime} Failed to import {FixedName(ChampionName)} Runes & Spells.");
                    SetImportStatus("Uh oh, LoL Assist failed to import Runes & Spells", LogType.EROR); 
                }
                finally
                {
                    sw.Stop();
                    if (IsErr[0])
                        Warnings.AppendLine($"{CurrentTime} Failed to import {FixedName(ChampionName)} Runes.");
                    else if (IsErr[1])
                        Warnings.AppendLine($"{CurrentTime} Failed to import {FixedName(ChampionName)} Spells.");
                    else if (IsErr[0] && IsErr[1])
                        SetImportStatus($"Uh Oh, something went wrong.\ntry deleting {FixedName(ChampionName)} data might fix the issue", LogType.EROR);
                    else
                        SetImportStatus($"Runes & Spells has been imported successfully! Elapsed [{sw.ElapsedMilliseconds}ms]");

                    WarningStatus = Warnings.ToString().TrimEnd(Environment.NewLine.ToCharArray());
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
        private async Task<string> GetSummonerInfo()
        {
            var summonerInfo = await LCUWrapper.GetCurrentSummonerAsync();
            if (summonerInfo == null)
                return null;
            else if (summonerInfo.displayName == null
            || summonerInfo.summonerLevel == 0)
                return null;

            return $"Name: {summonerInfo?.displayName} | Lvl {summonerInfo?.summonerLevel}";
        }

        private void SetImportStatus(string msg, LogType logType = LogType.INFO)
        {
            ImportStatus = msg;
            Log(msg, logType);
        }

        private void ClearChamp(bool hideManualImport = true)
        {
            ChampionImage = null;
            ChampionName = string.Empty;

            if (hideManualImport)
                ManualImportVisibility = Visibility.Hidden;
        }

        private void ResetStatus()
        {
            ClearChamp();
            ManualImportVisibility = Visibility.Collapsed;
            ImportStatus = "Waiting for a game queue...";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
