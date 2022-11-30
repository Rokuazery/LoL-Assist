using LoLA.Networking.WebWrapper.DataDragon;
using LoLA.Networking.WebWrapper.DataDragon.Data;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.IO;
using System;

namespace LoL_Assist_WAPP.Converters
{
    public class ChampionToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = null;
            var path = DataDragonWrapper.GetChampionImagePath(Converter.ChampionNameToId(value.ToString()));

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    result = image;
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
