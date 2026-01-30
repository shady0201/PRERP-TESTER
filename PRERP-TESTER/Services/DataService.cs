using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Services
{
    public static class DataService
    {
        private static readonly string FolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrerpTester");
        private static readonly string FilePath = Path.Combine(FolderPath, "data.json");

        public static void SaveData(object data)
        {
            try
            {
                if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                
            }

        }

        public static T LoadData<T>() where T : new()
        {
            try
            {
                if (!File.Exists(FilePath)) return new T();

                string json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<T>(json) ?? new T();
            }
            catch { return new T(); }
        }
    }
}
