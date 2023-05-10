using LoLA.Data.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LoL_Assist_WAPP.Converters
{
    public class ProviderToAvailabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var provider = (Provider)value;
            switch (provider)
            {
                case Provider.UGG:
                    return "NORMAL - ARAM";
                case Provider.OPGG:
                    return "NORMAL - ARAM - URF";
                case Provider.METAsrc:
                    return "NORMAL - ARAM - ARURF - URF";
                case Provider.Local:
                    return null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
