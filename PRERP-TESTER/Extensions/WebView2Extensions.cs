using PRERP_TESTER.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRERP_TESTER.Extensions
{
    public class WebView2Extensions
    {
        public static readonly DependencyProperty AccountIDProperty =
        DependencyProperty.RegisterAttached("AccountID", typeof(string), typeof(WebView2Extensions),
            new PropertyMetadata(null, OnAccountIDChanged));

        public static string GetAccountID(DependencyObject obj) => (string)obj.GetValue(AccountIDProperty);
        public static void SetAccountID(DependencyObject obj, string value) => obj.SetValue(AccountIDProperty, value);

        private static async void OnAccountIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Microsoft.Web.WebView2.Wpf.WebView2 webView && e.NewValue is string accountId)
            {

                string userDataFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sessions", accountId);

 
                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);

                await webView.EnsureCoreWebView2Async(env);

                // Sau khi khởi tạo xong mới nạp URL (để đảm bảo ăn session)
                if (webView.DataContext is TabViewModel vm)
                {
                    webView.Source = new Uri(vm.Url);
                }
            }
        }
    }
}
