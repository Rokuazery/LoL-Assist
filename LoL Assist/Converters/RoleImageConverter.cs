using LoL_Assist_WAPP.Models;
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
    public class RoleImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var role = (Role)value;
            if(role == Role.RECOMENDED) return $"{ConfigModel.RESOURCE_PATH}fill.png";
            return $"{ConfigModel.RESOURCE_PATH}{role.ToString().Replace("BOTTOM", "adc")}.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
