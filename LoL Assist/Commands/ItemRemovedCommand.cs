using LoL_Assist_WAPP.ViewModels;
using System.Windows.Input;
using System;

namespace LoL_Assist_WAPP.Commands
{
    public class ItemRemovedCommand: ICommand
    {
        private readonly ItemListViewModel _viewModel;
        public ItemRemovedCommand(ItemListViewModel itemListingViewModel)
        {
            _viewModel = itemListingViewModel;
        }

        public void Execute(object parameter)
        {
            _viewModel.RemoveItem(_viewModel.RemovedItemViewModel);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        protected void OnCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
