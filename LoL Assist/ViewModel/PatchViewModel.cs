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
            patchNotes.AppendLine("• Added U.GG Runes");
            patchNotes.AppendLine("• Added a realtime rune editor");
            patchNotes.AppendLine("• Better LeagueClient detection");
            patchNotes.AppendLine("• Using new rewritten LoLA library");
            patchNotes.AppendLine("• Added runes & spells provider/source option");
            patchNotes.AppendLine("• Moved directory data from '\\AppData\\Local\\Temp\\LoLA' to '\\AppData\\Local\\Temp\\LoLA Data'");
            patchNotes.AppendLine("• Minor design changes (tooltip, button, etc)");

            patchNotes.AppendLine("• Fixed minimize to tray");
            patchNotes.AppendLine("• Fixed an issue where role selection reset after phase changed");
            patchNotes.AppendLine("• Fixed LoL Assist crashing issue when launching LeagueClient");
            patchNotes.AppendLine("• Fixed an issue where Phase Monitor won't update at initialize");

            PatchNotes = patchNotes.ToString();
        }

        private void hideUserControl(object p) => Utils.Animation.FadeOut(p as UserControl);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
