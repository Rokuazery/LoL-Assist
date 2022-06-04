using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LoL_Assist_WAPP.ViewModel
{
    public class RoleSelectViewModel : INotifyPropertyChanged
    {
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

        private ICommand selectRoleCommand;
        public ICommand SelectRoleCommand
        {
            get
            {
                if (selectRoleCommand == null)
                    selectRoleCommand = new Command(o => SelectRole(o.ToString()));

                return selectRoleCommand;
            }
        }

        public string Role { get; set; } = string.Empty;

        public string GetRole
        {
            get { return Role; }
        }

        public Action ImportAction { get; set; }

        private async void SelectRole(string role)
        {
            Role = role;
            await Task.Run(() => { ImportAction?.Invoke(); }); 
        }

        public RoleSelectViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
