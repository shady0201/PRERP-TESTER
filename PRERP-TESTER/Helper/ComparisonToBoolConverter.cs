using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PRERP_TESTER.Helper
{
    public class ComparisonToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isChecked && isChecked ? parameter : Binding.DoNothing;
        }
    }
}