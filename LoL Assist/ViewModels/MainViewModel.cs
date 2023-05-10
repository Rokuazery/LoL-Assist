using LoLAHelper = LoLA.Networking.WebWrapper.DataProviders.Utils.Helper;
using LoLA.Networking.WebWrapper.DataProviders.Utils;
using LoLA.Networking.WebWrapper.DataProviders.OPGG;
using LoLA.Networking.WebWrapper.DataDragon.Data;
using static LoL_Assist_WAPP.Models.LoLAWrapper;
using LoLA.Networking.WebWrapper.DataDragon;
using static LoL_Assist_WAPP.Utils.Helper;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using LoLA.Networking.LCU.Events;
using LoLA.Networking.LCU.Enums;
using LoLA.Networking.LCU.Data;
using LoL_Assist_WAPP.Commands;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Models;
using LoL_Assist_WAPP.Utils;
using System.Windows.Input;
using LoLA.Networking.LCU;
using System.Diagnostics;
using LoLA.DataProviders;
using LoLA.Utils.Logger;
using System.Threading;
using LoLA.Data.Enums;
using System.Windows;
using System.Text;
using System.Linq;
using LoLA.Data;
using System;
using LoLA;

namespace LoL_Assist_WAPP.ViewModels
{
    public class MainViewModel: ViewModelBase
    {
        #region Miscellaneous

        #region Commands
        public ICommand ManualImportCommand { get; }
        public ICommand ShowEditRunesPanelCommand { get; }
        public ICommand ShowDownloadPanelCommand { get; }
        public ICommand ShowAutoMessagePanelCommand { get; }

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
        private ChampionSkill _selectedChampionSkill;
        public ChampionSkill SelectedChampionSkill
        {
            get => _selectedChampionSkill;
            set
            {
                if (_selectedChampionSkill != value)
                {
                    _selectedChampionSkill = value;
                    OnPropertyChanged(nameof(SelectedChampionSkill));
                }
            }
        }

        private Rune _selectedRune;
        public Rune SelectedRune
        {
            get => _selectedRune;
            set
            {
                if (_selectedRune != value)
                {
                    _selectedRune = value;
                    OnPropertyChanged(nameof(SelectedRune));
                }
            }
        }

        private Spell _selectedSpell;
        public Spell SelectedSpell
        {
            get => _selectedSpell;
            set
            {
                if (_selectedSpell != value)
                {
                    _selectedSpell = value;
                    OnPropertyChanged(nameof(SelectedSpell));
                }
            }
        }

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

        private ObservableCollection<Provider> _providers;
        public ObservableCollection<Provider> Providers
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

        private Provider _selectedProvider;
        public Provider SelectedProvider
        {
            get => _selectedProvider;
            set
            {
                if (_selectedProvider != value)
                {
                    ConfigModel.s_CurrentProvider = value;
                    ConfigModel.s_Config.Provider = value;
                    ConfigModel.SaveConfig();
                    ReImport();

                    _selectedProvider = value;
                    SelectedRole = Role.RECOMENDED;
                    OnPropertyChanged(nameof(SelectedProvider));
                }
            }
        }

        private ObservableCollection<Role> _roles;
        public ObservableCollection<Role> Roles
        {
            get
            {
                // Get available role for OP.GG
                if (SelectedProvider != Provider.OPGG || OPGGRankedRoot.instance == null)
                    return _roles;

                var selectedChampData = OPGGRankedRoot.instance.data.SingleOrDefault(d => d.id.ToString() == Converter.ChampionNameToKey(ChampionName));

                if(selectedChampData == null) return _roles;

                var availableRoles = new ObservableCollection<Role>();

                for (int i = 0; i < selectedChampData.positions.Length; i++)
                {
                    foreach (var role in _roles)
                    {
                        var name = Enum.GetName(typeof(Role), role);
                        if (name.ToLower().Trim() != selectedChampData.positions[i].name.ToLower().Trim())
                            continue;

                        availableRoles.Add(role);
                    }
                }

                // If the SelectedRole is not contained in availableRoles, then the first index of availableRoles is selected.
                SelectedRole = availableRoles.Contains(SelectedRole) ? SelectedRole : availableRoles.FirstOrDefault();

                return availableRoles;
            }
            set
            {
                if (_roles != value)
                {
                    _roles = value;
                    OnPropertyChanged(nameof(Roles));
                }
            }
        }

        private Role _selectedRole;
        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (_selectedRole != value)
                {
                    ReImport();
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

        private int _selectedBuildIndex;
        public int SelectedBuildIndex
        {
            get => _selectedBuildIndex;
            set
            {
                if (_selectedBuildIndex != value)
                {
                    _selectedBuildIndex = value;
                    OnPropertyChanged(nameof(SelectedBuildIndex));
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

        private string _gDetail;
        public string GDetail
        {
            get => _gDetail;
            set
            {
                if (_gDetail != value)
                {
                    _gDetail = value;
                    OnPropertyChanged(nameof(GDetail));
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

        private bool IsBusy = false;
        private SessionData CurrentSessionData = new SessionData();
        private readonly StringBuilder WarningStringBuilder = new StringBuilder();

        private ulong SummonerId = 0;
        private const string PROCESS_NAME = "LeagueClientUx";
        private const string CONNECTING = "Connecting";
        private const string DISCONNECTED = "Disconnected"; 
        private const string ICON_PATH = "pack://application:,,,/icon.ico";
        private string getCurrentTime { get => $"[{DateTime.Now:hh:mm:ss}]"; }

        public MainViewModel()
        {
            SelectedRole = Role.RECOMENDED;
            GMode = CurrentSessionData.GameMode;
            ManualImportCommand = new Command(_ => ReImport());
            ShutdownAppCommand = new Command(_ => ShutdownAppExecute());
            ShowEditRunesPanelCommand = new Command(_ => ShowRuneEditorPanel());
            ShowDownloadPanelCommand = new Command(_ => SetTopMostContent(new DownloadViewModel()));
            ShowAutoMessagePanelCommand = new Command(_ => SetTopMostContent(new MessageViewModel()));
            MoveWindowCommand = new Command(_ => { Application.Current.MainWindow.DragMove(); });

            MinimizeToTrayCommand = new Command(_ => MinimizeToTray());
            MinimizeWindowCommand = new Command(_ => Application.Current.MainWindow.WindowState = WindowState.Minimized);

            InitTrayIcon();

            WorkingStatus = $"Clearing old logs...";
            LogService.Clear();

            WorkingStatus = "Loading in configuration...";
            ConfigModel.LoadConfig();

            Roles = new ObservableCollection<Role>(Enum.GetValues(typeof(Role)).Cast<Role>());

            var providers = Enum.GetValues(typeof(Provider)).Cast<Provider>().ToList();
            providers.Remove(providers.Last());

            Providers = new ObservableCollection<Provider>(providers);

            ConfigModel.s_CurrentProvider = ConfigModel.s_Config.Provider;
            SelectedProvider = ConfigModel.s_Config.Provider;

            InitLoLA();
        }

        private async void InitLoLA()
        {
            if (ConfigModel.s_Config.UpdateOnStartup)
            {
                WorkingStatus = "Checking for updates...";
                await Updater.Start();
            }

            WorkingStatus = "Downloading prerequisite...";
            await Main.Init();

            WorkingStatus = "Initializing LoLA components...";

            var lolMonitorThread = new Thread(LoLMonitor);
            lolMonitorThread.Start();

            s_ChampMonitor = new ChampionMonitor();
            s_ChampMonitor.MonitorDelay = ConfigModel.s_Config.UpdateDelay;
            s_ChampMonitor.ChampionChanged += ChampionChanged;

            s_PhaseMonitor = new PhaseMonitor();
            s_PhaseMonitor.MonitorDelay = ConfigModel.s_Config.UpdateDelay;
            s_PhaseMonitor.PhaseChanged += PhaseChanged;
            ResetStatus();
        }

        // Make this use CancellationToken later
        private async void ImportBuilds(string championName, bool forceRuneUpdate = false)
        {
            if (string.IsNullOrEmpty(championName)  
            || (!ConfigModel.s_Config.AutoRunes 
            && !ConfigModel.s_Config.AutoSpells))
                return;

            ClearChamp(false);

            while (IsBusy)
                Thread.Sleep(ConfigModel.s_Config.UpdateDelay);

            IsBusy = true;
            // make the runes selectable later!
            int ReplaceMeINT1 = 0;
            int ReplaceMeINT2 = 0;
            Stopwatch sw = Stopwatch.StartNew();
            ChampionName = championName;

            if(LoLAHelper.IsClassicGameMode(GMode))
                DispatcherInvoke(() => OnPropertyChanged(nameof(Roles)));

            var currentChampionId = Converter.ChampionNameToId(championName);
            var defaultConfigName = LocalBuild.GetLocalBuildName(currentChampionId, GMode);

            SetImportStatus($"Fetching Runes & Spells for {ChampionName}...");
            sw.Start();

            var provider = !string.IsNullOrEmpty(defaultConfigName)
            && defaultConfigName != Enum.GetName(typeof(Provider), ConfigModel.s_CurrentProvider)
            ? Provider.Local : ConfigModel.s_CurrentProvider;

            Task<ChampionBuild> championBuildTask;
            Task<string> championImageTask;

            Task[] tasks = new Task[] {
                    (championBuildTask = Main.RequestBuildsData(currentChampionId, GMode, provider, SelectedRole, SelectedBuildIndex)),
                    (championImageTask = DataDragonWrapper.GetChampionImage(currentChampionId))
                };

            try
            {
                await Task.WhenAll(tasks);

                for (int t = 0; t < tasks.Length; t++)
                {
                    if (tasks[t].Exception != null)
                        SetWarning(t, true);
                }

                ChampionImage = championImageTask.Result;

                SetImportStatus($"Importing {FixedName(ChampionName)} Runes & Spells...");

                var championBuild = championBuildTask.Result;
                SelectedChampionSkill = championBuild.ChampionSkill;


                if (championBuild != null)
                {
                    var currentPerks = await LCUWrapper.GetCurrentRunePageAsync();

                    if (ConfigModel.s_Config.AutoRunes)
                    {
                        DispatcherInvoke(() => SelectedRune = championBuild?.Runes[ReplaceMeINT1]);
                        if (!await SetRuneAsync(SelectedRune, currentPerks, forceRuneUpdate))
                            SetWarning(0, false);
                    }

                    if (ConfigModel.s_Config.AutoSpells)
                    {
                        DispatcherInvoke(() => SelectedSpell = championBuild?.Spells[ReplaceMeINT2]);
                        // SelectedSpell needs to be cloned because of a referencing issue
                        if (!await ImportSpellsAsync(SelectedSpell.Clone(), GMode))
                            SetWarning(1, false);
                    }

                    currentPerks = null;
                }
            }
            catch (Exception ex)
            {
                WarningStringBuilder.AppendLine($"{getCurrentTime} Unexcpected error: {ex.Message}");
                WarningStatus = WarningStringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
                SetImportStatus($"{SelectedProvider} error: {ex.Message}", LogType.EROR);
            }
            finally
            {
                sw.Stop();
                IsBusy = false;
            }

    
            string status = ConfigModel.s_Config.AutoRunes && ConfigModel.s_Config.AutoSpells ? "Runes & Spells" : ConfigModel.s_Config.AutoRunes ? "Runes" : "Spells";
            string importStatus = $"{status} have been imported successfully! Elapsed [{sw.ElapsedMilliseconds}ms]";
            SetImportStatus(importStatus);
        }

        private void SetWarning(int errorCode, bool isFetch)
        {
            if(isFetch)
            {
                switch (errorCode)
                {
                    case 0:
                        WarningStringBuilder.AppendLine($"{getCurrentTime} Failed to fetch {FixedName(ChampionName)} Runes & Spells data.");
                        break;
                    case 1:
                        WarningStringBuilder.AppendLine($"{getCurrentTime} Failed to fetch {FixedName(ChampionName)} image.");
                        break;
                    default:
                        return;

                }

                WarningStatus = WarningStringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
                return;
            }

            switch (errorCode)
            {
                case 0:
                    WarningStringBuilder.AppendLine($"{getCurrentTime} Failed to import {FixedName(ChampionName)} Runes.");
                    break;
                case 1:
                    WarningStringBuilder.AppendLine($"{getCurrentTime} Failed to import {FixedName(ChampionName)} Spells.");
                    break;
                default:
                    return;

            }

            WarningStatus = WarningStringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }


        private async void PhaseChanged(object sender, PhaseMonitor.PhaseChangedArgs e)
        {
            if (e == null) return;

            Topmost = e.currentPhase == Phase.ChampSelect || e.currentPhase == Phase.ReadyCheck;
            CurrentSessionData = await LCUWrapper.GetSessionDataAsync();
      
            if (e.currentPhase != Phase.InProgress)
            {
                DispatcherInvoke(() => { // Needs to be invoke to fix delay issue

                    GMode = CurrentSessionData?.GameMode ?? GameMode.NONE;

                    RoleSelectVisibility = LoLAHelper.IsClassicGameMode(GMode)
                    && e.currentPhase == Phase.ChampSelect ? Visibility.Visible : Visibility.Collapsed;

                    SelectedRole = Roles[0];
                });
            }

            switch (e.currentPhase)
            {
                case Phase.ChampSelect:
                    ExecuteAutoMessage();
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
                    ConnectionStatus = await GetSummonerInfoAsync();
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
            SetSessionDetail(CurrentSessionData);
        }

        private void SetSessionDetail(SessionData sessionData)
        {
            if (sessionData == null || string.IsNullOrWhiteSpace(sessionData.Category)
                || string.IsNullOrWhiteSpace(sessionData.Description))
            {
                GDetail = string.Empty;
                return;
            }

            string description = sessionData.Description == "ARAM" ? null : $"| {sessionData.Description}";
            GMode = GMode == GameMode.NONE && sessionData.Description == "ARAM" ? GameMode.ARAM : GMode;
            GDetail = $"{description} | {sessionData.Category}";
        }

        private async void ExecuteAutoMessage()
        {
            if (!ConfigModel.s_Config.AutoMessage || string.IsNullOrEmpty(ConfigModel.s_Config.Message))
                return;

            Log("Sending a message...", LogType.INFO);
            var type = "championSelect";
            var messageSent = false;
            try
            {
                do
                {
                    var conversations = await LCUWrapper.GetConversationsAsync();
                    await LCUWrapper.SendMessageAsync(conversations, ConfigModel.s_Config.Message, type);

                    foreach (var conversation in conversations.Root)
                    {
                        if (conversation["type"].ToString() == type)
                        {
                            int? chatIndex;
                            do
                            {
                                var id = conversation["id"].ToString();
                                var messages = await LCUWrapper.GetMessagesAsync(id);
                                chatIndex = messages?.Count;
                            } while (chatIndex > 0);
                            messageSent = true;
                        }
                    }

                    Thread.Sleep(1);
                } while (!messageSent);
            }
            catch
            {
                messageSent = false;
                Log("Uh oh something went wrong when trying to send a message", LogType.EROR);
            }

            if (ConfigModel.s_Config.ClearMessageAfterSent && messageSent)
                ConfigModel.s_Config.Message = string.Empty;
        }

        private void DispatcherInvoke(Action action) 
            => Application.Current.Dispatcher.Invoke(action);

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

                if (!updateNeeded)
                {
                    Thread.Sleep(ConfigModel.s_Config.UpdateDelay * 2);
                    continue;
                }

                if (isProcessFound)
                {
                    WorkingStatus = $"Connecting to {PROCESS_NAME}...";
                    ConnectionStatus = CONNECTING;
                    string summonerInfo = null;
                    do
                    {
                        summonerInfo = await GetSummonerInfoAsync();
                        Thread.Sleep(ConfigModel.s_Config.UpdateDelay * 2);
                    } while (string.IsNullOrEmpty(summonerInfo));

                    DispatcherInvoke(() => {
                        WorkingStatusVisibility = Visibility.Collapsed;
                        ChampionContainerVisibility = Visibility.Visible;
                    });

                    ConnectionStatus = summonerInfo;
                    PhaseChanged(null, new PhaseMonitor.PhaseChangedArgs(await LCUWrapper.GetGamePhaseAsync()));
                }
                else
                {
                    ConnectionStatus = DISCONNECTED;
                    WorkingStatus = waitingForLeague;
                    DispatcherInvoke(() => {
                        WorkingStatusVisibility = Visibility.Visible;
                        ChampionContainerVisibility = Visibility.Collapsed;
                    });
                }

                updateConnection = isProcessFound;

                Thread.Sleep(ConfigModel.s_Config.UpdateDelay * 2);
            }
        }
        #endregion

        private void ShutdownAppExecute() => ShowMsgBox(new Action(() => { Application.Current.MainWindow.Close(); }),
        msg: "Are you sure you want to exit LoL Assist?", width: 230, height: 150);
        private void ShowRuneEditorPanel() => SetTopMostContent(new RuneEditorViewModel());
        private void ChampionChanged(object sender, ChampionMonitor.ChampionChangedArgs e) => ImportBuilds(e.championName, true);
        private async void ReImport() => await Task.Run(async () => {
            var currentChampion = (await LCUWrapper.GetCurrentChampionIdAsync(SummonerId)).ToString();
            currentChampion = Converter.ChampionKeyToName(currentChampion);
            ImportBuilds(currentChampion, true); 
        });

        #region Tray Icon

        private readonly string[] r_menuOptions = { "Exit", "Show", "Edit Runes", "Minimize to Tray" };
        private void InitTrayIcon()
        {
            var trayIcon = new System.Windows.Forms.NotifyIcon();
            var menus = new List<System.Windows.Forms.MenuItem>();
            var menuContainer = new System.Windows.Forms.ContextMenu();

            for (int i = 0; i < r_menuOptions.Length; i++)
            {
                var item = new System.Windows.Forms.MenuItem()
                {
                    Index = i,
                    Text = r_menuOptions[i]
                };

                item.Click += new EventHandler(MenuItem_Click);
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
            trayIcon.DoubleClick += (o, args) => ShowMainWindow();
            trayIcon.Text = $"LoL Assist v{ConfigModel.r_Version}";

            // Dispose tray icon on closed
            Application.Current.MainWindow.Closed += (o, args) => {
                trayIcon.Icon = null;
                trayIcon.Dispose();
                Environment.Exit(0);
            };
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as System.Windows.Forms.MenuItem;
            switch (menuItem.Text)
            {
                case "Exit":
                    Application.Current.MainWindow.Close();
                    break;
                case "Show":
                    ShowMainWindow();
                    break;
                case "Edit Runes":
                    ShowMainWindow();
                    ShowRuneEditorPanel();
                    break;
                case "Minimize to Tray":
                    Application.Current.MainWindow.Hide();
                    break;
            }
        }

        private void MinimizeToTray()
        {
            if(Application.Current.MainWindow.IsVisible)
                ShowMsgBox(new Action(() => { Application.Current.MainWindow.Hide(); }),
               $"Minimize {Application.Current.MainWindow.Title} to tray?", width: 230, height: 150);
        }

        private void ShowMainWindow()
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.Activate();
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
            SetTopMostContent(new MessageBoxModel()
            {
                Message = msg,
                Width = width,
                Height = height,
                Action = () => { action.Invoke(); }
            });
        }

        private async Task<string> GetSummonerInfoAsync()
        {
            var summonerInfo = await LCUWrapper.GetCurrentSummonerAsync();

            if (summonerInfo?.displayName == null || summonerInfo.summonerId == 0)
                return null;

            // Save it for later as soon as this method get call for later use in api request
            SummonerId = summonerInfo.summonerId;
            s_ChampMonitor.SummonerId = summonerInfo.summonerId;
            return $"{summonerInfo?.displayName} | Lvl {summonerInfo?.summonerLevel}";
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
            SelectedRune = null;
            SelectedSpell = null;
            SelectedChampionSkill = null;

            if (hideManualImport)
                ManualImportVisibility = Visibility.Hidden;
        }

        private void ResetStatus()
        {
            ClearChamp();
            ManualImportVisibility = Visibility.Collapsed;
            ImportStatus = "Waiting for a game queue...";
        }
        #endregion
    }
}
