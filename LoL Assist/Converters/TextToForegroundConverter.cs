using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LoL_Assist_WAPP.Converters
{
    public class TextToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => value.ToString().ToLower() switch
            {
                _ when value.ToString().ToLower() == "declined" || value.ToString().ToLower() == "disconnected" 
                || value.ToString().ToLower() == "invalid build config" || value.ToString().ToLower() == "none"
                => (SolidColorBrush)Application.Current.Resources["RedBrush"],  // Red

                _ when value.ToString().ToLower() == "auto accept is disabled" || value.ToString().ToLower() == "connecting"
                => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffc40c")), // Yellow

                "accepted" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#29ab87")),  // Green

                _ => (SolidColorBrush)Application.Current.Resources["FontPrimaryBrush"] // White
            };

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
