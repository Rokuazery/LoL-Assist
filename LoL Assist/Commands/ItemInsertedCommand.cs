using LoL_Assist_WAPP.ViewModels;
using System.Windows.Input;
using System;

namespace LoL_Assist_WAPP.Commands
{
    public class ItemInsertedCommand: ICommand
    {
        private readonly ItemListViewModel _viewModel;
        public ItemInsertedCommand(ItemListViewModel itemListingViewModel)
        {
            _viewModel = itemListingViewModel;
        }

        public void Execute(object parameter)
        {
            _viewModel.InsertItem(_viewModel.InsertedItemViewModel, _viewModel.TargetItemViewModel);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        protected void OnCanExecuteChanged() =>
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
