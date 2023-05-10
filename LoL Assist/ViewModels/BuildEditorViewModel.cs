using LoLA.Networking.WebWrapper.DataDragon.Data;
using LoLA.Networking.WebWrapper.DataDragon;
using static LoL_Assist_WAPP.Utils.Helper;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using LoL_Assist_WAPP.Converters;
using LoLA.Networking.LCU.Enums;
using LoL_Assist_WAPP.Commands;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Models;
using System.Windows.Input;
using System.Threading;
using Newtonsoft.Json;
using System.Windows;
using System.Linq;
using LoLA.Data;
using System.IO;
using System;
using LoLA;
using LoLA.DataProviders;

namespace LoL_Assist_WAPP.ViewModels
{
    public class BuildEditorViewModel: ViewModelBase
    {
        #region Commands
        public ICommand SaveCommand { get; }
        public ICommand SetAsDefaultCommand { get; }
        public ICommand ClearDefaultSourceCommand { get; }
        public ICommand MoveWindowCommand { get; }
        private void SetAsDefaultExecute()
        {
            try
            {
                if (SelectedBuildName != null && !SelectedBuildName.Equals(CREATE_NEW)
                && !string.IsNullOrEmpty(SelectedBuildName))
                {
                    DefaultBuildConfig.SetDefaultConfig(SelectedGameMode, SelectedBuildName);
                    LocalBuild.WriteDefaultBuildConfig(selectedChampionId, DefaultBuildConfig);
                    SlDefaultConfig = DefaultBuildConfig.GetDefaultConfig(SelectedGameMode);
                }
            }
            catch { /*ex*/ }
        }

        public void DeleteConfigExecute()
        {
            if(SelectedBuildName != null && !SelectedBuildName.Equals(CREATE_NEW))
            {
                var buildName = SelectedBuildName;
                SelectedBuildName = null;
                
                var _buildsName = new List<string>();
                _buildsName.Add(CREATE_NEW);

                LocalBuild.DeleteData(selectedChampionId, Path.GetFileNameWithoutExtension(buildName), SelectedGameMode);
                foreach (var path in LocalBuild.GetBuildFiles(selectedChampionId, SelectedGameMode))
                    _buildsName.Add(Path.GetFileName(path));

                BuildsName = _buildsName;

                var customBuildPath = LocalBuild.BuildsFolder(selectedChampionId, SelectedGameMode);
                var fullPath = $"{customBuildPath}\\{DefaultBuildConfig.GetDefaultConfig(SelectedGameMode)}";

                if (!File.Exists(fullPath))
                    DefaultBuildConfig.ResetDefaultConfig(SelectedGameMode);

                LocalBuild.WriteDefaultBuildConfig(selectedChampionId, DefaultBuildConfig);
                SlDefaultConfig = DefaultBuildConfig.GetDefaultConfig(SelectedGameMode);
            }
        }

        private void ClearDefaultSourceExecute()
        {
            if (SelectedChampion != null && SelectedGameMode != GameMode.NONE)
            {
                DefaultBuildConfig.ResetDefaultConfig(SelectedGameMode);
                LocalBuild.WriteDefaultBuildConfig(selectedChampionId, DefaultBuildConfig);
                SlDefaultConfig = DefaultBuildConfig.GetDefaultConfig(SelectedGameMode);
            }
        }

        private async void SaveConfigExecute()
        {
            if(!string.IsNullOrEmpty(FileName) && SelectedChampion != null && SelectedGameMode != GameMode.NONE)
            {
                var filePath = LocalBuild.DataPath(selectedChampionId, FileName, SelectedGameMode);

                if (File.Exists(filePath))
                    File.Create(filePath).Dispose();

                using(var streaWriter = new StreamWriter(filePath))
                {
                    var json = JsonConvert.SerializeObject(selectedBuild);
                    streaWriter.Write(json);
                    referenceBuild = json;
                }

                var _buildsName = new List<string>() { CREATE_NEW };
                foreach (var path in LocalBuild.GetBuildFiles(selectedChampionId, SelectedGameMode))
                    _buildsName.Add(Path.GetFileName(path));

                BuildsName = _buildsName;
                SelectedBuildName = Path.GetFileName(filePath);
                await Saved();
            }
            else if(string.IsNullOrEmpty(FileName))
            {
                WarningText = "'file name:' field cannot be empty!";
                await Task.Delay(2500);
                WarningText = null;
            }
        }
        #endregion

        #region Needed
        private ObservableCollection<string> _champions = new ObservableCollection<string>();
        public ObservableCollection<string> Champions
        {
            get => _champions;
            set
            {
                if (_champions != value)
                {
                    _champions = value;
                    OnPropertyChanged(nameof(Champions));
                }
            }
        }

        private ObservableCollection<string> _gameModes = new ObservableCollection<string>();
        public ObservableCollection<string> GameModes
        {
            get => _gameModes;
            set
            {
                if (_gameModes != value)
                {
                    _gameModes = value;
                    OnPropertyChanged(nameof(GameModes));
                }
            }
        }

        private string _selectedChampion;
        public string SelectedChampion
        {
            get => _selectedChampion;
            set
            {
                if (_selectedChampion != value)
                {
                    _selectedChampion = value;
                    OnPropertyChanged(nameof(SelectedChampion));
                    DataChanged();
                }
            }
        }

        private GameMode _selectedGameMode;
        public GameMode SelectedGameMode
        {
            get => _selectedGameMode;
            set
            {
                if (_selectedGameMode != value)
                {
                    SlGameMode = value;
                    _selectedGameMode = value;
                    OnPropertyChanged(nameof(SelectedGameMode));
                    DataChanged();
                }
            }
        }

        private List<string> _buildsName = new List<string>();
        public List<string> BuildsName
        {
            get => _buildsName;
            set
            {
                if (_buildsName != value)
                {
                    _buildsName = value;
                    OnPropertyChanged(nameof(BuildsName));
                }
            }
        }

        private string _selectedBuildName;
        public string SelectedBuildName
        {
            get => _selectedBuildName;
            set
            {
                if (_selectedBuildName != value)
                {
                    _selectedBuildName = value;
                    OnPropertyChanged(nameof(SelectedBuildName));
                    FetchBuild();
                }
            }
        }
        #endregion

        #region Editable
        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set
            {
                if (_fileName != value)
                {
                    _fileName = value;
                    OnPropertyChanged(nameof(FileName));
                    IsChanged();
                }
            }
        }

        #region Spells
        public List<ItemImageModel> _spells = new List<ItemImageModel>();
        public List<ItemImageModel> Spells
        {
            get => _spells;
            set
            {
                if (_spells != value)
                {
                    _spells = value;
                    OnPropertyChanged(nameof(Spells));
                }
            }
        }

        private ItemImageModel _selectedSpell1;
        public ItemImageModel SelectedSpell1
        {
            get => _selectedSpell1;
            set    
            {
                if (_selectedSpell1 != value)
                {
                    _selectedSpell1 = value;
                    SelectedSpell2 = RemoveDup(SelectedSpell1, SelectedSpell2);
                    if (value != null)
                    {
                        #if DEBUG
                        Console.WriteLine($"Selected First Spell: {selectedBuild.Spells.First().First} | val: {value.Text}");
                        #endif
                        selectedBuild.Spells.FirstOrDefault().First = DataConverter.SpellNameToSpellId(value.Text);
                    }
                    OnPropertyChanged(nameof(SelectedSpell1));
                    IsChanged();
                }
            }
        }

        private ItemImageModel _selectedSpell2;
        public ItemImageModel SelectedSpell2
        {
            get => _selectedSpell2;
            set
            {
                if (_selectedSpell2 != value)
                {
                    _selectedSpell2 = value;
                    SelectedSpell1 = RemoveDup(SelectedSpell2, SelectedSpell1);
                    if(value != null)
                    {
                        #if DEBUG
                        Console.WriteLine($"Selected Second Spell: {selectedBuild.Spells.First().Second} | val: {value.Text}");
                        #endif
                        selectedBuild.Spells.FirstOrDefault().Second = DataConverter.SpellNameToSpellId(value.Text);
                    }

                    OnPropertyChanged(nameof(SelectedSpell2));
                    IsChanged();
                }
            }
        }
        #endregion

        #region Perks
        private string _runeName;
        public string RuneName
        {
            get => _runeName;
            set
            {
                if (_runeName != value)
                {
                    _runeName = value;

                    if(value != null)
                        selectedBuild.Runes.FirstOrDefault().Name = value;

                    OnPropertyChanged(nameof(RuneName));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _runePaths = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> RunePaths
        {
            get => _runePaths;
            set
            {
                if (_runePaths != value)
                {
                    _runePaths = value;
                    OnPropertyChanged(nameof(RunePaths));
                }
            }
        }

        private ItemImageModel _selectedPath1;
        public ItemImageModel SelectedPath1
        {
            get => _selectedPath1;
            set
            {
                if (_selectedPath1 != value)
                {
                    _selectedPath1 = value;
                    SelectedPath2 = RemoveDup(SelectedPath1, SelectedPath2);
                    if(value != null)
                        selectedBuild.Runes.FirstOrDefault().PrimaryPath = Converter.PathNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedPath1));
                    path1Changed();
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _keystones = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Keystones
        {
            get => _keystones;
            set
            {
                if (_keystones != value)
                {
                    _keystones = value;
                    OnPropertyChanged(nameof(Keystones));
                }
            }
        }

        private ItemImageModel _selectedKeystone;
        public ItemImageModel SelectedKeystone
        {
            get => _selectedKeystone;
            set
            {
                if (_selectedKeystone != value)
                {
                    _selectedKeystone = value;
                    if(value != null)
                        selectedBuild.Runes.FirstOrDefault().Keystone = Converter.PerkNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedKeystone));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _perks1 = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perks1
        {
            get => _perks1;
            set
            {
                if (_perks1 != value)
                {
                    _perks1 = value;
                    OnPropertyChanged(nameof(Perks1));
                }
            }
        }

        private ItemImageModel _selectedPerk1;
        public ItemImageModel SelectedPerk1
        {
            get => _selectedPerk1;
            set
            {
                if (_selectedPerk1 != value)
                {
                    _selectedPerk1 = value;
                    if(value!=null)
                        selectedBuild.Runes.FirstOrDefault().Slot1 = Converter.PerkNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedPerk1));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _perks2 = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perks2
        {
            get => _perks2;
            set
            {
                if (_perks2 != value)
                {
                    _perks2 = value;
                    OnPropertyChanged(nameof(Perks2));
                }
            }
        }

        private ItemImageModel _selectedPerk2;
        public ItemImageModel SelectedPerk2
        {
            get => _selectedPerk2;
            set
            {
                if (_selectedPerk2 != value)
                {
                    _selectedPerk2 = value;
                    if(value != null)
                        selectedBuild.Runes.FirstOrDefault().Slot2 = Converter.PerkNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedPerk2));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _perks3 = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perks3
        {
            get => _perks3;
            set
            {
                if (_perks3 != value)
                {
                    _perks3 = value;
                    OnPropertyChanged(nameof(Perks3));
                }
            }
        }

        private ItemImageModel _selectedPerk3;
        public ItemImageModel SelectedPerk3
        {
            get => _selectedPerk3;
            set
            {
                if (_selectedPerk3 != value)
                {
                    _selectedPerk3 = value;
                    if(value!=null)
                        selectedBuild.Runes.First().Slot3 = Converter.PerkNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedPerk3));
                    IsChanged();
                }
            }
        }

        private ItemImageModel _selectedPath2;
        public ItemImageModel SelectedPath2
        {
            get => _selectedPath2;
            set
            {
                if (_selectedPath2 != value)
                {
                    _selectedPath2 = value;
                    if(value != null)
                        selectedBuild.Runes.FirstOrDefault().SecondaryPath = Converter.PathNameToId(value.Text);
                    SelectedPath1 = RemoveDup(SelectedPath2, SelectedPath1);

                    OnPropertyChanged(nameof(SelectedPath2));
                    path2Changed();
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _perks4 = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perks4
        {
            get => _perks4;
            set
            {
                if (_perks4 != value)
                {
                    _perks4 = value;
                    OnPropertyChanged(nameof(Perks4));
                }
            }
        }

        private ItemImageModel _selectedPerk4;
        public ItemImageModel SelectedPerk4
        {
            get => _selectedPerk4;
            set
            {
                if (_selectedPerk4 != value)
                {
                    _selectedPerk4 = value;
                    if(value!=null)
                        selectedBuild.Runes.FirstOrDefault().Slot4 = Converter.PerkNameToId(value.Text);
                    SelectedPerk5 = RemoveDup(value, SelectedPerk5);

                    OnPropertyChanged(nameof(SelectedPerk4));
                    Perk4Changed();
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _perks5 = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perks5
        {
            get => _perks5;
            set
            {
                if (_perks5 != value)
                {
                    _perks5 = value;
                    OnPropertyChanged(nameof(Perks5));
                }
            }
        }

        private ItemImageModel _selectedPerk5;
        public ItemImageModel SelectedPerk5
        {
            get => _selectedPerk5;
            set
            {
                if (_selectedPerk5 != value)
                {
                    _selectedPerk5 = value;
                    if(value != null)
                        selectedBuild.Runes.FirstOrDefault().Slot5 = Converter.PerkNameToId(value.Text);
                    SelectedPerk4 = RemoveDup(value, SelectedPerk4);

                    OnPropertyChanged(nameof(SelectedPerk5));
                    IsChanged();
                }
            }
        }
        #region Shards
        public ObservableCollection<ItemImageModel> _shards1 = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Shards1
        {
            get => _shards1;
            set
            {
                if (_shards1 != value)
                {
                    _shards1 = value;
                    OnPropertyChanged(nameof(Shards1));
                }
            }
        }

        private ItemImageModel _selectedShard1;
        public ItemImageModel SelectedShard1
        {
            get => _selectedShard1;
            set
            {
                if (_selectedShard1 != value)
                {
                    _selectedShard1 = value;
                    if (value != null)
                        selectedBuild.Runes.FirstOrDefault().Shard1 = DataConverter.ShardDescriptionToShardId(value.Text);
                    SelectedPerk4 = RemoveDup(SelectedShard1, SelectedPerk4);

                    OnPropertyChanged(nameof(SelectedShard1));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _shards2 = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Shards2
        {
            get => _shards2;
            set
            {
                if (_shards2 != value)
                {
                    _shards2 = value;
                    OnPropertyChanged(nameof(Shards2));
                }
            }
        }

        private ItemImageModel _selectedShard2;
        public ItemImageModel SelectedShard2
        {
            get => _selectedShard2;
            set
            {
                if (_selectedShard2 != value)
                {
                    _selectedShard2 = value;
                    if (value != null)
                        selectedBuild.Runes.FirstOrDefault().Shard2 = DataConverter.ShardDescriptionToShardId(value.Text);
                    SelectedPerk4 = RemoveDup(SelectedShard2, SelectedPerk4);

                    OnPropertyChanged(nameof(SelectedShard2));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _shards3 = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Shards3
        {
            get => _shards3;
            set
            {
                if (_shards3 != value)
                {
                    _shards3 = value;
                    OnPropertyChanged(nameof(Shards3));
                }
            }
        }

        private ItemImageModel _selectedShard3;
        public ItemImageModel SelectedShard3
        {
            get => _selectedShard3;
            set
            {
                if (_selectedShard3 != value)
                {
                    _selectedShard3 = value;
                    if (value != null)
                        selectedBuild.Runes.FirstOrDefault().Shard3 = DataConverter.ShardDescriptionToShardId(value.Text);
                    SelectedPerk4 = RemoveDup(SelectedShard3, SelectedPerk4);

                    OnPropertyChanged(nameof(SelectedShard3));
                    IsChanged();
                }
            }
        }
        #endregion

        #endregion

        #endregion

        #region SmallInfo
        private string _warningText;
        public string WarningText
        {
            get => _warningText;
            set
            {
                if (_warningText != value)
                {
                    _warningText = value;
                    OnPropertyChanged(nameof(WarningText));
                }
            }
        }

        private GameMode _slGameMode = GameMode.NONE;
        public GameMode SlGameMode
        {
            get => _slGameMode;
            set
            {
                if (_slGameMode != value)
                {
                    _slGameMode = value;
                    OnPropertyChanged(nameof(SlGameMode));
                }
            }
        }

        private string _slDefaultConfig = ProviderConverter.ToName(ConfigModel.s_CurrentProvider);
        public string SlDefaultConfig
        {
            get => _slDefaultConfig;
            set
            {
                if (_slDefaultConfig != value)
                {
                    if (string.IsNullOrEmpty(value))
                        _slDefaultConfig = ProviderConverter.ToName(ConfigModel.s_CurrentProvider);
                    else
                        _slDefaultConfig = value;
                    OnPropertyChanged(nameof(SlDefaultConfig));
                }
            }
        }

        private string _saveInfo;
        public string SaveInfo
        {
            get => _saveInfo;
            set
            {
                if (_saveInfo != value)
                {
                    _saveInfo = value;
                    OnPropertyChanged(nameof(SaveInfo));
                }
            }
        }
        #endregion

        private string referenceBuild;
        private string selectedChampionId;
        public DefaultBuildConfig DefaultBuildConfig;
        private const string CREATE_NEW = "Create New*";
        private ChampionBuild selectedBuild = new ChampionBuild();
        public BuildEditorViewModel()
        {
            Init();

            Shards1 = RuneModel.Shards(1, false);
            Shards2 = RuneModel.Shards(2, false);
            Shards3 = RuneModel.Shards(3, false);

            MoveWindowCommand = new Command(wnd => DragMove(wnd));
            SaveCommand = new Command(_ => SaveConfigExecute());
            SetAsDefaultCommand = new Command(_ => SetAsDefaultExecute());
            ClearDefaultSourceCommand = new Command(_ => ClearDefaultSourceExecute());
        }

        private void DragMove(object wnd)
        {
            var window = wnd as Window;
            window.DragMove();
        }

        public async void Init()
        {
            var runePaths = new ObservableCollection<ItemImageModel>();

            await Task.Run(() => {
                Dictionary<string, Data> data = null;
                while (data == null)
                {
                    data = DataDragonWrapper.s_Champions?.Data;
                    Thread.Sleep(1000);
                }
                data = null;
            });

            foreach (var champion in DataDragonWrapper.s_Champions.Data.Values)
                Champions.Add(champion.name);

            GameModes = new ObservableCollection<string>(Enum.GetValues(typeof(GameMode))
            .Cast<GameMode>().Select(v => v.ToString()).Where(l => l != "NONE"));

            foreach (var perk in DataDragonWrapper.s_Perks)
                runePaths.Add(ItemImage(perk.key, ImageSrc(perk.key)));

            RunePaths = runePaths;
        }

        private async void DataChanged()
        {
            var buildsName = new List<string>();
            var spells = new List<ItemImageModel>();

            ClearProfile();

            if (!string.IsNullOrEmpty(SelectedChampion)
            && SelectedGameMode != GameMode.NONE)
            {
                SaveInfo = null;

                buildsName.Add(CREATE_NEW);
                selectedChampionId = Converter.ChampionNameToId(SelectedChampion);

                // Load in default build config
                DefaultBuildConfig = new DefaultBuildConfig();

                if (File.Exists(LocalBuild.DefaultConfigPath(selectedChampionId))) 
                    DefaultBuildConfig = LocalBuild.GetDefaultBuildConfig(selectedChampionId);

                var customBuildPath = LocalBuild.BuildsFolder(selectedChampionId, SelectedGameMode);
                var fullPath = $"{customBuildPath}\\{DefaultBuildConfig.GetDefaultConfig(SelectedGameMode)}";
                
                if(!File.Exists(fullPath)) DefaultBuildConfig.ResetDefaultConfig(SelectedGameMode);

                SlDefaultConfig = DefaultBuildConfig.GetDefaultConfig(SelectedGameMode);

                await Task.Run(() => {
                    foreach (var path in LocalBuild.GetBuildFiles(selectedChampionId, SelectedGameMode))
                        buildsName.Add(Path.GetFileName(path));

                    foreach (var spell in DataConverter.s_SpellNameToSpellKey)
                    {
                        if (SelectedGameMode == GameMode.ARAM && spell.Key.Equals("Smite")
                        || SelectedGameMode == GameMode.ARAM && spell.Key.Equals("Teleport")
                        || SelectedGameMode != GameMode.ARAM && spell.Key.Equals("Mark"))
                            continue;

                        spells.Add(ItemImage(spell.Key, ImageSrc(DataConverter.SpellNameToSpellId(spell.Key))));
                    }
                    Spells = spells;
                    BuildsName = buildsName;
                });
            }
            else
            {
                SelectedGameMode = GameMode.NONE;
                selectedBuild = null;
                BuildsName = null;
                SaveInfo = null;
                ClearSmallInfo();
            }
        }

        public void FetchBuild()
        {
            if (string.IsNullOrEmpty(SelectedBuildName) || SelectedBuildName.Equals(CREATE_NEW))
            {
                ClearProfile();
                SaveInfo = "New* Unsaved";
                return;
            }

            SaveInfo = null;

            var path = LocalBuild.GetBuildFiles(selectedChampionId, SelectedGameMode)
                .FirstOrDefault(p => Path.GetFileName(p) == SelectedBuildName);

            if (path == null)
            {
                ClearProfile();
                return;
            }

            try
            {
                using var streamReader = new StreamReader(path);
                var jsonContent = streamReader.ReadToEnd();

                FileName = Path.GetFileNameWithoutExtension(path);
                selectedBuild = JsonConvert.DeserializeObject<ChampionBuild>(jsonContent);
                referenceBuild = jsonContent;
                LoadBuild(selectedBuild);
            }
            catch (IOException ex)
            {
                Log($"Failed to read build file: {ex.Message}", LoLA.Utils.Logger.LogType.EROR);
                ClearProfile();
            }
        }

        private void path1Changed() 
        {
            Perks1 = new ObservableCollection<ItemImageModel>();
            Perks2 = new ObservableCollection<ItemImageModel>();
            Perks3 = new ObservableCollection<ItemImageModel>();
            Keystones = new ObservableCollection<ItemImageModel>();

            if (SelectedPath1 != null)
            {
                var perk = DataDragonWrapper.s_Perks.FirstOrDefault(p => p.name == SelectedPath1.Text);
                if (perk != null)
                {
                    int i = 0;
                    foreach (var slot in perk.slots)
                    {
                        foreach (var rune in slot.runes)
                        {
                            var model = new ItemImageModel()
                            {
                                Text = rune.name,
                                Image = ImageSrc(rune.name.Replace(":", string.Empty))
                            };

                            switch (i)
                            {
                                case 0:
                                    Keystones.Add(model);
                                    break;
                                case 1:
                                    Perks1.Add(model);
                                    break;
                                case 2:
                                    Perks2.Add(model);
                                    break;
                                case 3:
                                    Perks3.Add(model);
                                    break;
                            }
                        }
                        i++;
                    }
                }
            }
        }

        private string[,] secondarySlot = null;
        private void path2Changed()
        {
            Perks5 = new ObservableCollection<ItemImageModel>();
            Perks4 = new ObservableCollection<ItemImageModel>();
            var perks4 = new ObservableCollection<ItemImageModel>();

            if (SelectedPath2 != null)
            {
                secondarySlot = RuneModel.r_SecondPath[SelectedPath2.Text];
                foreach (var perk in secondarySlot)
                {
                    if(!string.IsNullOrEmpty(perk))
                    {
                        var model = new ItemImageModel()
                        {
                            Text = perk,
                            Image = ImageSrc(perk.Replace(":", string.Empty))
                        };
                        perks4.Add(model);
                    }
                }
                Perks4 = perks4;
            }
        }

        private void Perk4Changed()
        {
            if (SelectedPerk4 == null || string.IsNullOrEmpty(SelectedPerk4.Text))
                return;

            var perks5 = new ObservableCollection<ItemImageModel>();

            bool isFound = false;

            for (int r = 0; r < secondarySlot.GetLength(0) && !isFound; r++)
            {
                for (int c = 0; c < secondarySlot.GetLength(1) && !isFound; c++)
                {
                    if (SelectedPerk4.Text == secondarySlot[r, c])
                    {
                        isFound = true;

                        for (int r2 = 0; r2 < secondarySlot.GetLength(0); r2++)
                        {
                            if (r != r2)
                            {
                                for (int c2 = 0; c2 < secondarySlot.GetLength(1); c2++)
                                {
                                    string perk = secondarySlot[r2, c2];

                                    if (!string.IsNullOrEmpty(perk))
                                    {
                                        var model = new ItemImageModel
                                        {
                                            Text = perk,
                                            Image = ImageSrc(perk.Replace(":", string.Empty))
                                        };

                                        perks5.Add(model);
                                    }
                                }
                            }
                        }

                        Perks5 = perks5;
                    }
                }
            }
        }

        private void LoadBuild(ChampionBuild championBuild)
        {
            if(championBuild.Spells != null && championBuild.Spells.Count > 0)
            {
                var spell = championBuild.Spells.First();
                try
                {
                    SelectedSpell1 = Spells.SingleOrDefault(s => s.Text == DataConverter.SpellIdToSpellName(spell.First));
                    SelectedSpell2 = Spells.SingleOrDefault(s => s.Text == DataConverter.SpellIdToSpellName(spell.Second));
                }
                catch { }
            }
            else
            {
                selectedBuild.Spells = new List<Spell> { new Spell() };
                ClearSpells();
            }

            if(championBuild.Runes != null && championBuild.Runes.Count > 0)
            {
                var rune = championBuild.Runes.First();
                try
                {
                    RuneName = rune.Name;
                    SelectedPath1 = RunePaths.SingleOrDefault(p => p.Text == Converter.PathIdToName(rune.PrimaryPath));
                    SelectedKeystone = Keystones.SingleOrDefault(k => k.Text == Converter.PerkIdToName(rune.Keystone));
                    SelectedPerk1 = Perks1.SingleOrDefault(p => p.Text == Converter.PerkIdToName(rune.Slot1));
                    SelectedPerk2 = Perks2.SingleOrDefault(p => p.Text == Converter.PerkIdToName(rune.Slot2));
                    SelectedPerk3 = Perks3.SingleOrDefault(p => p.Text == Converter.PerkIdToName(rune.Slot3));

                    SelectedPath2 = RunePaths.SingleOrDefault(p => p.Text == Converter.PathIdToName(rune.SecondaryPath));
                    SelectedPerk4 = Perks4.SingleOrDefault(p => p.Text == Converter.PerkIdToName(rune.Slot4));
                    SelectedPerk5 = Perks5.SingleOrDefault(p => p.Text == Converter.PerkIdToName(rune.Slot5));
                }
                catch { }

                try
                {
                    SelectedShard1 = Shards1.SingleOrDefault(o => o.Text == DataConverter.ShardIdToShardDescription(rune.Shard1));
                    SelectedShard2 = Shards2.SingleOrDefault(f => f.Text == DataConverter.ShardIdToShardDescription(rune.Shard2));
                    SelectedShard3 = Shards3.SingleOrDefault(d => d.Text == DataConverter.ShardIdToShardDescription(rune.Shard3));
                }
                catch { }
            }
            else
            {
                selectedBuild.Runes = new List<Rune> { new Rune() };
                ClearRunes();
            }
            
        }

        #region Space junk

        private void IsChanged()
        {
            var serializedBuild = JsonConvert.SerializeObject(selectedBuild);

            if (!string.IsNullOrWhiteSpace(SelectedBuildName) && SelectedGameMode != GameMode.NONE)
            {
                var buildNameWithoutExtension = Path.GetFileNameWithoutExtension(SelectedBuildName);
                var isUnsaved = (!serializedBuild.Equals(referenceBuild, StringComparison.Ordinal)
                                || !buildNameWithoutExtension.Equals(FileName))
                                && !string.Equals(SelectedBuildName, CREATE_NEW);

                SaveInfo = isUnsaved ? "*Unsaved" : (SelectedBuildName.Equals(CREATE_NEW) ? "New* Unsaved" : null);
            }
        }

        private ItemImageModel RemoveDup(ItemImageModel s1, ItemImageModel s2)
        {
            if (s1 != null && s2 != null && !string.IsNullOrEmpty(s1.Text))
                return s1.Equals(s2) ? null : s2;

            return s2;
        }

        public void ClearSmallInfo()
        {
            SlDefaultConfig = null;
            SlGameMode = GameMode.NONE;
        }

        public void ClearProfile()
        {
            SelectedBuildName = null;
            referenceBuild = null;

            selectedBuild = new ChampionBuild() {
                Spells = new List<Spell>() { new Spell() },
                Runes = new List<Rune>() { new Rune() }
            };

            FileName = null;

            ClearSpells();
            ClearRunes();
        }

        private void ClearSpells()
        {
            SelectedSpell1 = null;
            SelectedSpell2 = null;
        }

        private void ClearRunes()
        {
            RuneName = null;
            SelectedPerk1 = null;
            SelectedPerk2 = null;
            SelectedPerk3 = null;
            SelectedPerk4 = null;
            SelectedPerk5 = null;
            SelectedKeystone = null;
            SelectedPath1 = null;
            SelectedPath2 = null;
            SelectedShard1 = null;
            SelectedShard2 = null;
            SelectedShard3 = null;
        }

        private async Task Saved()
        {
            SaveInfo = "Saved!";
            await Task.Delay(2500);
            SaveInfo = null;
        }
        #endregion
    }
}
