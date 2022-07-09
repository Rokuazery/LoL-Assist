using System.Windows.Data;
using System;


namespace LoL_Assist_WAPP.Converters
{
    public class EmptyToNoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => string.IsNullOrEmpty(value?.ToString()) ? "None" : value?.ToString();

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
