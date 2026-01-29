using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PRERP_TESTER.Converters
{
    public class SubtractValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double actualWidth && double.TryParse(parameter?.ToString(), out double subtractValue))
            {
                // Trả về chiều rộng sau khi đã trừ đi khoảng trống cho nút "+"
                return Math.Max(0, actualWidth - subtractValue);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
