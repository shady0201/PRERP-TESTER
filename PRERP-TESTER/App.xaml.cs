using PRERP_TESTER.Services;
using PRERP_TESTER.ViewModels;
using PRERP_TESTER.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PRERP_TESTER;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        this.DispatcherUnhandledException += (s, args) =>
        {
            LogService.LogError(args.Exception, "Global.DispatcherUnhandledException");
            MessageBox.Show("Ứng dụng gặp lỗi không xác định. Chi tiết lỗi đã được lưu vào Log.", "Lỗi hệ thống");
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            LogService.LogError((Exception)args.ExceptionObject, "Global.UnhandledException");
        };
        var shell = new MainWindow();
        shell.Show();
    }
}

