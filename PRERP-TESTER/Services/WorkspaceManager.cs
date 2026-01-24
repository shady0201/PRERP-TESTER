using CommunityToolkit.Mvvm.ComponentModel;
using PRERP_TESTER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Services
{
    public class WorkspaceManager
    {
        private readonly SessionManager _sessions;
        private readonly JsonStorageService _storage;
        private readonly Action<object?> _setHostContent;

        // tất cả workspace (module+account)
        private readonly Dictionary<WorkspaceKey, TabManager> _tabManagers = new();

        private TabManager? CurrentTabs;

        public WorkspaceManager(SessionManager sessions, JsonStorageService storage, Action<object?> setHostContent)
        {
            _sessions = sessions;
            _storage = storage;
            _setHostContent = setHostContent;
        }

        public async Task SwitchWorkspaceAsync(string moduleId, string accountId)
        {
            var key = new WorkspaceKey(moduleId, accountId);

            if (!_tabManagers.TryGetValue(key, out var tm))
            {
                tm = new TabManager(_sessions, _storage, _setHostContent)
                {
                    ModuleId = moduleId,
                    AccountId = accountId
                };

                _tabManagers[key] = tm;

                // restore tabs theo workspace (nếu có)
                await tm.RestoreWorkspaceAsync();
                if (tm.Tabs.Count == 0)
                    tm.OpenTab(url: "https://prerp.bmtu.edu.vn/"); // hoặc trang default theo module
            }

            CurrentTabs = tm;

            // show selected tab của workspace đó
            if (tm.SelectedTab is not null)
                await tm.ActivateTabAsync(tm.SelectedTab);
        }

        public async Task SaveAllAsync()
        {
            foreach (var tm in _tabManagers.Values)
                await tm.SaveWorkspaceAsync();
        }
    }
}
