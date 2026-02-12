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
                LogService.LogError(ex, "DataService.SaveData");
                ToastService.Show("Lỗi lưu dữ liệu", "Không thể lưu dữ liệu ứng dụng. Vui lòng kiểm tra quyền ghi file.", Models.ToastType.Error);
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
            catch (Exception ex){ 
                LogService.LogError(ex, "DataService.LoadData");
                ToastService.Show("Lỗi đọc dữ liệu", "Không thể đọc dữ liệu ứng dụng. Vui lòng kiểm tra quyền đọc file.", Models.ToastType.Error);
                return new T(); 
            }
        }
    }
}
