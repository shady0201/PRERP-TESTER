using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;
using PRERP_TESTER.Services;

namespace PRERP_TESTER.Helper
{
    public class Base64ToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string base64String && !string.IsNullOrEmpty(base64String))
            {
                try
                {
                    string pureBase64 = base64String.Contains(",") ? base64String.Split(',')[1] : base64String;
                    byte[] binaryData = System.Convert.FromBase64String(pureBase64);

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = new MemoryStream(binaryData);
                    bi.EndInit();
                    return bi;
                }
                catch (Exception ex){
                    LogService.LogError(ex, "Base64ToImageSourceConverter");
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}