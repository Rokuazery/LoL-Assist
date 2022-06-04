using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LoL_Assist_WAPP.Converters
{
    public class TextToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch(value.ToString())
            {
                case "Declined":
                case "Disconnected":
                case "Invalid Build Config":
                    return (SolidColorBrush)Application.Current.Resources["RedBrush"]; // Red
                case "Accepted":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#29ab87")); // Green
                case "Auto Accept is disabled":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffc40c")); // Yellow
                default:
                    return (SolidColorBrush)Application.Current.Resources["FontPrimaryBrush"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
