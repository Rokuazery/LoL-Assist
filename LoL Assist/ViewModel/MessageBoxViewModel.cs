using System;
using System.ComponentModel;

namespace LoL_Assist_WAPP.ViewModel
{
    internal class MessageBoxViewModel : INotifyPropertyChanged
    {
        public Action Action { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
