using LoLA.Networking.WebWrapper.DataDragon.Data;
using static LoL_Assist_WAPP.Model.LoLAWrapper;
using LoLA.Networking.WebWrapper.DataDragon;
using static LoL_Assist_WAPP.Utils.Helper;
using System.Collections.Generic;
using LoLA.Networking.LCU.Events;
using LoLA.Networking.LCU.Enums;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using LoL_Assist_WAPP.Utils;
using System.Windows.Input;
using LoLA.Networking.LCU;
using System.Diagnostics;
using LoLA.Utils.Logger;
using System.Threading;
using LoLA.Data.Enums;
using System.Windows;
using System.Text;
using System.Linq;
using LoLA.Data;
using System;
using LoLA;

namespace LoL_Assist_WAPP.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Miscellaneous

        #region Commands
        public ICommand DebugCommand { get; }
        public ICommand ManualImportCommand { get; }
        public ICommand ShowEditRunesPanelCommand { get; }
        public ICommand ShowDownloadPanelCommand { get; }

        #region Window Commands
        public ICommand MoveWindowCommand { get; }
        public ICommand ShutdownAppCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        public ICommand MinimizeToTrayCommand { get; }

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

        private Visibility _manualImportVisibility = Visibility.Collapsed;
        public Visibility ManualImportVisibility
        {
            get => _manualImportVisibility;
            set
            {
                if (_manualImportVisibility != value)
                {
                    _manualImportVisibility = value;
                    OnPropertyChanged(nameof(ManualImportVisibility));
                }
            }
        }

        private object _topMostContent = null;
        public object TopMostContent
        {
            get => _topMostContent;
            set
            {
                //Console.WriteLine("Content set: " + value);
                _topMostContent = value;
                OnPropertyChanged(nameof(TopMostContent));
            }
        }

        #endregion

        #region Champion data
        private string _championImage;
        public string ChampionImage
        {
            get => _championImage;
            set
            {
                if (_championImage != value)
                {
                    _championImage = value;
                    OnPropertyChanged(nameof(ChampionImage));
                }
            }
        }

        private string _championName;
        public string ChampionName
        {
            get => _championName;
            set
            {
                if (_championName != value)
                {
                    _championName = value;
                    OnPropertyChanged(nameof(ChampionName));
                }
            }
        }

        private List<ItemImageModel> _providers;
        public List<ItemImageModel> Providers
        {
            get => _providers;
            set
            {
                if (_providers != value)
                {
                    _providers = value;
                    OnPropertyChanged(nameof(Providers));
                }
            }
        }

        private ItemImageModel _selectedProvider;
        public ItemImageModel SelectedProvider
        {
            get => _selectedProvider;
            set
            {
                if (_selectedProvider != value)
                {
                    if(!string.IsNullOrEmpty(value.Text))
                    {
                        ConfigModel.s_CurrentProvider = Converters.ProviderConverter.ToEnum(value.Text);
                        reImport();
                    }
            
                    _selectedProvider = value;
                    OnPropertyChanged(nameof(SelectedProvider));
                }
            }
        }

        private List<ItemImageModel> _roles;
        public List<ItemImageModel> Roles
        {
            get => _roles;
            set
            {
                if (_roles != value)
                {
                    _roles = value;
                    OnPropertyChanged(nameof(Roles));
                }
            }
        }

        private ItemImageModel _selectedRole;
        public ItemImageModel SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (_selectedRole != value)
                {
                    reImport();
                    _selectedRole = value;
                    OnPropertyChanged(nameof(SelectedRole));
                }
            }
        }

        private Visibility _roleSelectVisibility = Visibility.Collapsed;
        public Visibility RoleSelectVisibility
        {
            get => _roleSelectVisibility;
            set
            {
                if (_roleSelectVisibility != value)
                {
                    _roleSelectVisibility = value;
                    OnPropertyChanged(nameof(RoleSelectVisibility));
                }
            }
        }
        #endregion

        #region Status
        private GameMode _gMode;
        public GameMode GMode
        {
            get => _gMode;
            set
            {
                if (_gMode != value)
                {
                    _gMode = value;
                    OnPropertyChanged(nameof(GMode));
                }
            }
        }

        private string _workingStatus;
        public string WorkingStatus
        {
            get => _workingStatus;
            set
            {
                if (_workingStatus != value)
                {
                    _workingStatus = value;
                    OnPropertyChanged(nameof(WorkingStatus));
                }
            }
        }

        private Visibility _workingStatusVisibility;
        public Visibility WorkingStatusVisibility
        {
            get => _workingStatusVisibility;
            set
            {
                if (_workingStatusVisibility != value)
                {
                    _workingStatusVisibility = value;
                    OnPropertyChanged(nameof(WorkingStatusVisibility));
                }
            }
        }

        private string _connectionStatus = DISCONNECTED;
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus != value)
                {
                    _connectionStatus = value;
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        private string _importStatus;
        public string ImportStatus
        {
            get => _importStatus;
            set
            {
                if (_importStatus != value)
                {
                    _importStatus = value;
                    OnPropertyChanged(nameof(ImportStatus));
                }
            }
        }

        private string _warningStatus;
        public string WarningStatus
        {
            get => _warningStatus;
            set
            {
                if (_warningStatus != value)
                {
                    _warningStatus = value;
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

        private Visibility _warningGridVisibility = Visibility.Collapsed;
        public Visibility WarningGridVisibility
        {
            get => _warningGridVisibility;
            set
            {
                if (_warningGridVisibility != value)
                {
                    _warningGridVisibility = value;
                    OnPropertyChanged(nameof(WarningGridVisibility));
                }
            }
        }

        private Visibility _championContainerVisibility = Visibility.Collapsed;
        public Visibility ChampionContainerVisibility
        {
            get => _championContainerVisibility;
            set
            {
                if (_championContainerVisibility != value)
                {
                    _championContainerVisibility = value;
                    OnPropertyChanged(nameof(ChampionContainerVisibility));
                }
            }
        }

        private string _warningCount;
        public string WarningCount
        {
            get => _warningCount;
            set
            {
                if (_warningCount != value)
                {
                    _warningCount = value;
                    OnPropertyChanged(nameof(WarningCount));
                    if (string.IsNullOrEmpty(value))
                        WarningGridVisibility = Visibility.Collapsed;
                    else WarningGridVisibility = Visibility.Visible;
                }
            }
        }
        #endregion

        private bool isBusy = false;
        private GameMode currentGameMode = GameMode.NONE;
        private readonly Task<ChampionBuild> r_currentChampionBuild = null;
        private readonly StringBuilder warningStringBuilder = new StringBuilder();

        private const string PROCESS_NAME = "LeagueClient";
        private const string CONNECTING = "Connecting";
        private const string DISCONNECTED = "Disconnected"; 
        private const string ICON_PATH = "pack://application:,,,/icon.ico";
        private string getCurrentTime { get => $"[{DateTime.Now:hh:mm:ss}]"; }

        public MainViewModel()
        {
            initTrayIcon();

            GMode = currentGameMode;
            ManualImportCommand = new Command(o => { reImport(); });
            ShutdownAppCommand = new Command(o => { shutdownAppExecute(); });
            ShowEditRunesPanelCommand = new Command(o => { showRuneEditorPanel(); });
            ShowDownloadPanelCommand = new Command(o => { setTopMostContent(new DownloadViewModel()); });
            MoveWindowCommand = new Command(o => { Application.Current.MainWindow.DragMove(); });

            MinimizeToTrayCommand = new Command(o => { minimizeToTray(); });
            MinimizeWindowCommand = new Command(o => { Application.Current.MainWindow.WindowState = WindowState.Minimized; });

            WorkingStatus = $"Clearing old logs...";
            LogService.Clear();

            WorkingStatus = "Loading in configuration...";
            ConfigModel.LoadConfig();

            showPatchNotes(ConfigModel.s_Config.DoNotShowPatch);

           var roles = Enum.GetValues(typeof(Role)).Cast<Role>();

            Roles = new List<ItemImageModel>();

            Providers = new List<ItemImageModel>() {
              ItemImage("METAsrc.com", ImageSrc("metasrc")),
              ItemImage("U.GG", ImageSrc("ugg"))
            };

            SelectedProvider = Providers[0];

            foreach (var role in roles)
            {
                var image = role == Role.RECOMENDED ? 
                $"{ConfigModel.RESOURCE_PATH}Fill.png" :
                $"{ConfigModel.RESOURCE_PATH}{role.ToString().Replace("BOTTOM", "adc")}.png";

                Roles.Add(ItemImage(role.ToString(), image));
            }
            initLoLA();
        }

        private async void initLoLA()
        {
            if (ConfigModel.s_Config.UpdateOnStartup)
            {
                WorkingStatus = "Checking for updates...";
                await Updater.Start();
            }

            WorkingStatus = "Downloading prerequisite...";
            while (!await Main.Init())
                Thread.Sleep(ConfigModel.s_Config.MonitoringDelay * 2);

            WorkingStatus = "Initializing LoLA components...";

            champMonitor.ChampionChanged += championChanged;
            champMonitor.MonitorDelay = ConfigModel.s_Config.MonitoringDelay;

            var lolMonitorThread = new Thread(LoLMonitor);
            lolMonitorThread.Start();

            phaseMonitor.PhaseChanged += phaseChanged;
            phaseMonitor.MonitorDelay = ConfigModel.s_Config.MonitoringDelay;
            resetStatus();
        }

        private async void importBuilds(string championName, bool forceRuneUpdate = false)
        {
            if (!string.IsNullOrEmpty(championName))
            {
                clearChamp(false);

                while (isBusy)
                    Thread.Sleep(ConfigModel.s_Config.MonitoringDelay);

                isBusy = true;
                bool[] isError = { false, false };
                Stopwatch sw = Stopwatch.StartNew();
                ChampionName = championName;
                try
                {
                    if (ConfigModel.s_Config.AutoRunes || ConfigModel.s_Config.AutoSpells)
                    {
                        List<Task> tasks = new List<Task>();
                        var currentRole = Role.RECOMENDED;

                        var gameMode = currentGameMode;
                        var championBuild = r_currentChampionBuild;

                        if (gameMode.Equals(GameMode.CLASSIC) || gameMode.Equals(GameMode.PRACTICETOOL))
                            currentRole = (Role)(!string.IsNullOrEmpty(SelectedRole.Text) ? Enum.Parse(typeof(Role), SelectedRole.Text) : currentRole);

                        var currentChampionId = Converter.ChampionNameToId(championName);

                        var defaultConfigName = LocalBuild.GetLocalBuildName(currentChampionId, gameMode);

                        setImportStatus("Fetching Runes & Spells...");
                        sw.Start();

                        var provider = !string.IsNullOrEmpty(defaultConfigName) 
                        && defaultConfigName != ConfigModel.s_CurrentProvider.ToString()
                        ? Provider.Local : ConfigModel.s_CurrentProvider;

                        tasks.Add(championBuild = Main.RequestBuildsData(currentChampionId, gameMode, provider, currentRole));
                        tasks.Add(Task.Run(() => ChampionImage = DataDragonWrapper.GetChampionImage(currentChampionId).Result));
                        Parallel.ForEach(tasks, task => { Task.Run(() => task).Wait(); });

                        setImportStatus($"Importing {FixedName(ChampionName)} Runes & Spells...");

                        Rune runeBuild = (await championBuild)?.Rune;
                        Spell spellCombo = (await championBuild)?.Spell;

                        if (championBuild != null)
                        {
                            var currentPerks = await LCUWrapper.GetCurrentRunePageAsync();
                            List<Task> ImportTasks = new List<Task>();
                            if (ConfigModel.s_Config.AutoRunes)
                                ImportTasks.Add(SetRuneAsync(runeBuild, currentPerks, forceRuneUpdate));

                            if (ConfigModel.s_Config.AutoSpells)
                                ImportTasks.Add(ImportSpellsAsync(spellCombo, gameMode));

                            int i = 0;
                            Parallel.ForEach(ImportTasks, async task => {
                                try { await task; isError[i] = false; }
                                catch { isError[i] = true; }
                                i++;
                            });
                            currentPerks = null;
                        }
                    }
                }
                catch
                {
                    WarningStatus = $"{getCurrentTime} Failed to import Runes & Spells.";
                    warningStringBuilder.AppendLine($"{getCurrentTime} Failed to import {FixedName(ChampionName)} Runes & Spells.");
                    setImportStatus("Uh oh, LoL Assist failed to import Runes & Spells", LogType.EROR);
                }
                finally
                {
                    sw.Stop();
                    if (isError[0])
                        warningStringBuilder.AppendLine($"{getCurrentTime} Failed to import {FixedName(ChampionName)} Runes.");
                    else if (isError[1])
                        warningStringBuilder.AppendLine($"{getCurrentTime} Failed to import {FixedName(ChampionName)} Spells.");
                    else if (isError[0] && isError[1])
                        setImportStatus($"Uh Oh, something went wrong.\ntry deleting {FixedName(ChampionName)} data might fix the issue", LogType.EROR);
                    else
                        setImportStatus($"Runes & Spells has been imported successfully! Elapsed [{sw.ElapsedMilliseconds}ms]");

                    WarningStatus = warningStringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
                }
                isBusy = false;
            }
        }

        private async void phaseChanged(object sender, PhaseMonitor.PhaseChangedArgs e)
        {
            Topmost = e.currentPhase == Phase.ChampSelect || e.currentPhase == Phase.ReadyCheck;
            currentGameMode = await LCUWrapper.GetCurrentGameModeAsync();

            if (e.currentPhase != Phase.InProgress)
            {
                GMode = currentGameMode;
                dispatcherInvoke(() => { // Needs to be invoke to fix delay issue
                    if (currentGameMode == GameMode.CLASSIC
                    || currentGameMode == GameMode.PRACTICETOOL)
                    {
                        if(e.currentPhase == Phase.Lobby) SelectedRole = Roles[0];

                        RoleSelectVisibility = Visibility.Visible;
                    }
                    else RoleSelectVisibility = Visibility.Collapsed;
                });
            }

            switch (e.currentPhase)
            {
                case Phase.ChampSelect:
                    dispatcherInvoke(() => {
                        ImportStatus = "Selecting a champion...";
                        ManualImportVisibility = Visibility.Visible;
                    });
                    break;
                case Phase.None:
                case Phase.Lobby:
                case Phase.WaitingForStats:
                    resetStatus();
                    break;
                case Phase.EndOfGame:
                    resetStatus();
                    ConnectionStatus = await getSummonerInfoAsync();
                    break;
                case Phase.InProgress:
                    dispatcherInvoke(() => {
                        ImportStatus = "Game in progress...";
                        ManualImportVisibility = Visibility.Collapsed;
                    });
                    break;
                case Phase.Matchmaking:
                    dispatcherInvoke(() => {
                        clearChamp();
                        ImportStatus = "Queueing...";
                    });
                    break;
                case Phase.ReadyCheck:
                    setTopMostContent(new MatchFoundViewModel());
                    break;
            }
        }

        private void dispatcherInvoke(Action action) => Application.Current.Dispatcher.Invoke(action);

        #region LoL Checker
        // Checks for LeagueClient process and get summoner info
        private bool updateConnection = false;
        public async void LoLMonitor()
        {
            var waitingForLeague = $"Waiting for {PROCESS_NAME} to start...";
            WorkingStatus = waitingForLeague;
            while (true)
            {
                var isProcessFound = Process.GetProcessesByName(PROCESS_NAME).Length > 0;
                var updateNeeded = isProcessFound != updateConnection;

                if (updateNeeded)
                {
                    if (isProcessFound)
                    {
                        WorkingStatus = $"Connecting to {PROCESS_NAME}...";
                        ConnectionStatus = CONNECTING;
                        string summonerInfo = null;
                        do
                        {
                            summonerInfo = await getSummonerInfoAsync();
                            Thread.Sleep(ConfigModel.s_Config.MonitoringDelay * 2);
                        } while (string.IsNullOrEmpty(summonerInfo));

                        dispatcherInvoke(() => {
                            WorkingStatusVisibility = Visibility.Collapsed;
                            ChampionContainerVisibility = Visibility.Visible;
                        });

                        ConnectionStatus = summonerInfo;
                        phaseChanged(null, new PhaseMonitor.PhaseChangedArgs(await LCUWrapper.GetGamePhaseAsync()));
                    }
                    else
                    {
                        ConnectionStatus = DISCONNECTED;
                        WorkingStatus = waitingForLeague;
                        dispatcherInvoke(() => {
                            WorkingStatusVisibility = Visibility.Visible;
                            ChampionContainerVisibility = Visibility.Collapsed;
                        });
                    }
                    updateConnection = isProcessFound;
                }

                Thread.Sleep(ConfigModel.s_Config.MonitoringDelay * 2);
            }
        }
        #endregion

        private void shutdownAppExecute() => showMsgBox(new Action(() => { Application.Current.MainWindow.Close(); }),
        msg: "Are you sure you want to exit LoL Assist?", width: 230, height: 150);
        private void showPatchNotes(bool doNotShow) { if (!doNotShow) setTopMostContent(new PatchViewModel()); }
        private void championChanged(object sender, ChampionMonitor.ChampionChangedArgs e) => importBuilds(e.championName);
        private async void reImport() => await Task.Run(async () => { importBuilds(await LCUWrapper.GetCurrentChampionAsyncV2(), true); });
        private void showRuneEditorPanel() => setTopMostContent(new RuneEditorViewModel());

        #region Tray Icon
        private void initTrayIcon()
        {
            string[] menuOptions = new string[] { "Exit", "Show", "Edit Runes", "Patch Notes", "Minimize to Tray" };
            var trayIcon = new System.Windows.Forms.NotifyIcon();
            var menus = new List<System.Windows.Forms.MenuItem>();
            var menuContainer = new System.Windows.Forms.ContextMenu();

            for (int i = 0; i < menuOptions.Length; i++)
            {
                var item = new System.Windows.Forms.MenuItem()
                {
                    Index = i,
                    Text = menuOptions[i]
                };

                item.Click += new EventHandler(menuItem_Click);
                menus.Add(item);
            }

            menuContainer.MenuItems.AddRange(menus.ToArray());

            try
            {
                using var stream = Application.GetResourceStream(new Uri(ICON_PATH)).Stream;
                trayIcon.Icon = new System.Drawing.Icon(stream);
            }
            catch { }

            trayIcon.Visible = true;
            trayIcon.ContextMenu = menuContainer;
            trayIcon.DoubleClick += (o, args) => showMainWindow();
            trayIcon.Text = $"LoL Assist v{ConfigModel.r_Version}";

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
            switch (menuItem.Text)
            {
                case "Exit":
                    Application.Current.MainWindow.Close();
                    break;
                case "Show":
                    showMainWindow();
                    break;
                case "Edit Runes":
                    showMainWindow();
                    showRuneEditorPanel();
                    break;
                case "Patch Notes":
                    showMainWindow();
                    showPatchNotes(false);
                    break;
                case "Minimize to Tray":
                    minimizeToTray();
                    break;
            }
        }

        private void minimizeToTray()
        {
            if(Application.Current.MainWindow.IsVisible)
                showMsgBox(new Action(() => { Application.Current.MainWindow.Hide(); }),
               $"Minimize {Application.Current.MainWindow.Title} to tray?", width: 230, height: 150);
        }

        private void showMainWindow()
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.Activate();
        }
        #endregion

        #region Debug D:
        //private int debugI = 0;
        //public async Task DebugExecute()
        //{
        //    LogService.Log(LogService.Model($"--------------attempt-{debugI}", "Debug", LogType.DBUG));
        //    //* Debug Code *//
        //    //await Main.RequestBuildsData("Aatrox", GameMode.URF, BuildsProvider.METAsrc);
        //    var build = await Main.RequestBuildsData("Ahri", GameMode.CLASSIC, Provider.UGG, Role.RECOMENDED);

        //    Console.WriteLine(build.Name);
        //    Console.WriteLine(build.Rune.PrimaryPath);
        //    Console.WriteLine(build.Rune.SecondaryPath);
        //    Console.WriteLine(build.Rune.Keystone);
        //    Console.WriteLine(build.Rune.Slot1);
        //    Console.WriteLine(build.Rune.Slot2);
        //    Console.WriteLine(build.Rune.Slot3);
        //    Console.WriteLine(build.Rune.Slot4);
        //    Console.WriteLine(build.Rune.Slot5);
        //    Console.WriteLine(build.Rune.Shard1);
        //    Console.WriteLine(build.Rune.Shard2);
        //    Console.WriteLine(build.Rune.Shard3);

        //    //Console.WriteLine(await LCUWrapper.GetCurrentSessionAsync());
        //    debugI++;
        //}
        #endregion

        #region Space junk
        private async void setTopMostContent(object obj)
        {
            TopMostContent = null;
            await Task.Delay(1); // Fix
            TopMostContent = obj;
        }
        private void showMsgBox(Action action, string msg, double width = 330, double height = 220)
        {
            setTopMostContent(new MessageBoxViewModel()
            {
                Message = msg,
                Width = width,
                Height = height,
                Action = () => { action.Invoke(); }
            });
        }
        private async Task<string> getSummonerInfoAsync()
        {
            var summonerInfo = await LCUWrapper.GetCurrentSummonerAsync();
            if (summonerInfo == null)
                return null;
            else if (summonerInfo.displayName == null
            || summonerInfo.summonerLevel == 0)
                return null;

            return $"{summonerInfo?.displayName} | Lvl {summonerInfo?.summonerLevel}";
        }
        private void setImportStatus(string msg, LogType logType = LogType.INFO)
        {
            dispatcherInvoke(() => { ImportStatus = msg; });
            Log(msg, logType);
        }
        private void clearChamp(bool hideManualImport = true)
        {
            ChampionImage = null;
            ChampionName = string.Empty;

            if (hideManualImport)
                ManualImportVisibility = Visibility.Hidden;
        }
        private void resetStatus()
        {
            clearChamp();
            ManualImportVisibility = Visibility.Collapsed;
            ImportStatus = "Waiting for a game queue...";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
