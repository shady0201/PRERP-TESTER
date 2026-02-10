using PRERP_TESTER.Views.Dialogs;
using System.Windows;

namespace PRERP_TESTER.Services
{
    public static class DialogService
    {
        public static bool ShowConfirm(string title, string message)
        {
            var dialog = new ConfirmDialog(title, message);

            dialog.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive)
                           ?? Application.Current.MainWindow;

            // Hiển thị và trả về kết quả
            return dialog.ShowDialog() ?? false;
        }
    }
}