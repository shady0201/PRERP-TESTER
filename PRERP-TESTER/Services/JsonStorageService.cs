using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PRERP_TESTER.Services
{
    public sealed class JsonStorageService
    {
        private readonly string _dataDir;
        private static readonly JsonSerializerOptions Opt = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public JsonStorageService(string dataDir)
        {
            _dataDir = dataDir;
            Directory.CreateDirectory(_dataDir);
        }

        public async Task SaveAsync<T>(string fileName, T data, CancellationToken ct = default)
        {
            var path = Path.Combine(_dataDir, fileName);
            await using var fs = File.Create(path);
            await JsonSerializer.SerializeAsync(fs, data, Opt, ct);
        }

        public async Task<T?> LoadAsync<T>(string fileName, CancellationToken ct = default)
        {
            var path = Path.Combine(_dataDir, fileName);
            if (!File.Exists(path)) return default;

            await using var fs = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<T>(fs, Opt, ct);
        }
    }
}
