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
        public ICommand SaveCommand { get; set; }
        public ICommand SetAsDefaultCommand { get; set; }
        public ICommand ClearDefaultSourceCommand { get; set; }

        private GameMode gm()
        {
            return (GameMode)Enum.Parse(typeof(GameMode), SelectedGameMode);
        }

        private void SetAsDefaultExecute()
        {
            try
            {
                if (!SelectedBuildName.Equals(CreateNewKey))
                {
                    var gameMode = gm();
                    defaultBuildConfig.setDefaultConfig(gameMode, SelectedBuildName);
                    writeDefaultBuildConfig(SelectedChampionId, defaultBuildConfig);
                    slDefaultConfig = defaultBuildConfig.getDefaultConfig(gameMode);
                }
            }
            catch { /*ex*/ }
        }

        public void DeleteConfigExecute()
        {
            if(SelectedBuildName != null && !SelectedBuildName.Equals(CreateNewKey))
            {
                var buildName = SelectedBuildName;
                SelectedBuildName = null;
                
                var gameMode = gm();
                var _buildsName = new List<string>();
                _buildsName.Add(CreateNewKey);

                Main.localBuild.DeleteData(SelectedChampionId, Path.GetFileNameWithoutExtension(buildName), gameMode);
                foreach (var path in Main.localBuild.GetBuildFiles(SelectedChampionId, gameMode))
                    _buildsName.Add(Path.GetFileName(path));

                BuildsName = _buildsName;

                var customBuildPath = Main.localBuild.BuildsFolder(SelectedChampionId, gameMode);
                var fullPath = $"{customBuildPath}\\{defaultBuildConfig.getDefaultConfig(gameMode)}";

                if (!File.Exists(fullPath))
                    defaultBuildConfig.resetDefaultConfig(gameMode);

                writeDefaultBuildConfig(SelectedChampionId, defaultBuildConfig);
                slDefaultConfig = defaultBuildConfig.getDefaultConfig(gameMode);
            }
        }

        private void ClearDefaultSourceExecute()
        {
            if (SelectedChampion != null && SelectedGameMode != null)
            {
                var gameMode = gm();
                defaultBuildConfig.resetDefaultConfig(gameMode);
                writeDefaultBuildConfig(SelectedChampionId, defaultBuildConfig);
                slDefaultConfig = defaultBuildConfig.getDefaultConfig(gameMode);
            }
        }

        private async void SaveConfigExecute()
        {
            if(!string.IsNullOrEmpty(FileName) && SelectedChampion != null && SelectedGameMode != null)
            {
                var sGM = gm();

                var filePath = Main.localBuild.DataPath(SelectedChampionId, FileName, sGM);

                if (File.Exists(filePath))
                    File.Create(filePath).Dispose();

                using(var streaWriter = new StreamWriter(filePath))
                {
                    var json = JsonConvert.SerializeObject(SelectedBuild, Formatting.Indented);
                    streaWriter.Write(json);
                    ReferenceBuild = json;
                }

                var _buildsName = new List<string>();
                _buildsName.Add(CreateNewKey);
                foreach (var path in Main.localBuild.GetBuildFiles(SelectedChampionId, sGM))
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
        private ObservableCollection<string> _ChampionList = new ObservableCollection<string>();
        public ObservableCollection<string> ChampionList
        {
            get { return _ChampionList; }
            set
            {
                if (_ChampionList != value)
                {
                    _ChampionList = value;
                    OnPropertyChanged("ChampionList");
                }
            }
        }

        private ObservableCollection<string> _GameModes = new ObservableCollection<string>();
        public ObservableCollection<string> GameModes
        {
            get { return _GameModes; }
            set
            {
                if (_GameModes != value)
                {
                    _GameModes = value;
                    OnPropertyChanged("GameModes");
                }
            }
        }

        private string _SelectedChampion;
        public string SelectedChampion
        {
            get { return _SelectedChampion; }
            set
            {
                if (_SelectedChampion != value)
                {
                    slChampion = value;
                    if (value == null || value == string.Empty)
                        slChampion = "None";

                    _SelectedChampion = value;
                    OnPropertyChanged("SelectedChampion");
                    Data_Changed();
                }
            }
        }

        private string _SelectedGameMode;
        public string SelectedGameMode
        {
            get { return _SelectedGameMode; }
            set
            {
                if (_SelectedGameMode != value)
                {
                    slGameMode = value;
                    _SelectedGameMode = value;
                    OnPropertyChanged("SelectedGameMode");
                    Data_Changed();
                }
            }
        }

        private List<string> _BuildsName = new List<string>();
        public List<string> BuildsName
        {
            get { return _BuildsName; }
            set
            {
                if (_BuildsName != value)
                {
                    _BuildsName = value;
                    OnPropertyChanged("BuildsName");
                }
            }
        }

        private string _SelectedBuildName;
        public string SelectedBuildName
        {
            get { return _SelectedBuildName; }
            set
            {
                if (_SelectedBuildName != value)
                {
                    _SelectedBuildName = value;
                    OnPropertyChanged("SelectedBuildName");
                    LoadBuild();
                }
            }
        }
        #endregion

        #region Editable
        private string _FileName;
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnPropertyChanged("FileName");

                    IsChanged();
                }
            }
        }

        #region Spells
        public List<ItemImageModel> _SpellList = new List<ItemImageModel>();
        public List<ItemImageModel> SpellList
        {
            get { return _SpellList; }
            set
            {
                if (_SpellList != value)
                {
                    _SpellList = value;
                    OnPropertyChanged("SpellList");
                }
            }
        }

        private ItemImageModel _SelectedSpell1;
        public ItemImageModel SelectedSpell1
        {
            get { return _SelectedSpell1; }
            set    
            {
                if (_SelectedSpell1 != value)
                {
                    _SelectedSpell1 = value;
                    SelectedSpell2 = RemoveDup(SelectedSpell1, SelectedSpell2);
                    if(value != null)
                        SelectedBuild.spell.Spell0 = Dictionaries.SpellNameToSpellID[value.Text];
                    OnPropertyChanged("SelectedSpell1");

                    IsChanged();
                }
            }
        }

        private ItemImageModel _SelectedSpell2;
        public ItemImageModel SelectedSpell2
        {
            get { return _SelectedSpell2; }
            set
            {
                if (_SelectedSpell2 != value)
                {
                    _SelectedSpell2 = value;
                    SelectedSpell1 = RemoveDup(SelectedSpell2, SelectedSpell1);
                    if(value != null)
                        SelectedBuild.spell.Spell1 = Dictionaries.SpellNameToSpellID[value.Text];
                    OnPropertyChanged("SelectedSpell2");

                    IsChanged();
                }
            }
        }
        #endregion

        #region Perks
        private string _RuneName;
        public string RuneName
        {
            get { return _RuneName; }
            set
            {
                if (_RuneName != value)
                {
                    _RuneName = value;

                    if(value != null)
                        SelectedBuild.rune.Name = value;

                    OnPropertyChanged("RuneName");

                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _PathList = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> PathList
        {
            get { return _PathList; }
            set
            {
                if (_PathList != value)
                {
                    _PathList = value;
                    OnPropertyChanged("PathList");
                }
            }
        }

        private ItemImageModel _SelectedPath1;
        public ItemImageModel SelectedPath1
        {
            get { return _SelectedPath1; }
            set
            {
                if (_SelectedPath1 != value)
                {
                    _SelectedPath1 = value;
                    SelectedPath2 = RemoveDup(SelectedPath1, SelectedPath2);
                    if(value != null)
                        SelectedBuild.rune.Path0 = DataConverter.PathNameToId(value.Text);
                    OnPropertyChanged("SelectedPath1");
                    Path1_Changed();

                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _KeystoneList = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> KeystoneList
        {
            get { return _KeystoneList; }
            set
            {
                if (_KeystoneList != value)
                {
                    _KeystoneList = value;
                    OnPropertyChanged("KeystoneList");
                }
            }
        }

        private ItemImageModel _SelectedKeystone;
        public ItemImageModel SelectedKeystone
        {
            get { return _SelectedKeystone; }
            set
            {
                if (_SelectedKeystone != value)
                {
                    _SelectedKeystone = value;
                    if(value != null)
                        SelectedBuild.rune.Keystone = DataConverter.PerkNameToId(value.Text);
                    OnPropertyChanged("SelectedKeystone");

                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _Perk1List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk1List
        {
            get { return _Perk1List; }
            set
            {
                if (_Perk1List != value)
                {
                    _Perk1List = value;
                    OnPropertyChanged("Perk1List");
                }
            }
        }

        private ItemImageModel _SelectedPerk1;
        public ItemImageModel SelectedPerk1
        {
            get { return _SelectedPerk1; }
            set
            {
                if (_SelectedPerk1 != value)
                {
                    _SelectedPerk1 = value;
                    if(value!=null)
                        SelectedBuild.rune.Slot1 = DataConverter.PerkNameToId(value.Text);
                    OnPropertyChanged("SelectedPerk1");

                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _Perk2List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk2List
        {
            get { return _Perk2List; }
            set
            {
                if (_Perk2List != value)
                {
                    _Perk2List = value;
                    OnPropertyChanged("Perk2List");
                }
            }
        }

        private ItemImageModel _SelectedPerk2;
        public ItemImageModel SelectedPerk2
        {
            get { return _SelectedPerk2; }
            set
            {
                if (_SelectedPerk2 != value)
                {
                    _SelectedPerk2 = value;
                    if(value != null)
                        SelectedBuild.rune.Slot2 = DataConverter.PerkNameToId(value.Text);
                    OnPropertyChanged("SelectedPerk2");

                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _Perk3List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk3List
        {
            get { return _Perk3List; }
            set
            {
                if (_Perk3List != value)
                {
                    _Perk3List = value;
                    OnPropertyChanged("Perk3List");
                }
            }
        }

        private ItemImageModel _SelectedPerk3;
        public ItemImageModel SelectedPerk3
        {
            get { return _SelectedPerk3; }
            set
            {
                if (_SelectedPerk3 != value)
                {
                    _SelectedPerk3 = value;
                    if(value!=null)
                        SelectedBuild.rune.Slot3 = DataConverter.PerkNameToId(value.Text);
                    OnPropertyChanged("SelectedPerk3");

                    IsChanged();
                }
            }
        }

        private ItemImageModel _SelectedPath2;
        public ItemImageModel SelectedPath2
        {
            get { return _SelectedPath2; }
            set
            {
                if (_SelectedPath2 != value)
                {
                    _SelectedPath2 = value;
                    if(value != null)
                        SelectedBuild.rune.Path1 = DataConverter.PathNameToId(value.Text);
                    SelectedPath1 = RemoveDup(SelectedPath2, SelectedPath1);
                    OnPropertyChanged("SelectedPath2");
                    Path2_Changed();

                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _Perk4List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk4List
        {
            get { return _Perk4List; }
            set
            {
                if (_Perk4List != value)
                {
                    _Perk4List = value;
                    OnPropertyChanged("Perk4List");
                }
            }
        }

        private ItemImageModel _SelectedPerk4;
        public ItemImageModel SelectedPerk4
        {
            get { return _SelectedPerk4; }
            set
            {
                if (_SelectedPerk4 != value)
                {
                    _SelectedPerk4 = value;
                    if(value!=null)
                        SelectedBuild.rune.Slot4 = DataConverter.PerkNameToId(value.Text);
                    SelectedPerk5 = RemoveDup(value, SelectedPerk5);
                    OnPropertyChanged("SelectedPerk4");
                    Perk4_Changed();

                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _Perk5List = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perk5List
        {
            get { return _Perk5List; }
            set
            {
                if (_Perk5List != value)
                {
                    _Perk5List = value;
                    OnPropertyChanged("Perk5List");
                }
            }
        }

        private ItemImageModel _SelectedPerk5;
        public ItemImageModel SelectedPerk5
        {
            get { return _SelectedPerk5; }
            set
            {
                if (_SelectedPerk5 != value)
                {
                    _SelectedPerk5 = value;
                    if(value != null)
                        SelectedBuild.rune.Slot5 = DataConverter.PerkNameToId(value.Text);
                    SelectedPerk4 = RemoveDup(value, SelectedPerk4);
                    OnPropertyChanged("SelectedPerk5");

                    IsChanged();
                }
            }
        }
        #region Shards
        public ObservableCollection<ItemImageModel> _OffenseList = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> OffenseList
        {
            get { return _OffenseList; }
            set
            {
                if (_OffenseList != value)
                {
                    _OffenseList = value;
                    OnPropertyChanged("OffenseList");
                }
            }
        }

        private ItemImageModel _SelectedOffense;
        public ItemImageModel SelectedOffense
        {
            get { return _SelectedOffense; }
            set
            {
                if (_SelectedOffense != value)
                {
                    _SelectedOffense = value;
                    if (value != null)
                        SelectedBuild.rune.Shard0 = Dictionaries.ShardDescToShardId[value.Text];
                    SelectedPerk4 = RemoveDup(SelectedOffense, SelectedPerk4);
                    OnPropertyChanged("SelectedOffense");

                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _FlexList = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> FlexList
        {
            get { return _FlexList; }
            set
            {
                if (_FlexList != value)
                {
                    _FlexList = value;
                    OnPropertyChanged("FlexList");
                }
            }
        }

        private ItemImageModel _SelectedFlex;
        public ItemImageModel SelectedFlex
        {
            get { return _SelectedFlex; }
            set
            {
                if (_SelectedFlex != value)
                {
                    _SelectedFlex = value;
                    if (value != null)
                        SelectedBuild.rune.Shard1 = Dictionaries.ShardDescToShardId[value.Text];
                    SelectedPerk4 = RemoveDup(SelectedFlex, SelectedPerk4);
                    OnPropertyChanged("SelectedFlex");

                    IsChanged();
                }
            }
        }

        public ObservableCollection<ItemImageModel> _DefenseList = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> DefenseList
        {
            get { return _DefenseList; }
            set
            {
                if (_DefenseList != value)
                {
                    _DefenseList = value;
                    OnPropertyChanged("DefenseList");
                }
            }
        }

        private ItemImageModel _SelectedDefense;
        public ItemImageModel SelectedDefense
        {
            get { return _SelectedDefense; }
            set
            {
                if (_SelectedDefense != value)
                {
                    _SelectedDefense = value;
                    if (value != null)
                        SelectedBuild.rune.Shard2 = Dictionaries.ShardDescToShardId[value.Text];
                    SelectedPerk4 = RemoveDup(SelectedDefense, SelectedPerk4);
                    OnPropertyChanged("SelectedDefense");

                    IsChanged();
                }
            }
        }
        #endregion

        #endregion

        #endregion

        #region SmallInfo
        private string _WarningTxt;
        public string WarningTxt
        {
            get { return _WarningTxt; }
            set
            {
                if (_WarningTxt != value)
                {
                    _WarningTxt = value;
                    OnPropertyChanged("WarningTxt");
                }
            }
        }

        private string _slChampion = "None";
        public string slChampion
        {
            get { return _slChampion; }
            set
            {
                if (_slChampion != value)
                {
                    _slChampion = value;
                    OnPropertyChanged("slChampion");
                }
            }
        }

        private string _slGameMode = "None";
        public string slGameMode
        {
            get { return _slGameMode; }
            set
            {
                if (_slGameMode != value)
                {
                    _slGameMode = value;
                    OnPropertyChanged("slGameMode");
                }
            }
        }


        private string _slDefaultConfig = "None";
        public string slDefaultConfig
        {
            get { return _slDefaultConfig; }
            set
            {
                if (_slDefaultConfig != value)
                {
                    _slDefaultConfig = value;
                    OnPropertyChanged("slDefaultConfig");
                }
            }
        }

        private string _saveInfo;

        public string saveInfo
        {
            get { return _saveInfo; }
            set
            {
                if (_saveInfo != value)
                {
                    _saveInfo = value;
                    OnPropertyChanged("saveInfo");
                }
            }
        }

        #endregion

        private string ReferenceBuild;
        private string SelectedChampionId;
        public DefaultBuildConfig defaultBuildConfig;
        private const string CreateNewKey = "Create New*";
        private ChampionBD SelectedBuild = new ChampionBD();
        private Dictionary<string, string[,]> SecondPath = new Dictionary<string, string[,]>();
        public BuildEditorViewModel()
        {
            Init();
            PathModel.Init();

            SaveCommand = new RelayCommand(action => { SaveConfigExecute(); }, o => true);
            //DeleteCommand = new RelayCommand(action => { DeleteConfigExecute(); }, o => true);
            SetAsDefaultCommand = new RelayCommand(action => { SetAsDefaultExecute(); }, o => true);
            ClearDefaultSourceCommand = new RelayCommand(action => { ClearDefaultSourceExecute(); }, o => true);
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

                _buildsName.Add(CreateNewKey);
                SelectedChampionId = DataConverter.ChampionNameToId(SelectedChampion);

                // Load in default build config
                defaultBuildConfig = new DefaultBuildConfig();

                if (File.Exists(defConfigPath(SelectedChampionId))) defaultBuildConfig = getDefaultBuildConfig(SelectedChampionId);

                var customBuildPath = Main.localBuild.BuildsFolder(SelectedChampionId, gameMode);
                var fullPath = $"{customBuildPath}\\{defaultBuildConfig.getDefaultConfig(gameMode)}";
                
                if(!File.Exists(fullPath))
                    defaultBuildConfig.resetDefaultConfig(gameMode);

                slDefaultConfig = defaultBuildConfig.getDefaultConfig(gameMode);

                await Task.Run(() => {
                    foreach (var path in Main.localBuild.GetBuildFiles(SelectedChampionId, gameMode))
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
        }

        private async void LoadBuild()
        {
            if(!string.IsNullOrEmpty(SelectedBuildName) 
            && !SelectedBuildName.Equals(CreateNewKey))
            {
                saveInfo = null;
                await Task.Run(() => {
                    foreach (var path in Main.localBuild.GetBuildFiles(SelectedChampionId, gm()))
                    {
                        if (Path.GetFileName(path) == SelectedBuildName)
                        {
                            try
                            {
                                using (StreamReader sr = new StreamReader(path))
                                {
                                    var jsonContent = sr.ReadToEnd();
                                    FileName = Path.GetFileNameWithoutExtension(path);
                                    SelectedBuild = JsonConvert.DeserializeObject<ChampionBD>(jsonContent);
                                    ReferenceBuild = jsonContent;
                                    LoadBuild(SelectedBuild);
                                }
                            }
                            catch { }
                        }
                    }
                    saveInfo = null;
                });
            }
            else if(!string.IsNullOrEmpty(SelectedBuildName)
            && SelectedBuildName.Equals(CreateNewKey)) {
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
            KeystoneList = new ObservableCollection<ItemImageModel>();

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
                                        KeystoneList.Add(model);
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
            var _perk4list = new ObservableCollection<ItemImageModel>();

            if (SelectedPath2 != null)
            {
                secondSlot = SecondPath[SelectedPath2.Text];
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
                SelectedKeystone = KeystoneList.SingleOrDefault(k => k.Text == DataConverter.PerkIdToName(championBuild.rune.Keystone));
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
            var sBuild = JsonConvert.SerializeObject(SelectedBuild, Formatting.Indented);

            if(SelectedGameMode != null && SelectedBuildName != null)
            {
                if ((!sBuild.Equals(ReferenceBuild) ||
                !Path.GetFileNameWithoutExtension(SelectedBuildName).Equals(FileName))
                && !SelectedBuildName.Equals(CreateNewKey))
                    saveInfo = "*Unsaved";
                else if (SelectedBuildName.Equals(CreateNewKey))
                    saveInfo = "New* Unsaved";
                else saveInfo = null;
            }
        }

        private void InitSecondPath()
        {
            SecondPath.Add("Domination", PathModel.Domination);
            SecondPath.Add("Sorcery", PathModel.Sorcery);
            SecondPath.Add("Precision", PathModel.Precision);
            SecondPath.Add("Inspiration", PathModel.Inspiration);
            SecondPath.Add("Resolve", PathModel.Resolve);
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
                else return s2;
            }
            else return s2;
        }

        public ItemImageModel RemoveDup(ItemImageModel s1, ItemImageModel s2)
        {
            if (s1 != null && s2 != null && !string.IsNullOrEmpty(s1.Text))
            {
                if (s1.Text == s2.Text)
                    return null;
                else return s2;
            }
            else return s2;
        }

        public void ClearProfile()
        {
            SelectedBuildName = null;
            ReferenceBuild = null;
            SelectedBuild = new ChampionBD();
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
