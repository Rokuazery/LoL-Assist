using LoLA.Networking.WebWrapper.DataDragon;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Models;
using System.ComponentModel;
using System;
using System.Windows.Data;

namespace LoL_Assist_WAPP.ViewModels
{
    public class ChampionPickerViewModel: ViewModelBase
    {
        private CollectionViewSource ChampionsCollection;
        public ICollectionView ChampionCollectionView => ChampionsCollection.View;



        private ObservableCollection<string> _champions;
        //public ItemListViewModel Champions { get; }
        //public ItemListViewModel ChampionPicks { get; }
        public ChampionPickerViewModel()
        {
            _champions = new ObservableCollection<string>();
            ChampionsCollection = new CollectionViewSource { Source = _champions };

            foreach (var championData in DataDragonWrapper.s_Champions.Data)
            {
                _champions.Add(championData.Value.name);
            }

            //Champions = new ItemListViewModel();
            //ChampionPicks = new ItemListViewModel();

            //int index = 0;
            //Champions.IsOrigin = true;
            //foreach (var championData in DataDragonWrapper.s_Champions.Data)
            //{
            //    var itemViewModel = new ItemViewModel() {
            //        OriginIndex = index,
            //        Content = championData.Value.name
            //    };

            //    Champions.AddItem(itemViewModel);
            //    index++;
            //}
             
        }
    }
}
