using LoLA.Networking.WebWrapper.DataDragon.Data;
using System.Globalization;
using System.Windows.Data;
using System;

namespace LoL_Assist_WAPP.Converters
{
    public class RunePerkToolTipConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Converter.GetPerkDescriptionByName(value.ToString());

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
