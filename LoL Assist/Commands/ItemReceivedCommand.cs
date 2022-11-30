using LoL_Assist_WAPP.ViewModels;
using System.Windows.Input;
using System;

namespace LoL_Assist_WAPP.Commands
{
    public class ItemReceivedCommand: ICommand
    {
        private readonly ItemListViewModel _viewModel;
        public ItemReceivedCommand(ItemListViewModel itemListingViewModel)
        {
            _viewModel = itemListingViewModel;
        }

        public void Execute(object parameter)
        {
            _viewModel.AddItem(_viewModel.IncomingItemViewModel);
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
