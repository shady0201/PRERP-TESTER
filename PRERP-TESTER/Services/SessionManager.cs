using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace PRERP_TESTER.Services
{
    public class SessionManager
    {
        private readonly string _profilesRoot;
        private readonly ConcurrentDictionary<string, Lazy<Task<CoreWebView2Environment>>> _envCache = new();
        public SessionManager(string profilesRoot)
        {
            _profilesRoot = profilesRoot;
            Directory.CreateDirectory(_profilesRoot);
        }

        public Task<CoreWebView2Environment> GetEnvironmentAsync(string accountId)
        {
            var lazy = _envCache.GetOrAdd(accountId, id => new Lazy<Task<CoreWebView2Environment>>(() => CreateEnvAsync(id)));
            return lazy.Value;
        }

        private async Task<CoreWebView2Environment> CreateEnvAsync(string accountId)
        {
            var userDataFolder = Path.Combine(_profilesRoot, accountId);
            Directory.CreateDirectory(userDataFolder);

            // options nếu cần (proxy, args...)
            var options = new CoreWebView2EnvironmentOptions();

            return await CoreWebView2Environment.CreateAsync(
                browserExecutableFolder: null,
                userDataFolder: userDataFolder,
                options: options
            );
        }
    }
}
