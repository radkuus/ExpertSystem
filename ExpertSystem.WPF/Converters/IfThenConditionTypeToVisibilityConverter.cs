using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ExpertSystem.WPF.Converters
{
    public class IfThenConditionTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? type = value as string;
            string? param = parameter as string;

            if (type == null || param == null) 
            {
                return Visibility.Collapsed;
            }
            if (param == "if/and" && (type == "If" || type == "and"))
            {
                return Visibility.Visible;
            }    
            if (param == "then" && type == "then")
            {
                return Visibility.Visible;
            }
                
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
