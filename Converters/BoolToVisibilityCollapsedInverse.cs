using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Traker.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityCollapsedInverse : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Inverse Logic: true -> Collapsed, false -> Visible
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            // Default fallback if input is not a bool
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                // Inverse Logic: Collapsed -> true, Visible -> false
                return visibility == Visibility.Collapsed;
            }

            return false;
        }
        #endregion
    }
}