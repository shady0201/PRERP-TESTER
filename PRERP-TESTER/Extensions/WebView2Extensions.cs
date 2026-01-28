using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using PRERP_TESTER.ViewModels;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace PRERP_TESTER.Extensions
{
    public static class WebView2Extensions
    {
        private static readonly ConcurrentDictionary<string, Task<CoreWebView2Environment>> _envCache = new();

        public static readonly DependencyProperty AccountIDProperty =
            DependencyProperty.RegisterAttached("AccountID", typeof(string), typeof(WebView2Extensions),
                new PropertyMetadata(null, OnAccountIDChanged));

        public static string GetAccountID(DependencyObject obj) => (string)obj.GetValue(AccountIDProperty);
        public static void SetAccountID(DependencyObject obj, string value) => obj.SetValue(AccountIDProperty, value);

        private static async void OnAccountIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebView2 webView && e.NewValue is string accountId && !string.IsNullOrEmpty(accountId))
            {
                webView.Unloaded -= OnWebViewUnloaded; // Đảm bảo không đăng ký trùng lặp
                webView.Unloaded += OnWebViewUnloaded;
                try
                {
                    var envTask = _envCache.GetOrAdd(accountId, id => CreateEnvironmentForAccount(id));

                    var env = await envTask;
                    await webView.EnsureCoreWebView2Async(env);
                    var context = webView.DataContext;

                    if (webView.DataContext is TabViewModel vm && vm.Url != null)
                    {
                        webView.Source = new Uri(vm.Url.ToString());

                        vm.NavigationRequested += (action) =>
                        {
                            switch (action)
                            {
                                case "Back": if (webView.CoreWebView2.CanGoBack) webView.CoreWebView2.GoBack(); break;
                                case "Forward": if (webView.CoreWebView2.CanGoForward) webView.CoreWebView2.GoForward(); break;
                                case "Reload": webView.CoreWebView2.Reload(); break;
                                case "GoTo":
                                    try
                                    {
                                        webView.Source = new Uri(vm.Url);
                                    }
                                    catch (UriFormatException)
                                    {
                                        webView.Source = new Uri("https://www.google.com/search?q=" + Uri.EscapeDataString(vm.Url));
                                    }
                                    break;
                            }
                        };

                        // Cập nhật ngược lại URL vào TextBox khi người dùng click link trên web
                        webView.SourceChanged += (s, args) => {
                            vm.Url = webView.Source.ToString();
                        };
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WebView2 Init Error: {ex.Message}");
                }
            }
        }
        private static Task<CoreWebView2Environment> CreateEnvironmentForAccount(string accountId)
        {
            string userDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sessions", $"Account_{accountId}");

            if (!Directory.Exists(userDataFolder))
                Directory.CreateDirectory(userDataFolder);

            return CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
        }


        // tránh memory leak
        private static void OnWebViewUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is WebView2 webView)
            {
                webView.Unloaded -= OnWebViewUnloaded;
                try
                {
                    if (webView.CoreWebView2 != null)
                    {
                        webView.CoreWebView2.Stop();
                    }

                    // 2. Dispose WebView2
                    // Lưu ý: Việc Dispose sẽ đóng trình xử lý trình duyệt (Browser Process) liên quan nếu là tab cuối
                    webView.Dispose();

                    System.Diagnostics.Debug.WriteLine("WebView2 has been disposed successfully.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disposing WebView2: {ex.Message}");
                }
            }
        }
    }
}
