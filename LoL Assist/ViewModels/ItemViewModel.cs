using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoL_Assist_WAPP.ViewModels
{
    public class ItemViewModel: ViewModelBase
    {
        private int _originIndex;
        public int OriginIndex
        {
            get => _originIndex;
            set
            {
                if (_originIndex != value)
                {
                    _originIndex = value;
                    OnPropertyChanged(nameof(OriginIndex));
                }
            }
        }

        private int _actualIndex;
        public int ActualIndex
        {
            get => _actualIndex;
            set
            {
                if (_actualIndex != value)
                {
                    _actualIndex = value;
                    OnPropertyChanged(nameof(ActualIndex));
                }
            }
        }

        public object _content;
        public object Content
        {
            get => _content;
            set
            {
                if(_content != value)
                {
                    _content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }
    }
}
