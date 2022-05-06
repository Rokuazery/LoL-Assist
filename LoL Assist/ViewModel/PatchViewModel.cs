using LoL_Assist_WAPP.Model;
using System.ComponentModel;
using System.Text;

namespace LoL_Assist_WAPP.ViewModel
{
    internal class PatchViewModel : INotifyPropertyChanged
    {
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
            DoNotShowPatchNote = ConfigModel.config.DoNotShowPatch;
            SetPatchNotes();
            Title = $"What's New in v{ConfigModel.version}";
        }

        void SetPatchNotes()
        {
            StringBuilder patchNotes = new StringBuilder();
            patchNotes.AppendLine("• Added a What's New panel");
            patchNotes.AppendLine("• Minor improvement for R&P(Runes & Spells) Editor Window UI"); 
            patchNotes.AppendLine("• Added a reset config button on the settings");
            patchNotes.AppendLine("• Moved the clear cache button to miscellaneous section");
            patchNotes.AppendLine("• Fixed an issue where LoLA failed to fetch rune data");
            patchNotes.AppendLine("• Fixed an issue where LoL Assist shows an error when Auto Spells/Auto Runes are disabled");

            PatchNotes = patchNotes.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
