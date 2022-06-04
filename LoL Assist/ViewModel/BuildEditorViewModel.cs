using LoLA.WebAPIs.DataDragon.Objects;
using System.Collections.ObjectModel;
using static LoL_Assist_WAPP.Utils;
using System.Collections.Generic;
using LoLA.WebAPIs.DataDragon;
using System.Threading.Tasks;
using System.ComponentModel;
using LoL_Assist_WAPP.Model;
using System.Windows.Input;
using System.Threading;
using LoLA.LCU.Objects;
using Newtonsoft.Json;
using LoLA.Objects;
using System.Linq;
using System.IO;
using LoLA.LCU;
using System;
using LoLA;

namespace LoL_Assist_WAPP.ViewModel
{
    public class BuildEditorViewModel : INotifyPropertyChanged
    {

        #region Commands
        public ICommand SaveCommand { get; }
        public ICommand SetAsDefaultCommand { get; }
        public ICommand ClearDefaultSourceCommand { get; }

        private GameMode gm()
        {
            return (GameMode)Enum.Parse(typeof(GameMode), SelectedGameMode);
        }

        private void SetAsDefaultExecute()
        {
            try
            {
                if (!SelectedBuildName.Equals(_CreateNewKey))
                {
                    var gameMode = gm();
                    _DefaultBuildConfig.setDefaultConfig(gameMode, SelectedBuildName);
                    writeDefaultBuildConfig(_SelectedChampionId, _DefaultBuildConfig);
                    SlDefaultConfig = _DefaultBuildConfig.getDefaultConfig(gameMode);
                }
            }
            catch { /*ex*/ }
        }

        public void DeleteConfigExecute()
        {
            if(SelectedBuildName != null && !SelectedBuildName.Equals(_CreateNewKey))
            {
                var buildName = SelectedBuildName;
                SelectedBuildName = null;
                
                var gameMode = gm();
                var _buildsName = new List<string>();
                _buildsName.Add(_CreateNewKey);

                Main.localBuild.DeleteData(_SelectedChampionId, Path.GetFileNameWithoutExtension(buildName), gameMode);
                foreach (var path in Main.localBuild.GetBuildFiles(_SelectedChampionId, gameMode))
                    _buildsName.Add(Path.GetFileName(path));

                BuildsName = _buildsName;

                var customBuildPath = Main.localBuild.BuildsFolder(_SelectedChampionId, gameMode);
                var fullPath = $"{customBuildPath}\\{_DefaultBuildConfig.getDefaultConfig(gameMode)}";

                if (!File.Exists(fullPath))
                    _DefaultBuildConfig.resetDefaultConfig(gameMode);

                writeDefaultBuildConfig(_SelectedChampionId, _DefaultBuildConfig);
                SlDefaultConfig = _DefaultBuildConfig.getDefaultConfig(gameMode);
            }
        }

        private void ClearDefaultSourceExecute()
        {
            if (SelectedChampion != null && SelectedGameMode != null)
            {
                var gameMode = gm();
                _DefaultBuildConfig.resetDefaultConfig(gameMode);
                writeDefaultBuildConfig(_SelectedChampionId, _DefaultBuildConfig);
                SlDefaultConfig = _DefaultBuildConfig.getDefaultConfig(gameMode);
            }
        }

        private async void SaveConfigExecute()
        {
            if(!string.IsNullOrEmpty(FileName) && SelectedChampion != null && SelectedGameMode != null)
            {
                var sGM = gm();
                var filePath = Main.localBuild.DataPath(_SelectedChampionId, FileName, sGM);

                if (File.Exists(filePath))
                    File.Create(filePath).Dispose();

                using(var streaWriter = new StreamWriter(filePath))
                {
                    var json = JsonConvert.SerializeObject(_SelectedBuild, Formatting.Indented);
                    streaWriter.Write(json);
                    _ReferenceBuild = json;
                }

                var _buildsName = new List<string>();
                _buildsName.Add(_CreateNewKey);
                foreach (var path in Main.localBuild.GetBuildFiles(_SelectedChampionId, sGM))
                    _buildsName.Add(Path.GetFileName(path));

                BuildsName = _buildsName;
                SelectedBuildName = Path.GetFileName(filePath);
            }
            else if(string.IsNullOrEmpty(FileName))
            {
                WarningTxt = "'file name:' field cannot be empty!";
                await Task.Delay(2500);
                WarningTxt = null;
            }
            await saved();
        }
        #endregion

        #region Needed
        private ObservableCollection<string> championList = new ObservableCollection<string>();
        public ObservableCollection<string> ChampionList
        {
            get => championList;
            set
            {
                if (championList != value)
                {
                    championList = value;
                    OnPropertyChanged(nameof(ChampionList));
                }
            }
        }

        private ObservableCollection<string> gameModes = new ObservableCollection<string>();
        public ObservableCollection<string> GameModes
        {
            get => gameModes;
            set
            {
                if (gameModes != value)
                {
                    gameModes = value;
                    OnPropertyChanged(nameof(GameModes));
                }
            }
        }

        private string selectedChampion;
        public string SelectedChampion
        {
            get => selectedChampion;
            set
            {
                if (selectedChampion != value)
                {
                    SlChampion = value;
                    selectedChampion = value;

                    if (value == null || value == string.Empty)
                        SlChampion = "None";


                    OnPropertyChanged(nameof(SelectedChampion));
                    Data_Changed();
                }
            }
        }

        private string selectedGameMode;
        public string SelectedGameMode
        {
            get => selectedGameMode;
            set
            {
                if (selectedGameMode != value)
                {
                    SlGameMode = value;
                    selectedGameMode = value;
                    OnPropertyChanged(nameof(SelectedGameMode));
                    Data_Changed();
                }
            }
        }

        private List<string> buildsName = new List<string>();
        public List<string> BuildsName
        {
            get => buildsName;
            set
            {
                if (buildsName != value)
                {
                    buildsName = value;
                    OnPropertyChanged(nameof(BuildsName));
                }
            }
        }

        private string selectedBuildName;
        public string SelectedBuildName
        {
            get => selectedBuildName;
            set
            {
                if (selectedBuildName != value)
                {
                    selectedBuildName = value;
                    OnPropertyChanged(nameof(SelectedBuildName));
                    FetchBuild();
                }
            }
        }
        #endregion

        #region Editable
        private string fileName;
        public string FileName
        {
            get => fileName;
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged(nameof(FileName));
                    IsChanged();
                }
            }
        }

        #region Spells
        public List<ItemImageModel> spellList = new List<ItemImageModel>();
        public List<ItemImageModel> SpellList
        {
            get => spellList;
            set
            {
                if (spellList != value)
                {
                    spellList = value;
                    OnPropertyChanged(nameof(SpellList));
                }
            }
        }

        private ItemImageModel selectedSpell1;
        public ItemImageModel SelectedSpell1
        {
            get => selectedSpell1;
            set    
            {
                if (selectedSpell1 != value)
                {
                    selectedSpell1 = value;
                    SelectedSpell2 = RemoveDup(SelectedSpell1, SelectedSpell2);
                    if(value != null)
                        _SelectedBuild.spell.Spell0 = Dictionaries.SpellNameToSpellID[value.Text];

                    OnPropertyChanged(nameof(SelectedSpell1));
                    IsChanged();
                }
            }
        }

        private ItemImageModel selectedSpell2;
        public ItemImageModel SelectedSpell2
        {
            get => selectedSpell2;
            set
            {
                if (selectedSpell2 != value)
                {
                    selectedSpell2 = value;
                    SelectedSpell1 = RemoveDup(SelectedSpell2, SelectedSpell1);
                    if(value != null)
                        _SelectedBuild.spell.Spell1 = Dictionaries.SpellNameToSpellID[value.Text];

                    OnPropertyChanged(nameof(SelectedSpell2));
                    IsChanged();
                }
            }
        }
        #endregion

        #region Perks
        private string runeName;
        public string RuneName
        {
            get => runeName;
            set
            {
                if (runeName != value)
                {
                    runeName = value;

                    if(value != null)
                        _SelectedBuild.rune.Name = value;

                    OnPropertyChanged(nameof(RuneName));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> pathList = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> PathList
        {
            get => pathList;
            set
            {
                if (pathList != value)
                {
                    pathList = value;
                    OnPropertyChanged(nameof(PathList));
                }
            }
        }

        private ItemImageModel selectedPath1;
        public ItemImageModel SelectedPath1
        {
            get => selectedPath1;
            set
            {
                if (selectedPath1 != value)
                {
                    selectedPath1 = value;
                    SelectedPath2 = RemoveDup(SelectedPath1, SelectedPath2);
                    if(value != null)
                        _SelectedBuild.rune.Path0 = DataConverter.PathNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedPath1));
                    Path1_Changed();
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> keystones = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Keystones
        {
            get => keystones;
            set
            {
                if (keystones != value)
                {
                    keystones = value;
                    OnPropertyChanged(nameof(Keystones));
                }
            }
        }

        private ItemImageModel selectedKeystone;
        public ItemImageModel SelectedKeystone
        {
            get => selectedKeystone;
            set
            {
                if (selectedKeystone != value)
                {
                    selectedKeystone = value;
                    if(value != null)
                        _SelectedBuild.rune.Keystone = DataConverter.PerkNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedKeystone));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> perk1List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk1List
        {
            get => perk1List;
            set
            {
                if (perk1List != value)
                {
                    perk1List = value;
                    OnPropertyChanged(nameof(Perk1List));
                }
            }
        }

        private ItemImageModel selectedPerk1;
        public ItemImageModel SelectedPerk1
        {
            get => selectedPerk1;
            set
            {
                if (selectedPerk1 != value)
                {
                    selectedPerk1 = value;
                    if(value!=null)
                        _SelectedBuild.rune.Slot1 = DataConverter.PerkNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedPerk1));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> perk2List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk2List
        {
            get => perk2List;
            set
            {
                if (perk2List != value)
                {
                    perk2List = value;
                    OnPropertyChanged(nameof(Perk2List));
                }
            }
        }

        private ItemImageModel selectedPerk2;
        public ItemImageModel SelectedPerk2
        {
            get => selectedPerk2;
            set
            {
                if (selectedPerk2 != value)
                {
                    selectedPerk2 = value;
                    if(value != null)
                        _SelectedBuild.rune.Slot2 = DataConverter.PerkNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedPerk2));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> perk3List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk3List
        {
            get => perk3List;
            set
            {
                if (perk3List != value)
                {
                    perk3List = value;
                    OnPropertyChanged(nameof(Perk3List));
                }
            }
        }

        private ItemImageModel selectedPerk3;
        public ItemImageModel SelectedPerk3
        {
            get => selectedPerk3;
            set
            {
                if (selectedPerk3 != value)
                {
                    selectedPerk3 = value;
                    if(value!=null)
                        _SelectedBuild.rune.Slot3 = DataConverter.PerkNameToId(value.Text);

                    OnPropertyChanged(nameof(SelectedPerk3));
                    IsChanged();
                }
            }
        }

        private ItemImageModel selectedPath2;
        public ItemImageModel SelectedPath2
        {
            get => selectedPath2;
            set
            {
                if (selectedPath2 != value)
                {
                    selectedPath2 = value;
                    if(value != null)
                        _SelectedBuild.rune.Path1 = DataConverter.PathNameToId(value.Text);
                    SelectedPath1 = RemoveDup(SelectedPath2, SelectedPath1);

                    OnPropertyChanged(nameof(SelectedPath2));
                    Path2_Changed();
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> perk4List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk4List
        {
            get => perk4List;
            set
            {
                if (perk4List != value)
                {
                    perk4List = value;
                    OnPropertyChanged(nameof(Perk4List));
                }
            }
        }

        private ItemImageModel selectedPerk4;
        public ItemImageModel SelectedPerk4
        {
            get => selectedPerk4;
            set
            {
                if (selectedPerk4 != value)
                {
                    selectedPerk4 = value;
                    if(value!=null)
                        _SelectedBuild.rune.Slot4 = DataConverter.PerkNameToId(value.Text);
                    SelectedPerk5 = RemoveDup(value, SelectedPerk5);

                    OnPropertyChanged(nameof(SelectedPerk4));
                    Perk4_Changed();
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> perk5List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk5List
        {
            get => perk5List;
            set
            {
                if (perk5List != value)
                {
                    perk5List = value;
                    OnPropertyChanged(nameof(Perk5List));
                }
            }
        }

        private ItemImageModel selectedPerk5;
        public ItemImageModel SelectedPerk5
        {
            get => selectedPerk5;
            set
            {
                if (selectedPerk5 != value)
                {
                    selectedPerk5 = value;
                    if(value != null)
                        _SelectedBuild.rune.Slot5 = DataConverter.PerkNameToId(value.Text);
                    SelectedPerk4 = RemoveDup(value, SelectedPerk4);

                    OnPropertyChanged(nameof(SelectedPerk5));
                    IsChanged();
                }
            }
        }
        #region Shards
        public ObservableCollection<ItemImageModel> offenseList = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> OffenseList
        {
            get => offenseList;
            set
            {
                if (offenseList != value)
                {
                    offenseList = value;
                    OnPropertyChanged(nameof(OffenseList));
                }
            }
        }

        private ItemImageModel selectedOffense;
        public ItemImageModel SelectedOffense
        {
            get => selectedOffense;
            set
            {
                if (selectedOffense != value)
                {
                    selectedOffense = value;
                    if (value != null)
                        _SelectedBuild.rune.Shard0 = Dictionaries.ShardDescToShardId[value.Text];
                    SelectedPerk4 = RemoveDup(SelectedOffense, SelectedPerk4);

                    OnPropertyChanged(nameof(SelectedOffense));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> flexList = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> FlexList
        {
            get => flexList;
            set
            {
                if (flexList != value)
                {
                    flexList = value;
                    OnPropertyChanged(nameof(FlexList));
                }
            }
        }

        private ItemImageModel selectedFlex;
        public ItemImageModel SelectedFlex
        {
            get => selectedFlex;
            set
            {
                if (selectedFlex != value)
                {
                    selectedFlex = value;
                    if (value != null)
                        _SelectedBuild.rune.Shard1 = Dictionaries.ShardDescToShardId[value.Text];
                    SelectedPerk4 = RemoveDup(SelectedFlex, SelectedPerk4);

                    OnPropertyChanged(nameof(SelectedFlex));
                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> defenseList = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> DefenseList
        {
            get => defenseList;
            set
            {
                if (defenseList != value)
                {
                    defenseList = value;
                    OnPropertyChanged(nameof(DefenseList));
                }
            }
        }

        private ItemImageModel selectedDefense;
        public ItemImageModel SelectedDefense
        {
            get => selectedDefense;
            set
            {
                if (selectedDefense != value)
                {
                    selectedDefense = value;
                    if (value != null)
                        _SelectedBuild.rune.Shard2 = Dictionaries.ShardDescToShardId[value.Text];
                    SelectedPerk4 = RemoveDup(SelectedDefense, SelectedPerk4);

                    OnPropertyChanged(nameof(SelectedDefense));
                    IsChanged();
                }
            }
        }
        #endregion

        #endregion

        #endregion

        #region SmallInfo
        private string warningTxt;
        public string WarningTxt
        {
            get => warningTxt;
            set
            {
                if (warningTxt != value)
                {
                    warningTxt = value;
                    OnPropertyChanged(nameof(WarningTxt));
                }
            }
        }

        private string slChampion = "None";
        public string SlChampion
        {
            get => slChampion;
            set
            {
                if (slChampion != value)
                {
                    if (string.IsNullOrEmpty(value))
                        slChampion = "None";
                    else
                        slChampion = value;
                    OnPropertyChanged(nameof(SlChampion));
                }
            }
        }

        private string slGameMode = "None";
        public string SlGameMode
        {
            get => slGameMode;
            set
            {
                if (slGameMode != value)
                {
                    if (string.IsNullOrEmpty(value))
                        slGameMode = "None";
                    else
                        slGameMode = value;
                    OnPropertyChanged(nameof(SlGameMode));
                }
            }
        }


        private string slDefaultConfig = "None";
        public string SlDefaultConfig
        {
            get => slDefaultConfig;
            set
            {
                if (slDefaultConfig != value)
                {
                    if (string.IsNullOrEmpty(value))
                        slDefaultConfig = "None";
                    else
                        slDefaultConfig = value;
                    OnPropertyChanged(nameof(SlDefaultConfig));
                }
            }
        }

        private string saveInfo;

        public string SaveInfo
        {
            get => saveInfo;
            set
            {
                if (saveInfo != value)
                {
                    saveInfo = value;
                    OnPropertyChanged(nameof(SaveInfo));
                }
            }
        }

        #endregion

        private string _ReferenceBuild;
        private string _SelectedChampionId;
        public DefaultBuildConfig _DefaultBuildConfig;
        private const string _CreateNewKey = "Create New*";
        private ChampionBD _SelectedBuild = new ChampionBD();
        private Dictionary<string, string[,]> _SecondPath = new Dictionary<string, string[,]>();
        public BuildEditorViewModel()
        {
            Init();
            PathModel.Init();

            SaveCommand = new Command(o => { SaveConfigExecute(); });
            SetAsDefaultCommand = new Command(o => { SetAsDefaultExecute(); });
            ClearDefaultSourceCommand = new Command(o => { ClearDefaultSourceExecute(); });
        }

        public async void Init()
        {
            var _pathlist = new ObservableCollection<ItemImageModel>();

            await Task.Run(() => {
                Dictionary<string, Data> data = null;
                while (data == null)
                {
                    data = DataDragonWrapper.champions?.Data;
                    Thread.Sleep(1000);
                }
                data = null;
            });

            foreach (var champion in DataDragonWrapper.champions.Data.Values)
                ChampionList.Add(champion.name);

            GameModes = new ObservableCollection<string>(Enum.GetValues(typeof(GameMode))
            .Cast<GameMode>().Select(v => v.ToString()).Where(l => l != "NONE"));

            foreach (var perk in DataDragonWrapper.perks)
                _pathlist.Add(ItemImage(perk.key, ImageSrc(perk.key)));

            PathList = _pathlist;
            InitSecondPath();
            InitShards();
        }

        private async void Data_Changed()
        {
            var _buildsName = new List<string>();
            var _spells = new List<ItemImageModel>();

            ClearProfile();

            if (!string.IsNullOrEmpty(SelectedChampion)
            && !string.IsNullOrEmpty(SelectedGameMode))
            {
                saveInfo = null;
                var gameMode = gm();

                _buildsName.Add(_CreateNewKey);
                _SelectedChampionId = DataConverter.ChampionNameToId(SelectedChampion);

                // Load in default build config
                _DefaultBuildConfig = new DefaultBuildConfig();

                if (File.Exists(defConfigPath(_SelectedChampionId))) _DefaultBuildConfig = getDefaultBuildConfig(_SelectedChampionId);

                var customBuildPath = Main.localBuild.BuildsFolder(_SelectedChampionId, gameMode);
                var fullPath = $"{customBuildPath}\\{_DefaultBuildConfig.getDefaultConfig(gameMode)}";
                
                if(!File.Exists(fullPath))
                    _DefaultBuildConfig.resetDefaultConfig(gameMode);

                SlDefaultConfig = _DefaultBuildConfig.getDefaultConfig(gameMode);

                await Task.Run(() => {
                    foreach (var path in Main.localBuild.GetBuildFiles(_SelectedChampionId, gameMode))
                        _buildsName.Add(Path.GetFileName(path));

                    foreach (var spell in Dictionaries.SpellNameToSpellKey)
                    {
                        if (gameMode != GameMode.ARAM && spell.Key.Equals("Mark"))
                            continue;
                        else if (gameMode == GameMode.ARAM && spell.Key.Equals("Smite")
                        || gameMode == GameMode.ARAM && spell.Key.Equals("Teleport"))
                            continue;

                        _spells.Add(ItemImage(spell.Key, ImageSrc(Dictionaries.SpellNameToSpellID[spell.Key])));
                    }
                    SpellList = _spells;
                    BuildsName = _buildsName;
                });
            }
            else
            {
                SelectedGameMode = null;
                _SelectedBuild = null;
                BuildsName = null;
                saveInfo = null;
                ClearSmallInfo();
            }
        }

        public async void FetchBuild()
        {
            if(!string.IsNullOrEmpty(SelectedBuildName) 
            && !SelectedBuildName.Equals(_CreateNewKey))
            {
                saveInfo = null;
                await Task.Run(() => {
                    foreach (var path in Main.localBuild.GetBuildFiles(_SelectedChampionId, gm()))
                    {
                        if (Path.GetFileName(path) == SelectedBuildName)
                        {
                            try
                            {
                                using (StreamReader sr = new StreamReader(path))
                                {
                                    var jsonContent = sr.ReadToEnd();
                                    FileName = Path.GetFileNameWithoutExtension(path);
                                    _SelectedBuild = JsonConvert.DeserializeObject<ChampionBD>(jsonContent);
                                    _ReferenceBuild = jsonContent;
                                    LoadBuild(_SelectedBuild);
                                }
                            }
                            catch { }
                        }
                    }
                    saveInfo = null;
                });
            }
            else if(!string.IsNullOrEmpty(SelectedBuildName)
            && SelectedBuildName.Equals(_CreateNewKey)) {
                ClearProfile();
                saveInfo = "New* Unsaved";
            }
            else ClearProfile();
        }

        private void Path1_Changed() 
        {
            Perk1List = new ObservableCollection<ItemImageModel>();
            Perk2List = new ObservableCollection<ItemImageModel>();
            Perk3List = new ObservableCollection<ItemImageModel>();
            Keystones = new ObservableCollection<ItemImageModel>();

            if (SelectedPath1 != null)
            {
                int i = 0;
                foreach (var perk in DataDragonWrapper.perks)
                {
                    if (perk.name == SelectedPath1.Text)
                    {
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
                                        Perk1List.Add(model);
                                        break;
                                    case 2:
                                        Perk2List.Add(model);
                                        break;
                                    case 3:
                                        Perk3List.Add(model);
                                        break;
                                }
                            }
                            i++;
                        }
                    }
                }
            }
        }

        private string[,] secondSlot = null;
        private void Path2_Changed()
        {
            Perk5List = new ObservableCollection<ItemImageModel>();
            Perk4List = new ObservableCollection<ItemImageModel>();
            var _perk4list = new ObservableCollection<ItemImageModel>();

            if (SelectedPath2 != null)
            {
                secondSlot = _SecondPath[SelectedPath2.Text];
                foreach (var perk in secondSlot)
                {
                    if(!string.IsNullOrEmpty(perk))
                    {
                        var model = new ItemImageModel()
                        {
                            Text = perk,
                            Image = ImageSrc(perk.Replace(":", string.Empty))
                        };
                        _perk4list.Add(model);
                    }
                }
                Perk4List = _perk4list;
            }
        }

        private void Perk4_Changed()
        {
            var _perk5list = new ObservableCollection<ItemImageModel>();
            if(SelectedPerk4 != null && !string.IsNullOrEmpty(SelectedPerk4.Text))
            {
                bool isFound = false;
                for (int r = 0; r < secondSlot.GetLength(0); r++)
                {
                    if (isFound) continue;
                    for (int c = 0; c < secondSlot.GetLength(1); c++)
                    {
                        if (isFound) continue;
                        if(SelectedPerk4.Text == secondSlot[r, c])
                        {
                            isFound = true;
                            for (int r2 = 0; r2 < secondSlot.GetLength(0); r2++)
                            {
                                if (r != r2)
                                {
                                    for (int c2 = 0; c2 < secondSlot.GetLength(1); c2++)
                                    {
                                        if (!string.IsNullOrEmpty(secondSlot[r2, c2]))
                                        {
                                            var model = new ItemImageModel()
                                            {
                                                Text = secondSlot[r2, c2],
                                                Image = ImageSrc(secondSlot[r2, c2].Replace(":", string.Empty))
                                            };
                                            _perk5list.Add(model);
                                        }
                                    }
                                }
                            }
                            Perk5List = _perk5list;
                        }
                    }
                }
            }
        }

        private void LoadBuild(ChampionBD championBuild)
        {
            try 
            {
                SelectedSpell1 = SpellList.SingleOrDefault(s => s.Text == Dictionaries.SpellIDToSpellName[championBuild.spell.Spell0]);
                SelectedSpell2 = SpellList.SingleOrDefault(s => s.Text == Dictionaries.SpellIDToSpellName[championBuild.spell.Spell1]);
            } catch { }

            try
            {
                RuneName = championBuild.rune.Name;
                SelectedPath1 = PathList.SingleOrDefault(p => p.Text == DataConverter.PathIdToName(championBuild.rune.Path0));
                SelectedKeystone = Keystones.SingleOrDefault(k => k.Text == DataConverter.PerkIdToName(championBuild.rune.Keystone));
                SelectedPerk1 = Perk1List.SingleOrDefault(p => p.Text == DataConverter.PerkIdToName(championBuild.rune.Slot1));
                SelectedPerk2 = Perk2List.SingleOrDefault(p => p.Text == DataConverter.PerkIdToName(championBuild.rune.Slot2));
                SelectedPerk3 = Perk3List.SingleOrDefault(p => p.Text == DataConverter.PerkIdToName(championBuild.rune.Slot3));

                SelectedPath2 = PathList.SingleOrDefault(p => p.Text == DataConverter.PathIdToName(championBuild.rune.Path1));
                SelectedPerk4 = Perk4List.SingleOrDefault(p => p.Text == DataConverter.PerkIdToName(championBuild.rune.Slot4));
                SelectedPerk5 = Perk5List.SingleOrDefault(p => p.Text == DataConverter.PerkIdToName(championBuild.rune.Slot5));
            }
            catch { }

            try
            {
                SelectedOffense = OffenseList.SingleOrDefault(o => o.Text == Dictionaries.ShardIdToShardDescription[championBuild.rune.Shard0]);
                SelectedFlex = FlexList.SingleOrDefault(f => f.Text == Dictionaries.ShardIdToShardDescription[championBuild.rune.Shard1]);
                SelectedDefense = DefenseList.SingleOrDefault(d => d.Text == Dictionaries.ShardIdToShardDescription[championBuild.rune.Shard2]);
            }
            catch { }
        }

        #region Space junk

        private void IsChanged()
        {
            var sBuild = JsonConvert.SerializeObject(_SelectedBuild, Formatting.Indented);

            if(SelectedGameMode != null && SelectedBuildName != null)
            {
                if ((!sBuild.Equals(_ReferenceBuild) ||
                !Path.GetFileNameWithoutExtension(SelectedBuildName).Equals(FileName))
                && !SelectedBuildName.Equals(_CreateNewKey))
                    saveInfo = "*Unsaved";
                else if (SelectedBuildName.Equals(_CreateNewKey))
                    saveInfo = "New* Unsaved";
                else saveInfo = null;
            }
        }

        private void InitSecondPath()
        {
            _SecondPath.Add("Domination", PathModel.Domination);
            _SecondPath.Add("Sorcery", PathModel.Sorcery);
            _SecondPath.Add("Precision", PathModel.Precision);
            _SecondPath.Add("Inspiration", PathModel.Inspiration);
            _SecondPath.Add("Resolve", PathModel.Resolve);
        }

        private void InitShards()
        {
            var format = "webp";
            OffenseList.Add(ItemImage("5.4 bonus Attack Damage or 9 Ability Power (Adaptive)", ImageSrc("diamond", format)));
            OffenseList.Add(ItemImage("10% bonus attack speed", ImageSrc("axe", format)));
            OffenseList.Add(ItemImage("8 ability haste", ImageSrc("time", format)));

            FlexList.Add(ItemImage("5.4 bonus Attack Damage or 9 Ability Power (Adaptive)", ImageSrc("diamond", format)));
            FlexList.Add(ItemImage("6 bonus armor", ImageSrc("shield", format)));
            FlexList.Add(ItemImage("8 bonus magic resistance", ImageSrc("circle", format)));

            DefenseList.Add(ItemImage("15 − 90 (based on level) bonus health", ImageSrc("heart", format)));
            DefenseList.Add(ItemImage("6 bonus armor", ImageSrc("shield", format)));
            DefenseList.Add(ItemImage("8 bonus magic resistance", ImageSrc("circle", format)));
        }

        private ItemImageModel ItemImage(string text, string image)
        {
            var model = new ItemImageModel()
            {
                Text = text,
                Image = image
            };
            return model;
        }

        public string RemoveDup(string s1, string s2)
        {
            if (!string.IsNullOrEmpty(s1))
            {
                if (s1 == s2)
                    return null;
                return s2;
            }
            else return s2;
        }

        public ItemImageModel RemoveDup(ItemImageModel s1, ItemImageModel s2)
        {
            if (s1 != null && s2 != null && !string.IsNullOrEmpty(s1.Text))
            {
                if (s1.Text == s2.Text)
                    return null;
                return s2;
            }
            else return s2;
        }

        public void ClearSmallInfo()
        {
            SlDefaultConfig = null;
            SlGameMode = null;
            SlChampion = null;
        }

        public void ClearProfile()
        {
            SelectedBuildName = null;
            _ReferenceBuild = null;
            _SelectedBuild = new ChampionBD();
            FileName = null;
            RuneName = null;
            SelectedSpell1 = null;
            SelectedSpell2 = null;
            SelectedPerk1 = null;
            SelectedPerk2 = null;
            SelectedPerk3 = null;
            SelectedPerk4 = null;   
            SelectedPerk5 = null;
            SelectedKeystone = null;    
            SelectedPath1 = null;
            SelectedPath2 = null;
            SelectedOffense = null;
            SelectedFlex = null;
            SelectedDefense = null;
        }

        private async Task saved()
        {
            saveInfo = "Saved!";
            await Task.Delay(2500);
            saveInfo = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
