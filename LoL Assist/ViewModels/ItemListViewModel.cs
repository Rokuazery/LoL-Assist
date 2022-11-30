using System.Collections.ObjectModel;
using System.Collections.Generic;
using LoL_Assist_WAPP.Commands;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using System.Linq;
using System;

namespace LoL_Assist_WAPP.ViewModels
{
    public class ItemListViewModel: ViewModelBase
    {
        private ObservableCollection<ItemViewModel> _itemViewModels;

        private CollectionViewSource ItemViewModelsCollection;
        public ICollectionView ItemViewModels => ItemViewModelsCollection.View;

        private ItemViewModel _incomingItemViewModel;
        public ItemViewModel IncomingItemViewModel
        {
            get => _incomingItemViewModel;
            set
            {
                if(_incomingItemViewModel != value)
                {
                    _incomingItemViewModel = value;
                    OnPropertyChanged(nameof(IncomingItemViewModel));
                }
            }
        }

        private ItemViewModel _removedItemViewModel;
        public ItemViewModel RemovedItemViewModel
        {
            get => _removedItemViewModel;
            set
            {
                if(_removedItemViewModel != value)
                {
                    _removedItemViewModel = value;
                    OnPropertyChanged(nameof(RemovedItemViewModel));
                }
            }
        }

        private ItemViewModel _insertedItemViewModel;
        public ItemViewModel InsertedItemViewModel
        {
            get => _insertedItemViewModel;
            set
            {
                if(_insertedItemViewModel != value)
                {
                    _insertedItemViewModel = value;
                    OnPropertyChanged(nameof(InsertedItemViewModel));
                }
            }
        }

        private ItemViewModel _targetItemViewModel;
        public ItemViewModel TargetItemViewModel
        {
            get => _targetItemViewModel;
            set
            {
                if(_targetItemViewModel != value)
                {
                    _targetItemViewModel = value;
                    OnPropertyChanged(nameof(TargetItemViewModel));
                }
            }
        }

        private string _searchString;
        public string SearchString
        {
            get => _searchString;
            set
            {
                if (_searchString != value)
                {
                    _searchString = value;
                    ItemViewModelsCollection.View.Refresh();
                    OnPropertyChanged(nameof(SearchString));
                }
            }
        }

        //public string Nom { get; set; }
        public bool IsOrigin = false;
        public ICommand ItemReceivedCommand { get; }
        public ICommand ItemRemovedCommand { get; }
        public ICommand ItemInsertedCommand { get; }

        public ItemListViewModel()
        {
            _itemViewModels = new ObservableCollection<ItemViewModel>();

            ItemReceivedCommand = new ItemReceivedCommand(this);
            ItemRemovedCommand = new ItemRemovedCommand(this);
            ItemInsertedCommand = new ItemInsertedCommand(this);
            ItemViewModelsCollection = new CollectionViewSource { Source = _itemViewModels };
            ItemViewModelsCollection.Filter += Collection_Filter;
        }

        public void AddItem(ItemViewModel item)
        {
            if (!_itemViewModels.Contains(item))
            {
                if (IsOrigin)
                {
                    item.ActualIndex = -1;
                    _itemViewModels.Insert(item.OriginIndex, item);
                }
                else
                {
                    item.ActualIndex = item.ActualIndex == -1 ? _itemViewModels.Count : item.ActualIndex;
                    _itemViewModels.Insert(item.ActualIndex, item);
                }
            }
        }

        private void Collection_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchString))
            {
                e.Accepted = true;
                return;
            }

            var item = e.Item as ItemViewModel;
            if (item.Content.ToString().ToLower().Contains(SearchString.ToLower()))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        public void InsertItem(ItemViewModel insertedTodoItem, ItemViewModel targetTodoItem)
        {
            if(!IsOrigin)
            {

            }
            if (insertedTodoItem == targetTodoItem)
            {
                return;
            }

            int oldIndex = _itemViewModels.IndexOf(insertedTodoItem);
            int nextIndex = _itemViewModels.IndexOf(targetTodoItem);

            if (oldIndex != -1 && nextIndex != -1)
            {
                _itemViewModels.Move(oldIndex, nextIndex);

                _itemViewModels[oldIndex].ActualIndex = oldIndex;
                _itemViewModels[nextIndex].ActualIndex = nextIndex;
            }
        }

        public void RemoveItem(ItemViewModel item)
        {
            _itemViewModels.Remove(item);
        }
    }
}
