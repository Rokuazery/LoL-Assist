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
        private const string _disconnected = "Disconnected";
        #region Miscellaneous

        #region Commands
        public ICommand DebugCommand { get; }
        public ICommand ManualImportCommand { get; }

        #region Window Commands
        public ICommand MoveWindowCommand { get; }
        public ICommand ShutdownAppCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        public ICommand ShowBuildEditorWindowCommand { get; }

        private bool topmost;
        public bool Topmost
        {
            get => topmost;
            set
            {
                if (topmost != value)
                {
                    topmost = value;
                    OnPropertyChanged(nameof(Topmost));
                    Application.Current.Dispatcher.Invoke(() => { 
                        Application.Current.MainWindow.Activate(); // Fix for topmost issue
                    });
                }
            }
        }
        #endregion
        #endregion

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

        private object topMostContent = null;
        public object TopMostContent
        {
            get => topMostContent;
            set
            {
                Console.WriteLine("Content set: " + value);
                topMostContent = value;
                OnPropertyChanged(nameof(TopMostContent));
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

        private List<ItemImageModel> roles;
        public List<ItemImageModel> Roles
        {
            get => roles;
            set
            {
                if (roles != value)
                {
                    roles = value;
                    OnPropertyChanged(nameof(Roles));
                }
            }
        }

        private ItemImageModel selectedRole;
        public ItemImageModel SelectedRole
        {
            get => selectedRole;
            set
            {
                if (selectedRole != value)
                {
                    ReImport();
                    selectedRole = value;
                    OnPropertyChanged(nameof(SelectedRole));
                }
            }
        }

        private Visibility roleSelectVisibility = Visibility.Collapsed;
        public Visibility RoleSelectVisibility
        {
            get => roleSelectVisibility;
            set
            {
                if (roleSelectVisibility != value)
                {
                    roleSelectVisibility = value;
                    OnPropertyChanged(nameof(RoleSelectVisibility));
                }
            }
        }
        #endregion

        #region Status
        private GameMode gMode;
        public GameMode GMode
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

        private Visibility workingStatusVisibility;
        public Visibility WorkingStatusVisibility
        {
            get => workingStatusVisibility;
            set
            {
                if (workingStatusVisibility != value)
                {
                    workingStatusVisibility = value;
                    OnPropertyChanged(nameof(WorkingStatusVisibility));
                }
            }
        }

        private string connectionStatus = _disconnected;
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

        private Visibility championContainerVisibility = Visibility.Collapsed;
        public Visibility ChampionContainerVisibility
        {
            get => championContainerVisibility;
            set
            {
                if (championContainerVisibility != value)
                {
                    championContainerVisibility = value;
                    OnPropertyChanged(nameof(ChampionContainerVisibility));
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

        private bool _IsBusy = false;
        private bool _IsLoLMonitoringPuased = false;
        private const string _ProccName = "LeagueClient";
        private GameMode _CurrentGameMode = GameMode.NONE;
        private Task<ChampionBD> _CurrentChampionBuild = null; 
        private StringBuilder _Warnings = new StringBuilder();
        private string GetCurrentTime
        {
            get { return $"[{DateTime.Now:hh:mm:ss}]"; }
        }

        public MainViewModel()
        {
            InitTrayIcon();
            GMode = _CurrentGameMode;
            ManualImportCommand = new Command(o => { ReImport(); });
            DebugCommand = new Command(async o => { await DebugExecute(); });

            ShutdownAppCommand = new Command(o => { ShutdownAppExecute(); });
            ShowBuildEditorWindowCommand = new Command(o => { ShowBuildEditorWindow(); });
            MoveWindowCommand = new Command(o => { Application.Current.MainWindow.DragMove(); });
            MinimizeWindowCommand = new Command(o => { Application.Current.MainWindow.WindowState = WindowState.Minimized; });

            WorkingStatus = $"Clearing old logs...";
            LogService.Clear(); // remove old log file(LoLA.log)

            WorkingStatus = "Loading in configuration...";
            ConfigModel.LoadConfig();
            
            ShowPatchNotes(ConfigModel.config.DoNotShowPatch);

            Roles = new List<ItemImageModel>() {   // Roles XD
                new ItemImageModel(){Text = "Default", Image="pack://application:,,,/Resources/Fill.png"},
                new ItemImageModel(){Text = "Top", Image="pack://application:,,,/Resources/Top.png"},
                new ItemImageModel(){Text = "Jungle", Image="pack://application:,,,/Resources/Jungle.png"},
                new ItemImageModel(){Text = "Mid", Image="pack://application:,,,/Resources/Mid.png"},
                new ItemImageModel(){Text = "ADC", Image="pack://application:,,,/Resources/ADC.png"},
                new ItemImageModel(){Text = "Support", Image="pack://application:,,,/Resources/Support.png"}};

            InitLoLA();
        }

        private void ShowBuildEditorWindow()
        {
            var window = new BuildEditorWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private async void InitLoLA()
        {
            if (ConfigModel.config.UpdateOnStartup)
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

            _IsLoLMonitoringPuased = true;
            var lolMonitorThread = new Thread(LoLMonitor);
            lolMonitorThread.Start();

            phaseMonitor.PhaseChanged += Phase_Changed;
            ResetStatus();

            // await LCUWrapper.GetRunePagesAsync();
        }
        private async void ImportBuilds(string championName)
        {
            if (!string.IsNullOrEmpty(championName))
            {
                ClearChamp(false);

                while (_IsBusy)
                    Thread.Sleep(ConfigModel.config.MonitoringDelay);

                _IsBusy = true;
                bool[] IsErr = { false, false };
                Stopwatch sw = Stopwatch.StartNew();
                ChampionName = championName;
                try
                {
                    if (ConfigModel.config.AutoRunes || ConfigModel.config.AutoSpells)
                    {
                        List<Task> tasks = new List<Task>();
                        string CurrentRole = string.Empty;

                        var gm = _CurrentGameMode;
                        var cBD = _CurrentChampionBuild;

                        if (gm.Equals(GameMode.CLASSIC) || gm.Equals(GameMode.PRACTICETOOL))
                            CurrentRole = SelectedRole.Text.Replace("Default", string.Empty).ToLower();

                        var CurrentChampionId = DataConverter.ChampionNameToId(championName);
                        SetImportStatus("Fetching Runes & Spells...");
                        sw.Start();

                        if (!ConfigModel.config.CustomRunesSpells)
                            tasks.Add(cBD = Main.RequestBuildsData(CurrentChampionId, gm, BuildsProvider.Metasrc, CurrentRole));
                        else
                        {
                            var buildName = GetLocalBuildName(CurrentChampionId, gm);

                            if (buildName == ConfigModel.DefaultSource)
                                tasks.Add(cBD = Main.RequestBuildsData(CurrentChampionId, gm, BuildsProvider.Metasrc, CurrentRole));
                            else
                                tasks.Add(cBD = Main.RequestBuildsData(CurrentChampionId, gm,
                                BuildsProvider.Local, Path.GetFileNameWithoutExtension(buildName)));
                        }

                        tasks.Add(Task.Run(() => ChampionImage = DataDragonWrapper.GetChampionImage(CurrentChampionId, Global.Config.dDragonPatch).Result));
                        Parallel.ForEach(tasks, task => { Task.Run(() => task).Wait(); });

                        SetImportStatus($"Importing {FixedName(ChampionName)} Runes & Spells...");

                        RuneObj RB = (await cBD)?.rune;
                        SpellObj SC = (await cBD)?.spell;

                        if (cBD != null)
                        {
                            var currentPerks = await LCUWrapper.GetCurrentRunePageAsync();
                            List<Task> ImportTasks = new List<Task>();
                            if (ConfigModel.config.AutoRunes)
                                ImportTasks.Add(SetRuneAsync(RB, currentPerks));

                            if (ConfigModel.config.AutoSpells)
                                ImportTasks.Add(ImportSpellsAsync(SC, gm));

                            int i = 0;
                            Parallel.ForEach(ImportTasks, async task => {
                                try { await task; IsErr[i] = false; }
                                catch { IsErr[i] = true; }
                                i++;
                            });
                            currentPerks = null;
                        }
                    }
                }
                catch
                {
                    WarningStatus = $"{GetCurrentTime} Failed to import Runes & Spells.";
                    _Warnings.AppendLine($"{GetCurrentTime} Failed to import {FixedName(ChampionName)} Runes & Spells.");
                    SetImportStatus("Uh oh, LoL Assist failed to import Runes & Spells", LogType.EROR);
                }
                finally
                {
                    sw.Stop();
                    if (IsErr[0])
                        _Warnings.AppendLine($"{GetCurrentTime} Failed to import {FixedName(ChampionName)} Runes.");
                    else if (IsErr[1])
                        _Warnings.AppendLine($"{GetCurrentTime} Failed to import {FixedName(ChampionName)} Spells.");
                    else if (IsErr[0] && IsErr[1])
                        SetImportStatus($"Uh Oh, something went wrong.\ntry deleting {FixedName(ChampionName)} data might fix the issue", LogType.EROR);
                    else
                        SetImportStatus($"Runes & Spells has been imported successfully! Elapsed [{sw.ElapsedMilliseconds}ms]");

                    WarningStatus = _Warnings.ToString().TrimEnd(Environment.NewLine.ToCharArray());
                }
                _IsBusy = false;
            }
        }
        private async void Phase_Changed(object sender, PhaseMonitor.PhaseChangedArgs e)
        {
            Topmost = e.currentPhase == Phase.ChampSelect || e.currentPhase == Phase.ReadyCheck;
            _CurrentGameMode = await LCUWrapper.GetCurrentGameModeAsync();

            if (e.currentPhase != Phase.InProgress)
            {
                GMode = _CurrentGameMode;
                DispatcherInvoke(() => { // Needs to be invoke to fix delay issue
                    if (_CurrentGameMode == GameMode.CLASSIC
                    || _CurrentGameMode == GameMode.PRACTICETOOL)
                    {
                        SelectedRole = Roles[0];
                        RoleSelectVisibility = Visibility.Visible;
                    }
                    else RoleSelectVisibility = Visibility.Collapsed;
                });
            }

            switch (e.currentPhase)
            {
                case Phase.ChampSelect:
                    DispatcherInvoke(() => { 
                        ImportStatus = "Selecting a champion...";
                        ManualImportVisibility = Visibility.Visible;
                    });
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
                    DispatcherInvoke(() => { 
                        ImportStatus = "Game in progress..."; 
                        ManualImportVisibility = Visibility.Collapsed; 
                    });
                    break;
                case Phase.Matchmaking:
                    DispatcherInvoke(() => {
                        ClearChamp();
                        ImportStatus = "Queueing...";
                    });
                    break;
                case Phase.ReadyCheck:
                    SetTopMostContent(new MatchFoundViewModel());
                    break;
            }
            //var page = await LCUWrapper.GetCurrentRunePageAsync();
            //Console.WriteLine($"Name: {page.name}");
            //foreach (var pIds in page.selectedPerkIds)
            //    Console.WriteLine(string.Format("ID: {0}", pIds));
        }

        private void DispatcherInvoke(Action action) => Application.Current.Dispatcher.Invoke(action);

        #region LoL Checker
        // Checks for LeagueClient process and get summoner info
        public void LoLMonitor()
        {
            CheckLoL();

            while (true)
            {
                if (!_IsLoLMonitoringPuased)
                {
                    if (Process.GetProcessesByName(_ProccName).Length == 0)
                    {
                        DispatcherInvoke(() => {
                            WorkingStatusVisibility = Visibility.Visible;
                            ChampionContainerVisibility = Visibility.Collapsed;
                        });

                        ConnectionStatus = _disconnected;
                        _IsLoLMonitoringPuased = true;
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

            DispatcherInvoke(() => {
                WorkingStatusVisibility = Visibility.Collapsed;
                ChampionContainerVisibility = Visibility.Visible;
            });

            WorkingStatus = string.Empty;
            ConnectionStatus = summonerInfo;
            _IsLoLMonitoringPuased = false;
        }
        #endregion

        private void ShutdownAppExecute() => ShowMsgBox(new Action(() => { Application.Current.MainWindow.Close(); }),
        msg: "Are you sure you want to exit LoL Assist?", width: 230, height: 150);
        private void ShowPatchNotes(bool doNotShow) { if (!doNotShow) SetTopMostContent(new PatchViewModel()); }
        private void Champion_Changed(object sender, ChampionMonitor.ChampionChangedArgs e) => ImportBuilds(e.championName);
        private async void ReImport() => await Task.Run(async () => { ImportBuilds(await LCUWrapper.GetCurrentChampionAsyncV2()); });

        #region Tray Icon
        private void InitTrayIcon()
        {
            string[] menus = new string[] { "Exit", "Show", "Patch Notes", "Minimize to Tray" };
            var trayIcon = new System.Windows.Forms.NotifyIcon();
            var menuList = new List<System.Windows.Forms.MenuItem>();
            var menuContainer = new System.Windows.Forms.ContextMenu();

            for (int i = 0; i < menus.Length; i++)
            {
                var item = new System.Windows.Forms.MenuItem() {
                    Index = i,
                    Text = menus[i]
                };

                item.Click += new EventHandler(menuItem_Click);
                menuList.Add(item);
            }

            menuContainer.MenuItems.AddRange(menuList.ToArray());

            using(var stream = Application.GetResourceStream(new Uri("pack://application:,,,/icon.ico")).Stream)
            {
                trayIcon.Icon = new System.Drawing.Icon(stream);
            }

            trayIcon.Visible = true;
            trayIcon.ContextMenu = menuContainer;
            trayIcon.DoubleClick += (o, args) => ShowMainWindow();
            trayIcon.Text = $"LoL Assist v{ConfigModel.version}";

            // Dispose tray icon on closed
            Application.Current.MainWindow.Closed += (o, args) => {
                trayIcon.Icon = null;
                trayIcon.Dispose();
                Environment.Exit(0);
            };
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as System.Windows.Forms.MenuItem;
            switch(menuItem.Text)
            {
                case "Exit":
                    Application.Current.MainWindow.Close();
                    break;
                case "Show":
                    ShowMainWindow();
                    break;
                case "Patch Notes":
                    ShowMainWindow();
                    ShowPatchNotes(false);
                    break;
                case "Minimize to Tray":
                    Application.Current.MainWindow.ShowInTaskbar = false;
                    Application.Current.MainWindow.WindowState = WindowState.Minimized;
                    break;
            }
        }

        private void ShowMainWindow()
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.ShowInTaskbar = true;
            Application.Current.MainWindow.WindowState = WindowState.Normal;

            Application.Current.MainWindow.Activate();
        }
        #endregion

        #region Debug D:
        private int _debugI = 0;
        public async Task DebugExecute()
        {
            LogService.Log(LogService.Model($"--------------attempt-{_debugI}", "Debug", LogType.DBUG));
            //* Debug Code *//
            //await Main.RequestBuildsData("Aatrox", GameMode.URF, BuildsProvider.Metasrc);
            Console.WriteLine(await LCUWrapper.GetCurrentSessionAsync());
            _debugI++;
        }
        #endregion

        #region Space junk
        private async void SetTopMostContent(object obj)
        {
            TopMostContent = null;
            await Task.Delay(1); // Fix
            TopMostContent = obj;
        }
        private void ShowMsgBox(Action action, string msg, double width = 330, double height = 220)
        {
            SetTopMostContent(new MessageBoxViewModel()
            {
                message = msg,
                width = width,
                height = height,
                action = () => { action.Invoke(); }
            });
        }
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
            DispatcherInvoke(() => { ImportStatus = msg; });
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
