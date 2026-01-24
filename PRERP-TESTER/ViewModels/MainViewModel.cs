using System.Collections.ObjectModel;
using System.Linq;
using PRERP_TESTER.Models;
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

        public MainViewModel( WebViewService webViewService)
        {
            // Khởi tạo các Service dùng chung
            _webViewService = webViewService;
            _themeService = new ThemeService();
        }

        public void LoadModulesFromDatabase()
        {
            // 1. Lấy danh sách Entity thô từ Models
            List<ModuleEntity> moduleEntities = _databaseService.GetAllModules();
            List<Account> accountEntities = _databaseService.GetAllAccounts();

            foreach (var mEntity in moduleEntities)
            {
                // 2. Chuyển đổi Entity thành ViewModel để hiển thị
                var vm = new ModuleViewModel(_webViewService, mEntity, accountEntities);
                ActiveModules.Add(vm);
            }
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