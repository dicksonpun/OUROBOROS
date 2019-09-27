using MaterialDesignThemes.Wpf;
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
    public class PackIconLetterYBoxToButtonIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((PackIconKind)value)
            {
                case PackIconKind.LetterYBox:
                    return PackIconKind.MinusNetwork;
                default:
                    return PackIconKind.PlusNetwork;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
