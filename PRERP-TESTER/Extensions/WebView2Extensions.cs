using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using PRERP_TESTER.Models;
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
                webView.Unloaded -= OnWebViewUnloaded;
                webView.Unloaded += OnWebViewUnloaded;

                webView.NavigationCompleted -= OnNavigationCompleted;

                try
                {
                    var envTask = _envCache.GetOrAdd(accountId, id => CreateEnvironmentForAccount(id));

                    var env = await envTask;
                    await webView.EnsureCoreWebView2Async(env);
                    var context = webView.DataContext;
                    webView.NavigationCompleted += OnNavigationCompleted;

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
                    System.Diagnostics.Debug.WriteLine($"Error disposing WebView2: {ex.Message}");
                }
            }
        }

        private static async void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess || sender is not WebView2 webView) return;
            if (webView.DataContext is not TabViewModel vm) return;

            string currentUrl = webView.Source.ToString();

            try
            {
                if (currentUrl.Contains("landingpage") && currentUrl.Contains(GobalSetting.CurrentBaseUrl))
                {
                    string targetLink = (vm.UserAccount.Stype == "STUDENT")
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
                System.Diagnostics.Debug.WriteLine($"Auto-Login Script Error: {ex.Message}");
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
