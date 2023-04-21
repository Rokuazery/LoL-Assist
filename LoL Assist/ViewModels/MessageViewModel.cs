using LoL_Assist_WAPP.Commands;
using System.Windows.Controls;
using LoL_Assist_WAPP.Models;
using System.ComponentModel;
using LoL_Assist_WAPP.Utils;
using System.Windows.Input;
using System;

namespace LoL_Assist_WAPP.ViewModels
{
    public class MessageViewModel: ViewModelBase
    {
        public ICommand CloseMessageCommand { get; }
        public MessageViewModel()
        {
            CloseMessageCommand = new Command(u => Close(u));
            ConfigModel.LoadConfig();
        }
        
        private void Close(object userControl)
        {
            var control = userControl as UserControl;
            Animation.FadeOut(control);
        }
    }
}
