using System;
using System.ComponentModel;

namespace LoL_Assist_WAPP.ViewModel
{
    internal class MessageBoxViewModel : INotifyPropertyChanged
    {
        public Action action { get; set; }
        public string message { get; set; }
        public string title { get; set; }
        public double width { get; set; }
        public double height { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
