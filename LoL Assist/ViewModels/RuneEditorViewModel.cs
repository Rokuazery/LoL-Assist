using LoLA.Networking.WebWrapper.DataDragon.Data;
using LoLA.Networking.WebWrapper.DataDragon;
using System.Collections.ObjectModel;
using LoLA.Networking.LCU.Objects;
using System.Collections.Generic;
using LoL_Assist_WAPP.Commands;
using System.Windows.Controls;
using LoL_Assist_WAPP.Models;
using LoL_Assist_WAPP.Utils;
using System.ComponentModel;
using System.Windows.Input;
using LoLA.Networking.LCU;
using LoLA.Data;
using LoLA;

namespace LoL_Assist_WAPP.ViewModels
{
    public class RuneEditorViewModel: ViewModelBase
    {
        #region Commands
        public ICommand OpenBuildEditorCommand { get; }
        public ICommand SaveRunePageCommand { get; }
        public ICommand ReloadRunePagesCommand { get; }
        public ICommand CloseRuneEditorCommand { get; }

        #region Perk Commands
        public ICommand PrimaryPathSelectCommand { get; }
        public ICommand KeystoneSelectCommand { get; }
        public ICommand Perk1SelectCommand { get; }
        public ICommand Perk2SelectCommand { get; }
        public ICommand Perk3SelectCommand { get; }
        public ICommand SecondaryPathSelectCommand { get; }
        public ICommand Perk4SelectCommand { get; }
        public ICommand Perk5SelectCommand { get; }
        public ICommand Perk6SelectCommand { get; }
        public ICommand Shard1SelectCommand { get; }
        public ICommand Shard2SelectCommand { get; }
        public ICommand Shard3SelectCommand { get; }
        #endregion

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
                        SetSelectedRunePageByName(value);
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
            PrimaryPathSelectCommand = new Command(path => UpdatePrimaryPath(path.ToString()));
            KeystoneSelectCommand = new Command(keystone => UpdateKeystone(keystone?.ToString()));
            Perk1SelectCommand = new Command(perk => UpdatePrimaryPathSlot(perk?.ToString(), 1));
            Perk2SelectCommand = new Command(perk => UpdatePrimaryPathSlot(perk?.ToString(), 2));
            Perk3SelectCommand = new Command(perk => UpdatePrimaryPathSlot(perk?.ToString(), 3));
            SecondaryPathSelectCommand = new Command(path => UpdateSecondaryPath(path.ToString()));
            Perk4SelectCommand = new Command(perk => UpdateSecondPathSlot(perk?.ToString(), 0));
            Perk5SelectCommand = new Command(perk => UpdateSecondPathSlot(perk?.ToString(), 1));
            Perk6SelectCommand = new Command(perk => UpdateSecondPathSlot(perk?.ToString(), 2));
            Shard1SelectCommand = new Command(shard => UpdateShard(shard?.ToString(), 1));
            Shard2SelectCommand = new Command(shard => UpdateShard(shard?.ToString(), 2));
            Shard3SelectCommand = new Command(shard => UpdateShard(shard?.ToString(), 3));

            CloseRuneEditorCommand = new Command(usercontrol => CloseRuneEditor(usercontrol));
            OpenBuildEditorCommand = new Command(_ => Helper.ShowBuildEditorWindow());
            ReloadRunePagesCommand = new Command(_ => LoadRunePages());
            SaveRunePageCommand = new Command(_ => SaveRunePage());

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

            LoadRunePages();
        }

        private bool isSaving = false;
        private async void SaveRunePage()
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
                                LoadRunePages(); // reload pages
                        }
                    }
                }
                isSaving = false;
            }
        }

        private async void LoadRunePages()
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

        private void CloseRuneEditor(object usercontrol)
        {
            var control = usercontrol as UserControl;
            Animation.FadeOut(control);
        }

        private void SetSelectedRunePageByName(string runePageName)
        {
            var selectedRunePage = r_runePages.Find(runePage => runePage.name == runePageName);

            if (selectedRunePage != null)
                SetCurrentRunePage(selectedRunePage);
        }

        private void SetCurrentRunePage(RunePage runePage)
        {
            rune = Converter.RunePageToRune(runePage);
            UpdatePrimaryPath(Converter.PathIdToName(rune.PrimaryPath));
            UpdateKeystone(Converter.PerkIdToName(rune.Keystone));
            UpdatePrimaryPathSlot(Converter.PerkIdToName(rune.Slot1), 1);
            UpdatePrimaryPathSlot(Converter.PerkIdToName(rune.Slot2), 2);
            UpdatePrimaryPathSlot(Converter.PerkIdToName(rune.Slot3), 3);
            UpdateSecondaryPath(Converter.PathIdToName(rune.SecondaryPath));

            updateSecondaryPathSlots(Converter.PerkIdToName(rune.Slot4), Converter.PerkIdToName(rune.Slot5));
            UpdateShard(DataConverter.ShardIdToShardDescription(rune.Shard1), 1);
            UpdateShard(DataConverter.ShardIdToShardDescription(rune.Shard2), 2);
            UpdateShard(DataConverter.ShardIdToShardDescription(rune.Shard3), 3);
        }

        private void UpdatePrimaryPath(string selectedPath = "")
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
                UpdatePrimaryPathSlots(currentPrimaryPath);

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
                    currentSecondaryPath = null;
                    currentPerk4 = null;
                    currentPerk5 = null;
                    currentPerk6 = null;


                    Perks4.Clear();
                    Perks5.Clear();
                    Perks6.Clear();
                }

                if (!string.IsNullOrEmpty(currentPrimaryPath))
                    rune.PrimaryPath = Converter.PathNameToId(currentPrimaryPath);
            }
        }

        private string[,] secondSlot = null;
        private void UpdateSecondaryPath(string selectedSecondaryPath)
        {
            var newSecondaryPaths = ItemImageModels(currentSecondaryPath, selectedSecondaryPath, SecondaryPaths);

            if (newSecondaryPaths != null)
            {
                currentSecondaryPath = selectedSecondaryPath;

                rune.SecondaryPath = Converter.PathNameToId(selectedSecondaryPath);

                SecondaryPaths = newSecondaryPaths;

                currentPerk4 = null;
                currentPerk5 = null;
                currentPerk6 = null;

                Perks4 = PopulateSecondaryPathSlot(currentSecondaryPath, null, 0);
                Perks5 = PopulateSecondaryPathSlot(currentSecondaryPath, null, 1);
                Perks6 = PopulateSecondaryPathSlot(currentSecondaryPath, null, 2);
            }
        }

        private void UpdateKeystone(string selectedKeystone)
        {
            var newKeystones = ItemImageModels(currentKeystone, selectedKeystone, Keystones);

            if (newKeystones != null)
            {
                currentKeystone = selectedKeystone;

                rune.Keystone = Converter.PerkNameToId(currentKeystone);

                Keystones = newKeystones;
            }
        }

        private void UpdatePrimaryPathSlot(string selectedPerk, int slot)
        {
            ObservableCollection<ItemImageModel> newPerks;
            switch (slot)
            {
                case 1:
                    newPerks = ItemImageModels(currentPerk1, selectedPerk, Perks1);

                    if(newPerks != null)
                    {
                        currentPerk1 = selectedPerk;

                        rune.Slot1 = Converter.PerkNameToId(currentPerk1);

                        Perks1 = newPerks;
                    }
                    break;
                case 2:
                    newPerks = ItemImageModels(currentPerk2, selectedPerk, Perks2);

                    if (newPerks != null)
                    {
                        currentPerk2 = selectedPerk;

                        rune.Slot2 = Converter.PerkNameToId(currentPerk2);

                        Perks2 = newPerks;
                    }
                    break;
                case 3:
                    newPerks = ItemImageModels(currentPerk3, selectedPerk, Perks3);

                    if (newPerks != null)
                    {
                        currentPerk3 = selectedPerk;

                        rune.Slot3 = Converter.PerkNameToId(currentPerk3);

                        Perks3 = newPerks;
                    }
                    break;
            }
        }

        private ObservableCollection<ItemImageModel> PopulateSecondaryPathSlot(string secondaryPath, string selectedPerk, int index)
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
        private void UpdateSecondPathSlot(string selectedPerk, int index = -1)
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
                        SelectSecondaryPerk(selectedPerk, currentPerk4, currentPerk5, currentPerk6, index);
                        break;
                    case 1:
                        SelectSecondaryPerk(selectedPerk, currentPerk5, currentPerk4, currentPerk6, index);
                        break;
                    case 2:
                        SelectSecondaryPerk(selectedPerk, currentPerk6, currentPerk4, currentPerk5, index);
                        break;
                }
            }
        }

        private void SelectSecondaryPerk(string selectedPerk, string perk1, string perk2, string perk3, int index)
        {
            var selectedPerkIndex = GetSecondaryPathSlotIndex(selectedPerk);
            var perk1Index = GetSecondaryPathSlotIndex(perk1);
            var perk2Index = GetSecondaryPathSlotIndex(perk2);
            var perk3Index = GetSecondaryPathSlotIndex(perk3);

            int lastSlot = lastSelectedSlot[lastSlotIndex];
            var lastIndex = lastSlotIndex;
            bool isSlotChanged = !string.IsNullOrEmpty(selectedPerk);

            lastSlotIndex = isSlotChanged ? (lastSlotIndex == 0 ? 1 : 0) : lastSlotIndex;

            var perks = PopulateSecondaryPathSlot(currentSecondaryPath, selectedPerk, index);

            SetSecondaryPathPerksByIndex(perks, index);

            var perk1Temp = perk1;
            perk1 = SetSecondaryPathCurrentPerkByIndex(selectedPerk, index);

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
                        SetSecondaryPathRuneSlot(selectedPerk, perk2);

                        perk2 = SetSecondaryPathCurrentPerkByIndex(null, perk2Index);
                        SetSecondaryPathPerksByIndex(PopulateSecondaryPathSlot(currentSecondaryPath, perk2, perk2Index), perk2Index);
                    }
                    else if (lastSlot == perk3Index)
                    {
                        SetSecondaryPathRuneSlot(selectedPerk, perk3);

                        perk3 = SetSecondaryPathCurrentPerkByIndex(null, perk3Index);
                        SetSecondaryPathPerksByIndex(PopulateSecondaryPathSlot(currentSecondaryPath, perk3, perk3Index), perk3Index);
                    }

                    lastSelectedSlot[lastIndex] = GetSecondaryPathSlotIndex(perk1);
                }
                else if ((perk1Index == -1 && perk2Index == -1 && perk3Index != -1)
                || (perk1Index == -1 && perk2Index != -1 && perk3Index == -1))
                {
                    var currentRune = perk2Index != -1 ? perk2 : perk3;
                    SetSecondaryPathRuneSlot(selectedPerk, currentRune);

                    lastSlotIndex = 0;
                    lastSelectedSlot[1] = selectedPerkIndex;
                }
                else if (perk1Index != -1)
                {
                    SetSecondaryPathRuneSlot(selectedPerk, perk1Temp);
                    lastSlotIndex = lastSlotIndex == 0 ? 1 : 0;
                }
            }
        }

        private void SetSecondaryPathRuneSlot(string selectedPerk, string currentPerk)
        {
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
            if (string.IsNullOrEmpty(currentSecondaryPath)) return;

            secondSlot = RuneModel.r_SecondPath[currentSecondaryPath];

            Perks4 = PopulateSecondaryPathSlot(currentSecondaryPath, null, 0);
            Perks5 = PopulateSecondaryPathSlot(currentSecondaryPath, null, 1);
            Perks6 = PopulateSecondaryPathSlot(currentSecondaryPath, null, 2);

            var slot4Index = GetSecondaryPathSlotIndex(selectedPerk4);
            var slot5Index = GetSecondaryPathSlotIndex(selectedPerk5);

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

                SetSecondaryPathPerksByIndex(newPerks4, slot4Index);
                SetSecondaryPathCurrentPerkByIndex(selectedPerk4, slot4Index);
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

                SetSecondaryPathPerksByIndex(newPerks5, slot5Index);
                SetSecondaryPathCurrentPerkByIndex(selectedPerk5, slot5Index);
            }
        }

        private int GetSecondaryPathSlotIndex(string selectedPerk)
        {
            if (selectedPerk == null) return -1;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < secondSlot.GetLength(1); j++)
                {
                    if (secondSlot[i, j] == selectedPerk) return i;
                }
            }
            return -1;
        }

        private void UpdateShard(string selectedShard, int row)
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

        private void UpdatePrimaryPathSlots(string selectedPath)
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

        private string SetSecondaryPathCurrentPerkByIndex(string perk, int index)
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

        private void SetSecondaryPathPerksByIndex(ObservableCollection<ItemImageModel> perks, int index)
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

        private ObservableCollection<ItemImageModel> ItemImageModels(string currentValue, string newValue, ObservableCollection<ItemImageModel> itemModels)
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
    }
}
