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

                if (webView.CoreWebView2 != null)
                {
                    ToastService.Show("Web view: OnAccountIDChanged", "", ToastType.Information);
                }

                try
                {

                    var envTask = _envCache.GetOrAdd(accountId, id => CreateEnvironmentForAccount(id));
                    var env = await envTask;

                    await webView.EnsureCoreWebView2Async(env);

                    // Unload webview: show, hide in view, tab change, etc.
                    webView.Unloaded -= OnWebViewUnloaded;
                    webView.Unloaded += OnWebViewUnloaded;
                    // Loading
                    webView.NavigationCompleted -= OnNavigationCompleted;
                    webView.NavigationCompleted += OnNavigationCompleted;



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

                    // Title change
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

                    // TabWeb control handle
                    if (webView.DataContext is TabViewModel vm)
                    {
                        // Url handle
                        string initialUrl = string.IsNullOrWhiteSpace(vm.Url) ? "about:blank" : vm.Url;
                        try
                        {
                            webView.Source = new Uri(initialUrl);
                        }
                        catch
                        {
                            webView.Source = new Uri("about:blank");
                        }

                        // controls handle
                        Action<string> navigationHandler = (action) =>
                        {
                            if (webView == null || webView.CoreWebView2 == null) return;

                            try
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
                            }
                            catch (ObjectDisposedException ex) {
                                LogService.LogError(ex, "WebView2Extensions: navigationHandler");
                            }
                        };
                        vm.NavigationRequested += navigationHandler;
                        webView.Tag = navigationHandler;

                        // ĐĂNG KÝ SỰ KIỆN ĐÓNG TAB ĐỂ DISPOSE
                        Action<TabViewModel>? closeHandler = null;
                        closeHandler = (tab) =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                vm.OnTabClosed -= closeHandler;
                                vm.Cleanup();

                                try
                                {
                                    webView.CoreWebView2?.Stop();
                                    webView.Dispose();
                                }
                                catch (Exception ex){
                                    LogService.LogError(ex, "WebView2Extensions.TabCloseDispose");
                                }
                            });
                        };
                        vm.OnTabClosed += closeHandler;


                        // Binding url to ViewModel
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
                    ToastService.Show("Lỗi WebView2", "Không thể khởi tạo WebView2 cho tài khoản này.", ToastType.Error);
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

                if (webView.DataContext is TabViewModel vm && webView.Tag is Action<string> handler)
                {
                    //vm.NavigationRequested -= handler;
                    //ToastService.Show("Web view: OnWebViewUnloaded", vm.Url, ToastType.Information);
                }
            }
        }

        private static async void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess || sender is not WebView2 webView) return;
            if (webView.DataContext is not TabViewModel vm) return;

            try
            {
                // Loading
                Application.Current.Dispatcher.Invoke(() => vm.IsLoading = false);
                Application.Current.Dispatcher.Invoke(() => vm.IsDefaultPageVisible = false);

                // Lưu lịch sử
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
                ToastService.Show("Web view: RemoveFromCache", $"Removed environment cache for: {accountId}", ToastType.Error);
            }
        }

    }
}
