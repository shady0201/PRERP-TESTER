using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PRERP_TESTER.Services
{
    public class WebViewService
    {
        // Session cache for multiple accounts
        private readonly Dictionary<string, CoreWebView2Environment> _environments = new Dictionary<string, CoreWebView2Environment>();

        // Cache account path
        private readonly string _baseDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserSessions");

        // Lấy hoặc tạo mới CoreWebView2Environment theo AccountID ID
        public async Task<CoreWebView2Environment> GetEnvironmentAsync(string accountId)
        {
            if (_environments.ContainsKey(accountId))
            {
                return _environments[accountId];
            }
            string accountDataPath = Path.Combine(_baseDataPath, $"Acc_{accountId}");
            if (!Directory.Exists(accountDataPath))
            {
                Directory.CreateDirectory(accountDataPath);
            }
            var env = await CoreWebView2Environment.CreateAsync(null, accountDataPath);
            _environments[accountId] = env;

            return env;
        }

        public async Task InitializeWebViewAsync(WebView2 webView, string accountId, bool isDarkMode)
        {
            var env = await GetEnvironmentAsync(accountId);
            await webView.EnsureCoreWebView2Async(env);

            // Ép giao diện trình duyệt sang Dark hoặc Light
            webView.CoreWebView2.Profile.PreferredColorScheme = isDarkMode
                ? CoreWebView2PreferredColorScheme.Dark
                : CoreWebView2PreferredColorScheme.Light;
        }
    }
}