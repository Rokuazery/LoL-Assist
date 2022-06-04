using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Text;
using System;
using System.Windows.Controls;
using System.Diagnostics;

namespace LoL_Assist_WAPP.ViewModel
{
    internal class PatchViewModel : INotifyPropertyChanged
    {
        public ICommand RunUrlCommand { get; }

        private ICommand hideUserControlCommand;
        public ICommand HideUserControlCommand
        {
            get
            {
                if (hideUserControlCommand == null)
                {
                    hideUserControlCommand = new Command(o => HideUserControl(o));
                }
                return hideUserControlCommand;
            }
        }

        private bool doNotShowPatchNote;
        public bool DoNotShowPatchNote
        {
            get => doNotShowPatchNote;
            set
            {
                if (doNotShowPatchNote != value)
                {
                    doNotShowPatchNote = value;

                    ConfigModel.config.DoNotShowPatch = value;
                    ConfigModel.SaveConfig();

                    OnPropertyChanged(nameof(DoNotShowPatchNote));
                }
            }
        }

        private string patchNotes;
        public string PatchNotes
        {
            get => patchNotes;
            set
            {
                if (patchNotes != value)
                {
                    patchNotes = value;
                    OnPropertyChanged(nameof(PatchNotes));
                }
            }
        }

        private string title;
        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public PatchViewModel()
        {
            RunUrlCommand = new Command(o => { Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ"); });
            DoNotShowPatchNote = ConfigModel.config.DoNotShowPatch;
            Title = $"What's New in v{ConfigModel.version}";
            SetPatchNotes();
        }

        void SetPatchNotes()
        {
            StringBuilder patchNotes = new StringBuilder();
            patchNotes.AppendLine("• Added an option for role select (available for a certain game mode only)");
            patchNotes.AppendLine("• Added a system tray menu [Exit, Show, Patch Notes, Minimize to Tray]");
            patchNotes.AppendLine("• Fixed an issue where LoL Assist UI being unresponsive");
            patchNotes.AppendLine("• Minor UI improvements");
            patchNotes.AppendLine("• Better MVVM Bindings");
            patchNotes.AppendLine("• Code clean up"); 

            PatchNotes = patchNotes.ToString();
        }

        private void HideUserControl(object p) => Utils.Animation.FadeOut(p as UserControl);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
