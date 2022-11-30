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
        private bool _autoMessageEnabeld;
        public bool AutoMessageEnabled
        {
            get => _autoMessageEnabeld;
            set
            {
                if (_autoMessageEnabeld != value)
                {
                    _autoMessageEnabeld = value;
                    ConfigModel.s_Config.AutoMessage = value;
                    ConfigModel.SaveConfig(false);
                    OnPropertyChanged(nameof(AutoMessageEnabled));
                }
            }
        }

        private bool _clearAfterEachSent;
        public bool ClearAfterEachSent
        {
            get => _clearAfterEachSent;
            set
            {
                if (_clearAfterEachSent != value)
                {
                    _clearAfterEachSent = value;
                    ConfigModel.s_Config.ClearMessageAfterSent = value;
                    ConfigModel.SaveConfig(false);
                    OnPropertyChanged(nameof(ClearAfterEachSent));
                }
            }
        }

        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set
            {
                if (_messageText != value)
                {
                    _messageText = value;
                    ConfigModel.s_Config.Message = value;
                    ConfigModel.SaveConfig(false);
                    OnPropertyChanged(nameof(MessageText));
                }
            }
        }
        public static Action s_ClearMessage;
        public MessageViewModel()
        {
            CloseMessageCommand = new Command(u => Close(u));

            ConfigModel.LoadConfig();
            s_ClearMessage = new Action(ClearMessage);
            AutoMessageEnabled = ConfigModel.s_Config.AutoMessage;
            MessageText = ConfigModel.s_Config.Message;
            ClearAfterEachSent = ConfigModel.s_Config.ClearMessageAfterSent;
        }
        
        private void Close(object userControl)
        {
            var control = userControl as UserControl;
            Animation.FadeOut(control);
        }

        public void ClearMessage() 
            => MessageText = string.Empty;
    }
}
