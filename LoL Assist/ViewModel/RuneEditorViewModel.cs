using LoLA.Networking.WebWrapper.DataDragon.Data;
using LoLA.Networking.WebWrapper.DataDragon;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using LoL_Assist_WAPP.Utils;
using System.ComponentModel;
using System.Windows.Input;
using LoLA.Networking.LCU;
using System.Threading;
using LoLA.Data;
using System;
using LoLA;
using LoLA.Networking.LCU.Objects;

namespace LoL_Assist_WAPP.ViewModel
{
    public class RuneEditorViewModel : INotifyPropertyChanged
    {
        #region Command
        public ICommand OpenBuildEditorCommand { get; set; }
        public ICommand SaveRunePageCommand { get; set; }
        public ICommand ReloadRunePagesCommand { get; set; }

        private ICommand _closeRuneEditorCommand;
        public ICommand CloseRuneEditorCommand
        {
            get
            {
                if (_closeRuneEditorCommand == null)
                {
                    _closeRuneEditorCommand = new Command(usercontrol => closeRuneEditor(usercontrol));
                }
                return _closeRuneEditorCommand;
            }
        }

        private ICommand _primaryPathSelectCommand;
        public ICommand PrimaryPathSelectCommand
        {
            get
            {
                if (_primaryPathSelectCommand == null)
                {
                    _primaryPathSelectCommand = new Command(path => updatePrimaryPath(path.ToString()));
                }
                return _primaryPathSelectCommand;
            }
        }

        private ICommand _keystoneSelectCommand;
        public ICommand KeystoneSelectCommand
        {
            get
            {
                if (_keystoneSelectCommand == null)
                {
                    _keystoneSelectCommand = new Command(keystone => updateKeystone(keystone?.ToString()));
                }
                return _keystoneSelectCommand;
            }
        }

        private ICommand _perk1SelectCommand;
        public ICommand Perk1SelectCommand
        {
            get
            {
                if (_perk1SelectCommand == null)
                {
                    _perk1SelectCommand = new Command(perk => updatePrimaryPathSlot(perk?.ToString(), 1));
                }
                return _perk1SelectCommand;
            }
        }

        private ICommand _perk2SelectCommand;
        public ICommand Perk2SelectCommand
        {
            get
            {
                if (_perk2SelectCommand == null)
                {
                    _perk2SelectCommand = new Command(perk => updatePrimaryPathSlot(perk?.ToString(), 2));
                }
                return _perk2SelectCommand;
            }
        }

        private ICommand _perk3SelectCommand;
        public ICommand Perk3SelectCommand
        {
            get
            {
                if (_perk3SelectCommand == null)
                {
                    _perk3SelectCommand = new Command(perk => updatePrimaryPathSlot(perk?.ToString(), 3));
                }
                return _perk3SelectCommand;
            }
        }

        private ICommand _secondaryPathSelectCommand;
        public ICommand SecondaryPathSelectCommand
        {
            get
            {
                if (_secondaryPathSelectCommand == null)
                {
                    _secondaryPathSelectCommand = new Command(path => selectSecondaryPath(path.ToString()));
                }
                return _secondaryPathSelectCommand;
            }
        }

        private ICommand _perk4SelectCommand;
        public ICommand Perk4SelectCommand
        {
            get
            {
                if (_perk4SelectCommand == null)
                {
                    _perk4SelectCommand = new Command(perk => selectSecondPathSlot(perk?.ToString(), 0));
                }
                return _perk4SelectCommand;
            }
        }

        private ICommand _perk5SelectCommand;
        public ICommand Perk5SelectCommand
        {
            get
            {
                if (_perk5SelectCommand == null)
                {
                    _perk5SelectCommand = new Command(perk => selectSecondPathSlot(perk?.ToString(), 1));
                }
                return _perk5SelectCommand;
            }
        }

        private ICommand _perk6SelectCommand;
        public ICommand Perk6SelectCommand
        {
            get
            {
                if (_perk6SelectCommand == null)
                {
                    _perk6SelectCommand = new Command(perk => selectSecondPathSlot(perk?.ToString(), 2));
                }
                return _perk6SelectCommand;
            }
        }

        private ICommand _shard1SelectCommand;
        public ICommand Shard1SelectCommand
        {
            get
            {
                if (_shard1SelectCommand == null)
                {
                    _shard1SelectCommand = new Command(shard => updateShard(shard?.ToString(), 1));
                }
                return _shard1SelectCommand;
            }
        }

        private ICommand _shard2SelectCommand;
        public ICommand Shard2SelectCommand
        {
            get
            {
                if (_shard2SelectCommand == null)
                {
                    _shard2SelectCommand = new Command(shard => updateShard(shard?.ToString(), 2));
                }
                return _shard2SelectCommand;
            }
        }

        private ICommand _shard3SelectCommand;
        public ICommand Shard3SelectCommand
        {
            get
            {
                if (_shard3SelectCommand == null)
                {
                    _shard3SelectCommand = new Command(shard => updateShard(shard?.ToString(), 3));
                }
                return _shard3SelectCommand;
            }
        }
        #endregion

        #region Primary

        private ObservableCollection<ItemImageModel> _primaryPaths = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> PrimaryPaths
        {
            get => _primaryPaths;
            set
            {
                if (_primaryPaths != value)
                {
                    _primaryPaths = value;
                    OnPropertyChanged(nameof(PrimaryPaths));
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
        #endregion

        #region Secondary
        private ObservableCollection<ItemImageModel> _secondaryPaths;
        public ObservableCollection<ItemImageModel> SecondaryPaths
        {
            get => _secondaryPaths;
            set
            {
                if (_secondaryPaths != value)
                {
                    _secondaryPaths = value;
                    OnPropertyChanged(nameof(SecondaryPaths));
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

        public ObservableCollection<ItemImageModel> _perks6 = new ObservableCollection<ItemImageModel>();
        public ObservableCollection<ItemImageModel> Perks6
        {
            get => _perks6;
            set
            {
                if (_perks6 != value)
                {
                    _perks6 = value;
                    OnPropertyChanged(nameof(Perks6));
                }
            }
        }
        #endregion

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

        public ObservableCollection<string> _runePages = new ObservableCollection<string>();
        public ObservableCollection<string> RunePages
        {
            get => _runePages;
            set
            {
                if (_runePages != value)
                {
                    _runePages = value;
                    OnPropertyChanged(nameof(RunePages));
                }
            }
        }

        public string _selectedRunePageName;
        public string SelectedRunePageName
        {
            get => _selectedRunePageName;
            set
            {
                if (_selectedRunePageName != value)
                {
                    _selectedRunePageName = value;

                    if(!string.IsNullOrEmpty(value))
                    {
                        RunePageName = value;
                        setSelectedRunePageByName(value);
                    }

                    OnPropertyChanged(nameof(SelectedRunePageName));
                }
            }
        }

        public string _runePageName;
        public string RunePageName
        {
            get => _runePageName;
            set
            {
                if (_runePageName != value)
                {
                    _runePageName = value;

                    if (!string.IsNullOrEmpty(value))
                        rune.Name = value;

                    OnPropertyChanged(nameof(RunePageName));
                }
            }
        }

        #region currentPerks
        private string currentPrimaryPath { get; set; }
        private string currentKeystone { get; set; }
        private string currentPerk1 { get; set; }
        private string currentPerk2 { get; set; }
        private string currentPerk3 { get; set; }
        private string currentSecondaryPath { get; set; }
        private string currentPerk4 { get; set; }
        private string currentPerk5 { get; set; }
        private string currentPerk6 { get; set; }

        private string currentShard1 { get; set; }
        private string currentShard2 { get; set; }
        private string currentShard3 { get; set; }
        #endregion
    
        private Rune rune { get; set; }
        private readonly List<RunePage> r_runePages = new List<RunePage>();
        public RuneEditorViewModel()
        {
            OpenBuildEditorCommand = new Command(o => Helper.ShowBuildEditorWindow());
            ReloadRunePagesCommand = new Command(o => loadRunePages());
            SaveRunePageCommand = new Command(o => saveRunePage());

            PrimaryPaths = new ObservableCollection<ItemImageModel>();
            SecondaryPaths = new ObservableCollection<ItemImageModel>();
            Keystones = new ObservableCollection<ItemImageModel>();

            Perks1 = new ObservableCollection<ItemImageModel>();
            Perks2 = new ObservableCollection<ItemImageModel>();
            Perks3 = new ObservableCollection<ItemImageModel>();
            Perks4 = new ObservableCollection<ItemImageModel>();
            Perks5 = new ObservableCollection<ItemImageModel>();
            Perks6 = new ObservableCollection<ItemImageModel>();

            rune = new Rune();

            loadRunePages();
        }

        private bool isSaving = false;
        private async void saveRunePage()
        {
            if (!isSaving)
            {
                isSaving = true;
                if (!string.IsNullOrEmpty(SelectedRunePageName))
                {
                    var selectedRunePage = r_runePages.Find(runePage => runePage.name == SelectedRunePageName);

                    if(selectedRunePage != null)
                    {
                        SelectedRunePageName = RunePageName;

                        var runePage = Converter.RuneToRunePage(rune);

                        if (await LCUWrapper.DeleteRunePageAsync(selectedRunePage.id))
                        {
                            runePage.order = selectedRunePage.order;
                            runePage.current = selectedRunePage.current;
                            runePage.isActive = selectedRunePage.isActive;

                            if (await LCUWrapper.AddRunePageAsync(runePage))
                                loadRunePages(); // reload pages
                        }
                    }
                }
                isSaving = false;
            }
        }

        private async void loadRunePages()
        {
            var runePageNameTemp = SelectedRunePageName;
            SelectedRunePageName = null;

            var runePages = await LCUWrapper.GetRunePagesAsync();

            clearCurrentRunes();

            RunePages.Clear();
            r_runePages.Clear();

            if(runePages != null && runePages.Count > 0)
            {
                runePages.ForEach((runePage) => {
                    if (runePage.isEditable && runePage.isEditable)
                    {
                        RunePages.Add(runePage.name);
                        r_runePages.Add(runePage);
                    }
                });

                SelectedRunePageName = string.IsNullOrEmpty(runePageNameTemp)
                ? RunePages[0] : SelectedRunePageName = runePageNameTemp;
            }
        }

        private void closeRuneEditor(object usercontrol)
        {
            var control = usercontrol as UserControl;
            Animation.FadeOut(control);
            //rune = null;
        }

        private void setSelectedRunePageByName(string runePageName)
        {
            var selectedRunePage = r_runePages.Find(runePage => runePage.name == runePageName);

            if (selectedRunePage != null)
                setCurrentRunePage(selectedRunePage);
        }

        private void setCurrentRunePage(RunePage runePage)
        {
            rune = Converter.RunePageToRune(runePage);
            updatePrimaryPath(Converter.PathIdToName(rune.PrimaryPath));
            updateKeystone(Converter.PerkIdToName(rune.Keystone));
            updatePrimaryPathSlot(Converter.PerkIdToName(rune.Slot1), 1);
            updatePrimaryPathSlot(Converter.PerkIdToName(rune.Slot2), 2);
            updatePrimaryPathSlot(Converter.PerkIdToName(rune.Slot3), 3);
            selectSecondaryPath(Converter.PathIdToName(rune.SecondaryPath));

            updateSecondaryPathSlots(Converter.PerkIdToName(rune.Slot4), Converter.PerkIdToName(rune.Slot5));
            updateShard(DataConverter.ShardIdToShardDescription(rune.Shard1), 1);
            updateShard(DataConverter.ShardIdToShardDescription(rune.Shard2), 2);
            updateShard(DataConverter.ShardIdToShardDescription(rune.Shard3), 3);
        }

        private void updatePrimaryPath(string selectedPath = "")
        {
            if (currentPrimaryPath != selectedPath)
            {
                PrimaryPaths.Clear();
                SecondaryPaths.Clear();

                DataDragonWrapper.s_Perks.ForEach((perk) => {
                    var itemModel = new ItemImageModel {
                        Text = perk.name,
                        Image = selectedPath.Equals(perk.name) ?
                        Helper.ImageSrc(perk.name) : Helper.ImageSrc($"g_{perk.name}")
                    };

                    PrimaryPaths.Add(itemModel);
                });

                currentPrimaryPath = selectedPath;
                updatePrimaryPathSlots(currentPrimaryPath);

                foreach (var path in PrimaryPaths)
                {
                    var pathTemp = new ItemImageModel() { Image = path.Image, Text = path.Text };
                    if (!selectedPath.Equals(pathTemp.Text))
                    {
                        pathTemp.Image = currentPrimaryPath != currentSecondaryPath
                        && currentSecondaryPath == pathTemp.Text ? Helper.ImageSrc(pathTemp.Text) : Helper.ImageSrc($"g_{pathTemp.Text}");

                        SecondaryPaths.Add(pathTemp);
                    }
                }

                if (currentPrimaryPath == currentSecondaryPath)
                {
                    //lastSlotIndex = 0;
                    //lastSelectedSlot[0] = -1;
                    //lastSelectedSlot[1] = -1;

                    //currentSecondaryPath = null;
                    //currentPerk4 = null;
                    //currentPerk5 = null;
                    //currentPerk6 = null;

                    currentSecondaryPath = null;
                    currentPerk4 = null;
                    currentPerk5 = null;
                    currentPerk6 = null;

                    //rune.SecondaryPath = 0;
                    //rune.Slot4 = 0;
                    //rune.Slot5 = 0;

                    Perks4.Clear();
                    Perks5.Clear();
                    Perks6.Clear();
                }

                if (!string.IsNullOrEmpty(currentPrimaryPath))
                    rune.PrimaryPath = Converter.PathNameToId(currentPrimaryPath);
            }
        }

        private string[,] secondSlot = null;
        private void selectSecondaryPath(string selectedSecondaryPath)
        {
            var newSecondaryPaths = itemImageModels(currentSecondaryPath, selectedSecondaryPath, SecondaryPaths);

            if (newSecondaryPaths != null)
            {
                currentSecondaryPath = selectedSecondaryPath;

                rune.SecondaryPath = Converter.PathNameToId(selectedSecondaryPath);

                SecondaryPaths = newSecondaryPaths;

                currentPerk4 = null;
                currentPerk5 = null;
                currentPerk6 = null;

                Perks4 = populateSecondaryPathSlot(currentSecondaryPath, null, 0);
                Perks5 = populateSecondaryPathSlot(currentSecondaryPath, null, 1);
                Perks6 = populateSecondaryPathSlot(currentSecondaryPath, null, 2);
            }
        }

        private void updateKeystone(string selectedKeystone)
        {
            var newKeystones = itemImageModels(currentKeystone, selectedKeystone, Keystones);

            if (newKeystones != null)
            {
                currentKeystone = selectedKeystone;

                rune.Keystone = Converter.PerkNameToId(currentKeystone);

                Keystones = newKeystones;
            }
        }

        private void updatePrimaryPathSlot(string selectedPerk, int slot)
        {
            ObservableCollection<ItemImageModel> newPerks;
            switch (slot)
            {
                case 1:
                    newPerks = itemImageModels(currentPerk1, selectedPerk, Perks1);

                    if(newPerks != null)
                    {
                        currentPerk1 = selectedPerk;

                        rune.Slot1 = Converter.PerkNameToId(currentPerk1);

                        Perks1 = newPerks;
                    }
                    break;
                case 2:
                    newPerks = itemImageModels(currentPerk2, selectedPerk, Perks2);

                    if (newPerks != null)
                    {
                        currentPerk2 = selectedPerk;

                        rune.Slot2 = Converter.PerkNameToId(currentPerk2);

                        Perks2 = newPerks;
                    }
                    break;
                case 3:
                    newPerks = itemImageModels(currentPerk3, selectedPerk, Perks3);

                    if (newPerks != null)
                    {
                        currentPerk3 = selectedPerk;

                        rune.Slot3 = Converter.PerkNameToId(currentPerk3);

                        Perks3 = newPerks;
                    }
                    break;
            }
        }

        private ObservableCollection<ItemImageModel> populateSecondaryPathSlot(string secondaryPath, string selectedPerk, int index)
        {
            secondSlot = RuneModel.r_SecondPath[secondaryPath];
            var newPerks = new ObservableCollection<ItemImageModel>();
            for (int i = 0; i < secondSlot.GetLength(1); i++)
            {
                if (secondSlot[index, i] == null)
                    continue;

                var image = selectedPerk != null && selectedPerk.Equals(secondSlot[index, i]) ?
                Helper.ImageSrc(secondSlot[index, i]) : Helper.ImageSrc($"g_{secondSlot[index, i]}");

                var itemImageModel = Helper.ItemImage(secondSlot[index, i], image);

                newPerks.Add(itemImageModel);
            }
            return newPerks;
        }

        private int lastSlotIndex = 0;
        private readonly int[] lastSelectedSlot = new int[] {-1, -1};
        private void selectSecondPathSlot(string selectedPerk, int index = -1)
        {
            var changeNeeded = false;

            switch (index)
            {
                case 0:
                    changeNeeded = currentPerk4 != selectedPerk;
                    break;
                case 1:
                    changeNeeded = currentPerk5 != selectedPerk;
                    break;
                case 2:
                    changeNeeded = currentPerk6 != selectedPerk;
                    break;
            }

            if (changeNeeded)
            {
                switch (index)
                {
                    case 0:
                        selectSecondaryPerk(selectedPerk, currentPerk4, currentPerk5, currentPerk6, index);
                        break;
                    case 1:
                        selectSecondaryPerk(selectedPerk, currentPerk5, currentPerk4, currentPerk6, index);
                        break;
                    case 2:
                        selectSecondaryPerk(selectedPerk, currentPerk6, currentPerk4, currentPerk5, index);
                        break;
                }
            }
        }

        private void selectSecondaryPerk(string selectedPerk, string perk1, string perk2, string perk3, int index)
        {
            var selectedPerkIndex = getSecondaryPathSlotIndex(selectedPerk);
            var perk1Index = getSecondaryPathSlotIndex(perk1);
            var perk2Index = getSecondaryPathSlotIndex(perk2);
            var perk3Index = getSecondaryPathSlotIndex(perk3);

            int lastSlot = lastSelectedSlot[lastSlotIndex];
            var lastIndex = lastSlotIndex;

            if (!string.IsNullOrEmpty(selectedPerk))
                lastSlotIndex = lastSlotIndex == 0 ? 1 : 0;

            var perks = populateSecondaryPathSlot(currentSecondaryPath, selectedPerk, index);

            setSecondaryPathPerksByIndex(perks, index);

            var perk1Temp = perk1;
            perk1 = setSecondaryPathCurrentPerkByIndex(selectedPerk, index);

            if (perk1 != null)
            {
                if (perk2Index == -1 && perk3Index == -1)
                {
                    rune.Slot4 = Converter.PerkNameToId(selectedPerk);

                    lastSlotIndex = 0;
                    lastSelectedSlot[0] = selectedPerkIndex;
                }
                else if (perk2Index != -1 && perk3Index != -1)
                {
                    if (lastSlot == perk2Index)
                    {
                        setSecondaryPathRuneSlot(selectedPerk, perk2);

                        perk2 = setSecondaryPathCurrentPerkByIndex(null, perk2Index);
                        setSecondaryPathPerksByIndex(populateSecondaryPathSlot(currentSecondaryPath, perk2, perk2Index), perk2Index);
                    }
                    else if (lastSlot == perk3Index)
                    {
                        setSecondaryPathRuneSlot(selectedPerk, perk3);

                        perk3 = setSecondaryPathCurrentPerkByIndex(null, perk3Index);
                        setSecondaryPathPerksByIndex(populateSecondaryPathSlot(currentSecondaryPath, perk3, perk3Index), perk3Index);
                    }

                    lastSelectedSlot[lastIndex] = getSecondaryPathSlotIndex(perk1);
                }
                else if ((perk1Index == -1 && perk2Index == -1 && perk3Index != -1)
                || (perk1Index == -1 && perk2Index != -1 && perk3Index == -1))
                {
                    var currentRune = perk2Index != -1 ? perk2 : perk3;
                    setSecondaryPathRuneSlot(selectedPerk, currentRune);

                    lastSlotIndex = 0;
                    lastSelectedSlot[1] = selectedPerkIndex;
                }
                else if (perk1Index != -1)
                {
                    setSecondaryPathRuneSlot(selectedPerk, perk1Temp);
                    lastSlotIndex = lastSlotIndex == 0 ? 1 : 0;
                }
            }
        }

        private void setSecondaryPathRuneSlot(string selectedPerk, string currentPerk)
        {
            var currentPerkId = Converter.PerkNameToId(selectedPerk);
            var selectedPerkId = Converter.PerkNameToId(selectedPerk);
            var slot4 = Converter.PerkIdToName(rune.Slot4);
            var slot5 = Converter.PerkIdToName(rune.Slot5);


            if (rune.Slot4 == Converter.PerkNameToId(currentPerk))
                rune.Slot4 = Converter.PerkNameToId(selectedPerk);
            else
                rune.Slot5 = Converter.PerkNameToId(selectedPerk);
        }

        private void clearCurrentRunes()
        {
            lastSlotIndex = 0;
            lastSelectedSlot[0] = -1;
            lastSelectedSlot[1] = -1;
            currentPrimaryPath = null;
            currentKeystone = null;
            currentPerk1 = null;
            currentPerk2 = null;
            currentPerk3 = null;
            currentSecondaryPath = null;
            currentPerk4 = null;
            currentPerk5 = null;
            currentPerk6 = null;
            currentShard1 = null;
            currentShard2 = null;
            currentShard3 = null;
        }

        private void updateSecondaryPathSlots(string selectedPerk4, string selectedPerk5)
        {
            secondSlot = RuneModel.r_SecondPath[currentSecondaryPath];

            Perks4 = populateSecondaryPathSlot(currentSecondaryPath, null, 0);
            Perks5 = populateSecondaryPathSlot(currentSecondaryPath, null, 1);
            Perks6 = populateSecondaryPathSlot(currentSecondaryPath, null, 2);

            var slot4Index = getSecondaryPathSlotIndex(selectedPerk4);
            var slot5Index = getSecondaryPathSlotIndex(selectedPerk5);

            lastSlotIndex = 0; // reset last slot index

            if(slot4Index != -1)
            {
                lastSelectedSlot[0] = slot4Index;
                var newPerks4 = new ObservableCollection<ItemImageModel>();
                for (int i = 0; i < secondSlot.GetLength(1); i++)
                {
                    var rune = secondSlot[slot4Index, i];

                    if (rune == null) continue;

                    var image = selectedPerk4 != null && selectedPerk4.Equals(rune) ?
                    Helper.ImageSrc(rune) : Helper.ImageSrc($"g_{rune}");
                    var itemImageModel = Helper.ItemImage(rune, image);

                    newPerks4.Add(itemImageModel);
                }

                setSecondaryPathPerksByIndex(newPerks4, slot4Index);
                setSecondaryPathCurrentPerkByIndex(selectedPerk4, slot4Index);
            }

            if(slot5Index != -1)
            {
                lastSelectedSlot[1] = slot5Index;
                var newPerks5 = new ObservableCollection<ItemImageModel>();
                for (int i = 0; i < secondSlot.GetLength(1); i++)
                {
                    var rune = secondSlot[slot5Index, i];

                    if (rune == null) continue;

                    var image = selectedPerk5 != null && selectedPerk5.Equals(rune) ?
                    Helper.ImageSrc(rune) : Helper.ImageSrc($"g_{rune}");
                    var itemImageModel = Helper.ItemImage(rune, image);

                    newPerks5.Add(itemImageModel);
                }

                setSecondaryPathPerksByIndex(newPerks5, slot5Index);
                setSecondaryPathCurrentPerkByIndex(selectedPerk5, slot5Index);
            }
        }

        private int getSecondaryPathSlotIndex(string selectedPerk)
        {
            if(selectedPerk != null)
            {
                for (int i = 0; i < secondSlot.GetLength(1); i++)
                {
                    if (secondSlot[0, i] == selectedPerk)
                        return 0;
                }
                for (int i = 0; i < secondSlot.GetLength(1); i++)
                {
                    if (secondSlot[1, i] == selectedPerk)
                        return 1;
                }
                for (int i = 0; i < secondSlot.GetLength(1); i++)
                {
                    if (secondSlot[2, i] == selectedPerk)
                        return 2;
                }
            }
            return -1;
        }

        private void updateShard(string selectedShard, int row)
        {
            bool changeNeeded = false;

            switch (row)
            {
                case 1:
                    changeNeeded = currentShard1 != selectedShard;
                    break;
                case 2:
                    changeNeeded = currentShard2 != selectedShard;
                    break;
                case 3:
                    changeNeeded = currentShard3 != selectedShard;
                    break;
            }

            if (changeNeeded)
            {
                var newShards = new ObservableCollection<ItemImageModel>();
                foreach (var shard in RuneModel.Shards(row, true))
                {
                    if (shard.Text == selectedShard)
                        shard.Image = Helper.ImageSrc(DataConverter.ShardDescriptionToShardAlias(shard.Text));

                    newShards.Add(shard);
                }

                switch(row)
                {
                    case 1:
                        Shards1 = newShards;
                        currentShard1 = selectedShard;
                        rune.Shard1 = DataConverter.ShardDescriptionToShardId(selectedShard);
                        break;
                    case 2:
                        Shards2 = newShards;
                        currentShard2 = selectedShard;
                        rune.Shard2 = DataConverter.ShardDescriptionToShardId(selectedShard);
                        break;
                    case 3:
                        Shards3 = newShards;
                        currentShard3 = selectedShard;
                        rune.Shard3 = DataConverter.ShardDescriptionToShardId(selectedShard);
                        break;
                }
            }
        }

        private void updatePrimaryPathSlots(string selectedPath)
        {
            Perks1 = new ObservableCollection<ItemImageModel>();
            Perks2 = new ObservableCollection<ItemImageModel>();
            Perks3 = new ObservableCollection<ItemImageModel>();
            Keystones = new ObservableCollection<ItemImageModel>();

            if (currentPrimaryPath != null)
            {
                int i = 0;
                foreach (var perk in DataDragonWrapper.s_Perks)
                {
                    if (perk.name == selectedPath)
                    {
                        foreach (var slot in perk.slots)
                        {
                            foreach (var rune in slot.runes)
                            {
                                var model = new ItemImageModel() {
                                    Text = rune.name,
                                    Image = Helper.ImageSrc($"g_{rune.name}")
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
        }

        private string setSecondaryPathCurrentPerkByIndex(string perk, int index)
        {
            switch (index)
            {
                case 0:
                    currentPerk4 = perk;
                    break;
                case 1:
                    currentPerk5 = perk;
                    break;
                case 2:
                    currentPerk6 = perk;
                    break;
            }
            return perk;
        }

        private void setSecondaryPathPerksByIndex(ObservableCollection<ItemImageModel> perks, int index)
        {
            switch (index)
            {
                case 0:
                    Perks4 = perks;
                    break;
                case 1:
                    Perks5 = perks;
                    break;
                case 2:
                    Perks6 = perks;
                    break;
            }
        }

        private ObservableCollection<ItemImageModel> itemImageModels(string currentValue, string newValue, ObservableCollection<ItemImageModel> itemModels)
        {
            if (currentValue != newValue)
            {
                var newItemModels = new ObservableCollection<ItemImageModel>();

                foreach (var model in itemModels)
                {
                    var itemModel = new ItemImageModel
                    {
                        Text = model.Text,
                        Image = newValue.Equals(model.Text) ?
                        Helper.ImageSrc(newValue) : Helper.ImageSrc($"g_{model.Text}")
                    };

                    newItemModels.Add(itemModel);
                }
                return newItemModels;
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
