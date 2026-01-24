using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;

namespace PRERP_TESTER.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly WebViewService _webViewService;

        // Giả lập Database chứa toàn bộ Account của hệ thống
        private List<Account> _allSystemAccounts;

        // Danh sách các Module đang mở
        public ObservableCollection<ModuleViewModel> ActiveModules { get; set; }

        // Module đang được chọn để hiển thị
        private ModuleViewModel _currentModule;
        public ModuleViewModel CurrentModule
        {
            get => _currentModule;
            set => SetProperty(ref _currentModule, value);
        }

        public ICommand CreateModuleCommand { get; }

        public MainViewModel()
        {
            // 1. Khởi tạo Service (Dùng chung cho cả App)
            _webViewService = new WebViewService();
            ActiveModules = new ObservableCollection<ModuleViewModel>();

            // 2. Load dữ liệu giả lập (System Accounts)
            LoadMockSystemAccounts();

            // 3. Tự động tạo một Module mẫu khi mở App
            CreateDemoModule();

            // Command tạo module mới (nếu cần)
            CreateModuleCommand = new RelayCommand(CreateDemoModule);
        }

        private void LoadMockSystemAccounts()
        {
            // Dựa trên class Account trong file classes.txt
            _allSystemAccounts = new List<Account>
            {
                new Account
                {
                    Id = "acc_admin",
                    Username = "admin_user",
                    DisplayName = "Administrator",
                    Role = new Role { Name = "Admin" }
                },
                new Account
                {
                    Id = "acc_teacher",
                    Username = "teacher_ha",
                    DisplayName = "Nguyễn Văn Hà",
                    Role = new Role { Name = "Teacher" }
                },
                 new Account
                {
                    Id = "acc_student",
                    Username = "student_linh",
                    DisplayName = "Trần Mỹ Linh",
                    Role = new Role { Name = "Student" }
                }
            };
        }

        private void CreateDemoModule()
        {
            // --- TẠO MOCK DATA ĐÚNG CẤU TRÚC ENTITY ---

            // Bước 1: Tạo ModuleEntity
            var moduleEntity = new ModuleEntity
            {
                Name = $"Module Test {ActiveModules.Count + 1}",
                Description = "Kịch bản kiểm thử tự động hệ thống đào tạo",
                // Bước 2: Tạo mảng AccountTabs (Cấu hình Account tham gia Module)
                AccountTabs = new AccountTab[]
                {
                    // Cấu hình cho Account Admin
                    new AccountTab
                    {
                        AccountId = "acc_admin", // Phải khớp với Id trong _allSystemAccounts
                        TabWebItems = new TabWeb[] // Danh sách TabWeb (Title, Url)
                        {
                            new TabWeb { Title = "Q.Lý Hệ Thống", Url = "https://google.com" },
                            new TabWeb { Title = "Logs", Url = "https://bing.com" }
                        }
                    },
                    // Cấu hình cho Account Teacher
                    new AccountTab
                    {
                        AccountId = "acc_teacher",
                        TabWebItems = new TabWeb[]
                        {
                            new TabWeb { Title = "Chấm điểm", Url = "https://stackoverflow.com" }
                        }
                    }
                }
            };

            // Bước 3: Khởi tạo ModuleViewModel
            // Truyền vào: Service, Entity vừa tạo, và Danh sách Account tổng để lookup tên
            var moduleVM = new ModuleViewModel(_webViewService, moduleEntity, _allSystemAccounts);

            // Bước 4: Đưa vào danh sách hiển thị
            ActiveModules.Add(moduleVM);
            CurrentModule = moduleVM;
        }
    }
}