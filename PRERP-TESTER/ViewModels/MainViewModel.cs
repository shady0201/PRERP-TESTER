using System.Collections.ObjectModel;
using System.Linq;
using PRERP_TESTER.Services;

namespace PRERP_TESTER.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly WebViewService _webViewService;
        private readonly ThemeService _themeService;

        // Danh sách các Module đang mở (thanh menu bên trái)
        public ObservableCollection<ModuleViewModel> ActiveModules { get; set; } = new ObservableCollection<ModuleViewModel>();

        // Module hiện tại đang được chọn để hiển thị Workspace
        private ModuleViewModel _currentModule;
        public ModuleViewModel CurrentModule
        {
            get => _currentModule;
            set => SetProperty(ref _currentModule, value);
        }

        public MainViewModel()
        {
            // Khởi tạo các Service dùng chung
            _webViewService = new WebViewService();
            _themeService = new ThemeService();

            // Giả lập thêm 2 Module động khi khởi động
            CreateNewModule("Hệ Thống Đào Tạo");
            CreateNewModule("Quản Lý Thanh Toán");
        }

        public void CreateNewModule(string name)
        {
            // Tạo một instance mới của ModuleViewModel
            // Mỗi instance này sẽ giữ các WebView riêng của nó
            var newModule = new ModuleViewModel(_webViewService);
            ActiveModules.Add(newModule);

            // Tự động chọn module vừa tạo
            CurrentModule = newModule;
        }

        // Logic đổi Theme Dark/Light
        private bool _isDarkMode;
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (SetProperty(ref _isDarkMode, value))
                {
                    _themeService.SetTheme(value);
                }
            }
        }
    }
}