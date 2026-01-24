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

        // Truyền Service vào bộ não MainViewModel
        var mainVM = new MainViewModel();

        var shell = new ShellWindow();
        shell.DataContext = mainVM;
        shell.Show();
    }
}

