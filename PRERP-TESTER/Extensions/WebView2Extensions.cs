using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;
using PRERP_TESTER.ViewModels;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Policy;
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

                try
                {
                    webView.Unloaded -= OnWebViewUnloaded;
                    webView.Unloaded += OnWebViewUnloaded;

                    var envTask = _envCache.GetOrAdd(accountId, id => CreateEnvironmentForAccount(id));

                    var env = await envTask;
                    await webView.EnsureCoreWebView2Async(env);
                    var context = webView.DataContext;

                    // Loading
                    webView.NavigationCompleted -= OnNavigationCompleted;
                    webView.NavigationCompleted += (s, args) =>
                    {
                        if (webView.DataContext is TabViewModel vm)
                        {
                            Application.Current.Dispatcher.Invoke(() => vm.IsLoading = false);
                        }
                        OnNavigationCompleted(s, args);
                    };
                    webView.CoreWebView2.NavigationStarting += (s, args) =>
                    {
                        if (webView.DataContext is TabViewModel vm)
                        {
                            Application.Current.Dispatcher.Invoke(() => vm.IsLoading = true);
                        }
                    };

                    // Favicon
                    webView.CoreWebView2.FaviconChanged += async (s, args) =>
                    {
                        if (webView.DataContext is TabViewModel vm)
                        {
                            string iconUri = webView.CoreWebView2.FaviconUri;
                            await Application.Current.Dispatcher.InvokeAsync(() => {
                                vm.FaviconUrl = iconUri;
                            });

                            // favicon base64
                            using (var faviconStream = await webView.CoreWebView2.GetFaviconAsync(CoreWebView2FaviconImageFormat.Png))
                            {
                                if (faviconStream != null && faviconStream.Length > 0)
                                {
                                    // Chuyển stream sang Base64 để lưu trữ dễ dàng trong JSON lịch sử
                                    byte[] bytes = new byte[faviconStream.Length];
                                    faviconStream.Read(bytes, 0, (int)faviconStream.Length);
                                    string base64Favicon = "data:image/png;base64," + Convert.ToBase64String(bytes);

                                    // Cập nhật vào history
                                    //MainViewModel.Instance.UpdateHistoryFavicon(webView.Source, base64Favicon);
                                }
                            }
                        }
                    };

                    // title change
                    webView.CoreWebView2.DocumentTitleChanged += (s, args) =>
                    {
                        if (webView.DataContext is TabViewModel vm)
                        {
                            string webTitle = webView.CoreWebView2.DocumentTitle;
                            if (!string.IsNullOrEmpty(webTitle))
                            {
                                vm.Title = webTitle;
                            }
                        }
                    };

                    // TabWeb contrl handle
                    if (webView.DataContext is TabViewModel vm)
                    {
                        string initialUrl = string.IsNullOrWhiteSpace(vm.Url) ? "about:blank" : vm.Url;
                        try
                        {
                            webView.Source = new Uri(initialUrl);
                        }
                        catch
                        {
                            webView.Source = new Uri("about:blank");
                        }

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
                                        string target = string.IsNullOrWhiteSpace(vm.Url) ? "about:blank" : vm.Url;
                                        webView.Source = new Uri(target);
                                    }
                                    catch (UriFormatException)
                                    {
                                        webView.Source = new Uri("https://www.google.com/search?q=" + Uri.EscapeDataString(vm.Url));
                                    }
                                    break;
                            }
                        };
                        webView.SourceChanged += (s, args) => {
                            vm.Url = webView.Source.ToString();
                        };

                        // New window requested
                        webView.CoreWebView2.NewWindowRequested += (s, args) =>
                        {
                            args.Handled = true;

                            string newUrl = args.Uri;

                            if (AccountViewModel.Instance != null)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    AccountViewModel.Instance.AddTabFromUrl(newUrl);
                                });
                            }
                        };
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogError(ex, "WebView2Extensions.OnAccountIDChanged");
                    // TODO: Hiển thị lỗi cho người dùng
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
                    webView.Dispose();

                    System.Diagnostics.Debug.WriteLine("WebView2 has been disposed successfully");
                }
                catch (Exception ex)
                {
                    LogService.LogError(ex, "WebView2Extensions.OnWebViewUnloaded");
                }
            }
        }

        private static async void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess || sender is not WebView2 webView) return;
            if (webView.DataContext is not TabViewModel vm) return;

            try
            {
                // Lịch sử truy cập
                using var stream = await webView.CoreWebView2.GetFaviconAsync(CoreWebView2FaviconImageFormat.Png);
                if (stream != null)
                {
                    byte[] bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, (int)stream.Length);
                    string base64 = "data:image/png;base64," + Convert.ToBase64String(bytes);
                    MainViewModel.Instance.AddHistory(webView.CoreWebView2.DocumentTitle, webView.Source.ToString(), base64);
                }

                string currentUrl = webView.Source.ToString();
                // Tự động đăng nhập
                if (currentUrl.Contains("landingpage") && currentUrl.Contains(GobalSetting.CurrentBaseUrl))
                {
                    string targetLink = (vm.UserAccount.Role == AccountRole.STUDENT)
                        ? GobalSetting.CurrentBaseUrl + "/sftraining/login?lang=vi"
                        : GobalSetting.CurrentBaseUrl + "/login?lang=vi";

                    string selectTypeScript = $@"
                                                (function() {{
                                                    var links = document.querySelectorAll('a.card-link_button');
                                                    for (var i = 0; i < links.length; i++) {{
                                                        if (links[i].href.indexOf('{targetLink}') !== -1) {{
                                                            links[i].click();
                                                            break;
                                                        }}
                                                    }}
                                                }})();";
                    await webView.CoreWebView2.ExecuteScriptAsync(selectTypeScript);
                }

                else if (currentUrl.Contains("/login") && currentUrl.Contains(GobalSetting.CurrentBaseUrl))
                {
                    string loginScript = $@"
                                            (function() {{
                                                var userField = document.getElementsByName('email_txt')[0];
                                                var passField = document.getElementsByName('password_txt')[0];
                                                var loginBtn = document.querySelector('input[type=""submit""]');

                                                if (userField && passField && userField.value === '') {{
                                                    userField.value = '{vm.UserAccount.Username}';
                                                    passField.value = '{vm.UserAccount.Password}';
                        
                                                    setTimeout(function() {{
                                                        if (loginBtn) loginBtn.click();
                                                    }}, 500);
                                                }}
                                            }})();";
                    await webView.CoreWebView2.ExecuteScriptAsync(loginScript);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "WebView2Extensions.OnNavigationCompleted");
            }

        }

        public static void RemoveFromCache(string accountId)
        {
            if (_envCache.TryRemove(accountId, out var task))
            {
                System.Diagnostics.Debug.WriteLine($"Removed environment cache for: {accountId}");
            }
        }

    }
}
