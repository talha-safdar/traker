using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UltraAnalysis.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolToVisibilityCollapsed : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                bool boolValue = (bool)value;

                if (boolValue == true)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                // If input is not bool, return Collapsed by default
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                Visibility visibility = (Visibility)value;

                if (visibility == Visibility.Visible)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // If input is not Visibility type, return false by default
                return false;
            }
        }
        #endregion
    }
}