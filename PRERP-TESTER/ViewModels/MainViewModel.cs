using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;
using PRERP_TESTER.Views.Dialogs;

namespace PRERP_TESTER.ViewModels
{
    public class MainViewModel
    {
        // service
        private readonly WebViewService _webViewService;
           
        // data
        private List<Account> Accounts;
        public ObservableCollection<ModuleViewModel> Modules { get; set; }
        public ModuleViewModel CurrentModule { get; set; }

        // settings
        public bool IsDarkMode { get; set; } = false;

        // command
        public ICommand CreateModuleCommand { get; }

        public MainViewModel()
        {
            _webViewService = new WebViewService();
            Modules = new ObservableCollection<ModuleViewModel>();

            LoadMockSystemAccounts();
            CreateDemoModule();

            // Command list
            CreateModuleCommand = new RelayCommand(ExecuteCreateModule);
        }

        private void ExecuteCreateModule()
        {
            var dialog = new AddModuleDialog { Owner = Application.Current.MainWindow };

            if (dialog.ShowDialog() == true)
            {
               // Khởi tạo Entity chuẩn theo classes.txt 
                var newEntity = new ModuleEntity
                {
                    Name = dialog.ResultName,
                    AccountTabs = []
                };

                // Tạo ViewModel và tự động Focus
                var moduleVM = new ModuleViewModel(_webViewService, newEntity, Accounts);
                Modules.Add(moduleVM);
            }
        }

        private void LoadMockSystemAccounts()
        {
            // Dựa trên class Account trong file classes.txt
            Accounts =
            [
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
            ];
        }

        private void CreateDemoModule()
        {

            var moduleEntity = new ModuleEntity
            {
                Name = $"Module Test {Modules.Count + 1}",
                Description = "Kịch bản kiểm thử tự động hệ thống đào tạo",
                AccountTabs = new AccountTab[]
                {
                    // Cấu hình cho Account Admin
                    new AccountTab
                    {
                        AccountId = "acc_admin", // Phải khớp với Id trong Accounts
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
            var moduleVM = new ModuleViewModel(_webViewService, moduleEntity, Accounts);

            Modules.Add(moduleVM);
            CurrentModule = moduleVM;
        }
    }
}