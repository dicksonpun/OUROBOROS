using MaterialDesignThemes.Wpf;
using OUROBOROS.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace OUROBOROS.Converters
{
    public class IsUserViewToWarningTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUserView = (bool)value;
 
            if (isUserView)
            {
                // USER
                return "WARNING: USER VIEW DETECTED";
            }
            else
            {
                // NONUSER
                return "WARNING: NONUSER VIEW DETECTED";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
