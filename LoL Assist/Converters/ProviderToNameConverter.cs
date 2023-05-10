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
    public class ProviderToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var provider = (Provider)value;
            switch(provider)
            {
                case Provider.UGG:
                    return "U.GG";
                case Provider.OPGG:
                    return "OP.GG";
                case Provider.METAsrc:
                    return "METASrc.com";
                case Provider.Local:
                    return "Local";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
