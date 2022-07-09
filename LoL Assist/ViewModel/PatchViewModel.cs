using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using System.Text;

namespace LoL_Assist_WAPP.ViewModel
{
    public class PatchViewModel : INotifyPropertyChanged
    {
        public ICommand RunUrlCommand { get; }

        private ICommand _hideUserControlCommand;
        public ICommand HideUserControlCommand
        {
            get
            {
                if (_hideUserControlCommand == null)
                {
                    _hideUserControlCommand = new Command(o => hideUserControl(o));
                }
                return _hideUserControlCommand;
            }
        }

        private bool _doNotShowPatchNote;
        public bool DoNotShowPatchNote
        {
            get => _doNotShowPatchNote;
            set
            {
                if (_doNotShowPatchNote != value)
                {
                    _doNotShowPatchNote = value;

                    ConfigModel.s_Config.DoNotShowPatch = value;
                    ConfigModel.SaveConfig();

                    OnPropertyChanged(nameof(DoNotShowPatchNote));
                }
            }
        }

        private string _patchNotes;
        public string PatchNotes
        {
            get => _patchNotes;
            set
            {
                if (_patchNotes != value)
                {
                    _patchNotes = value;
                    OnPropertyChanged(nameof(PatchNotes));
                }
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public PatchViewModel()
        {
            RunUrlCommand = new Command(o => { Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ"); });
            DoNotShowPatchNote = ConfigModel.s_Config.DoNotShowPatch;
            Title = $"What's New in v{ConfigModel.r_Version}";
            initPatchNote();
        }

        private void initPatchNote()
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

        private void hideUserControl(object p) => Utils.Animation.FadeOut(p as UserControl);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
