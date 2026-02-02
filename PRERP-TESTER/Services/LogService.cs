using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Services
{
    public static class LogService
    {
        private static readonly string BaseFolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrerpTester");

        private static readonly string LogsFolderPath = Path.Combine(BaseFolderPath, "logs");

        public static void LogError(Exception ex, string context = "")
        {
            try
            {
                if (!Directory.Exists(LogsFolderPath))
                {
                    Directory.CreateDirectory(LogsFolderPath);
                }

                string fileName = $"{DateTime.Now:yyyy-MM-dd}.log";
                string filePath = Path.Combine(LogsFolderPath, fileName);

                string logContent = $"------------------------------------------------\n" +
                                    $"Time: {DateTime.Now:HH:mm:ss}\n" + // Chỉ cần giờ trong file ngày
                                    $"Location: {context}\n" +
                                    $"Error: {ex.Message}\n" +
                                    $"Stack Trace: {ex.StackTrace}\n";

                if (ex.InnerException != null)
                {
                    logContent += $"Inner Exception: {ex.InnerException.Message}\n";
                }

                File.AppendAllText(filePath, logContent + Environment.NewLine);
            }
            catch
            {
                
            }
        }
    }
}
