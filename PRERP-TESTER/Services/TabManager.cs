using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PRERP_TESTER.Models;
using System.Collections.ObjectModel;
using Microsoft.Web.WebView2.Wpf;

namespace PRERP_TESTER.Services
{
    public partial class TabManager : ObservableObject
    {
        private readonly SessionManager _sessions;
        private readonly JsonStorageService _storage;
        private readonly Action<object?> _setHostContent;

        public string ModuleId { get; set; } = "";
        public string AccountId { get; set; } = "";

        public ObservableCollection<TabWebItem> Tabs { get; } = new();
        private readonly Dictionary<string, TabRuntime> _runtime = new();

        [ObservableProperty]
        private TabWebItem? selectedTab;

        public TabManager(SessionManager sessions, JsonStorageService storage, Action<object?> setHostContent)
        {
            _sessions = sessions;
            _storage = storage;
            _setHostContent = setHostContent;
        }

        private string StorageFileName => $"tabs_{ModuleId}__{AccountId}.json";

        public TabWebItem OpenTab(string? url = null, string? title = null)
        {
            var tab = new TabWebItem
            {
                AccountId = AccountId,
                ModuleId = ModuleId,
                Url = string.IsNullOrWhiteSpace(url) ? "about:blank" : url!,
                Title = string.IsNullOrWhiteSpace(title) ? "New Tab" : title!,
            };

            Tabs.Add(tab);
            _runtime[tab.Id] = new TabRuntime(tab);

            SelectedTab = tab;
            _ = ActivateTabAsync(tab);

            return tab;
        }

        public async Task ActivateTabAsync(TabWebItem tab)
        {
            SelectedTab = tab;
            tab.LastActiveUtc = DateTime.UtcNow;

            var rt = _runtime[tab.Id];
            if (!rt.IsWebViewCreated)
            {
                var env = await _sessions.GetEnvironmentAsync(AccountId);

                var wv = new WebView2();
                await wv.EnsureCoreWebView2Async(env);

                // restore url
                if (!string.IsNullOrWhiteSpace(tab.Url) && tab.Url != "about:blank")
                    wv.CoreWebView2.Navigate(tab.Url);

                // sync state
                wv.CoreWebView2.SourceChanged += (_, __) =>
                {
                    var src = wv.Source?.ToString();
                    if (!string.IsNullOrWhiteSpace(src))
                    {
                        tab.Url = src!;
                        tab.LastActiveUtc = DateTime.UtcNow;
                    }
                };

                wv.CoreWebView2.DocumentTitleChanged += (_, __) =>
                {
                    tab.Title = wv.CoreWebView2.DocumentTitle ?? tab.Title;
                };

                rt.AttachWebView(wv);
            }

            _setHostContent(rt.WebView);
        }

        public void CloseTab(TabWebItem tab)
        {
            if (!_runtime.TryGetValue(tab.Id, out var rt)) return;

            if (ReferenceEquals(SelectedTab, tab))
                _setHostContent(null);

            rt.DisposeWebView();
            _runtime.Remove(tab.Id);
            Tabs.Remove(tab);

            SelectedTab = Tabs.LastOrDefault();
            if (SelectedTab is not null)
                _ = ActivateTabAsync(SelectedTab);
        }

        public async Task SaveWorkspaceAsync()
        {
            // lưu state thôi, không lưu runtime
            await _storage.SaveAsync(StorageFileName, Tabs.ToList());
        }

        public async Task RestoreWorkspaceAsync()
        {
            Tabs.Clear();
            _runtime.Clear();

            var list = await _storage.LoadAsync<List<TabWebItem>>(StorageFileName) ?? new();

            foreach (var t in list)
            {
                // đảm bảo đúng workspace
                t.ModuleId = ModuleId;
                t.AccountId = AccountId;

                Tabs.Add(t);
                _runtime[t.Id] = new TabRuntime(t);
            }

            SelectedTab = Tabs.LastOrDefault();
        }

    }
}
