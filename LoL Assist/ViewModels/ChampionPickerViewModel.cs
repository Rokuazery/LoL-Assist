using LoLA.Networking.WebWrapper.DataDragon;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;

namespace LoL_Assist_WAPP.ViewModels
{
    public class ChampionPickerViewModel: ViewModelBase
    {
        private CollectionViewSource ChampionsCollection;
        public ICollectionView ChampionCollectionView => ChampionsCollection.View;

        private ObservableCollection<string> _champions;
        public ChampionPickerViewModel()
        {
            //_champions = new ObservableCollection<string>();
            ChampionsCollection = new CollectionViewSource { Source = _champions };

            _champions = new ObservableCollection<string>(
                DataDragonWrapper.s_Champions.Data.Values.Select(championData => championData.name)
            );
        }
    }
}
