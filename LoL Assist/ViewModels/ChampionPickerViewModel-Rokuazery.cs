using LoLA.Networking.WebWrapper.DataDragon;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Models;
using System.ComponentModel;
using System;

namespace LoL_Assist_WAPP.ViewModels
{
    public class ChampionPickerViewModel: INotifyPropertyChanged
    {
        //private ObservableCollection<string> _champions;
        //public ObservableCollection<string> Champions
        //{
        //    get => _champions;
        //    set
        //    {
        //        if (_champions != value)
        //        {
        //            _champions = value;
        //            OnPropertyChanged(nameof(Champions));
        //        }
        //    }
        //}

        //private ObservableCollection<string> _championPicks;
        //public ObservableCollection<string> ChampionPicks
        //{
        //    get => _championPicks;
        //    set
        //    {
        //        if (_championPicks != value)
        //        {
        //            _championPicks = value;
        //            OnPropertyChanged(nameof(ChampionPicks));
        //        }
        //    }
        //}

        //private ObservableCollection<string> _championBans;
        //public ObservableCollection<string> ChampionBans
        //{
        //    get => _championBans;
        //    set
        //    {
        //        if (_championBans != value)
        //        {
        //            _championBans = value;
        //            OnPropertyChanged(nameof(ChampionBans));
        //        }
        //    }
        //}
        public ItemListViewModel Champions { get; }
        public ItemListViewModel ChampionPicks { get; }
        public ItemListViewModel ChampionBans { get; }
        public ChampionPickerViewModel()
        {
            Champions = new ItemListViewModel();
            ChampionPicks = new ItemListViewModel();
            ChampionBans = new ItemListViewModel();

            foreach (var championData in DataDragonWrapper.s_Champions.Data)
                Champions.AddItem(championData.Value.name);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
