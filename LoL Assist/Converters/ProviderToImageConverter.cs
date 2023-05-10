using LoL_Assist_WAPP.Utils;
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
    public class ProviderToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var provider = (Provider)value;
            switch (provider)
            {
                case Provider.UGG:
                    return Helper.ImageSrc("ugg");
                case Provider.OPGG:
                    return Helper.ImageSrc("opgg");
                case Provider.METAsrc:
                    return Helper.ImageSrc("metasrc");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
