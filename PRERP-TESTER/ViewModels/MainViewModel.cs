using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;
using PRERP_TESTER.Views.Dialogs;

namespace PRERP_TESTER.ViewModels
{
    public class MainViewModel : LazyLoadViewModel
    {
        // service
        private readonly WebViewService _webViewService;
           
        // data
        private List<Account>? Accounts;

        public ObservableCollection<ModuleViewModel> Modules { get; set; }

        private ModuleViewModel? _selectedModule;
        public ModuleViewModel? SelectedModule
        {   get => _selectedModule;
            set {
                if (SetProperty(ref _selectedModule, value))
                {
                    ActivateItem(Modules,value);
                }
            }
        }

        public static void ActivateItem(IEnumerable<LazyLoadViewModel> list, LazyLoadViewModel? selectedItem)
        {
            if (selectedItem == null || list == null) return;

            foreach (var vm in list)
            {
                vm.IsVisible = (vm == selectedItem);

                if (vm.IsVisible && !vm.IsLoaded)
                {
                    vm.IsLoaded = true;
                }
            }
        }


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
                    AccountModules = []
                };

                // Tạo ViewModel và tự động Focus
                var moduleVM = new ModuleViewModel(_webViewService, newEntity, Accounts);
                Modules.Add(moduleVM);
            }
        }

        private void LoadMockSystemAccounts()
        {
            // Dựa trên class AccountID trong file classes.txt
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
            for (int i = 0; i < 10; i++)
            {
                var moduleEntity = new ModuleEntity
                {
                    Name = $"Module Test {Modules.Count + 1}",
                    Description = "Kịch bản kiểm thử tự động hệ thống đào tạo",
                    AccountModules =
                    [
                        // Cấu hình cho AccountID Admin
                        new() {
                            AccountID = "acc_admin", // Phải khớp với Id trong ModuleAccounts
                            TabWebItems = new TabWeb[] // Danh sách TabWeb (Title, Url)
                            {
                                new() { Title = "Q.Lý Hệ Thống", Url = "https://google.com" },
                                new() { Title = "Logs", Url = "https://bing.com" }
                            }
                        },
                        // Cấu hình cho AccountID Teacher
                        new AccountModule
                        {
                            AccountID = "acc_teacher",
                            TabWebItems = new TabWeb[]
                            {
                                new() { Title = "Chấm điểm", Url = "https://stackoverflow.com" }
                            }
                        }
                    ]
                };
                var moduleVM = new ModuleViewModel(_webViewService, moduleEntity, Accounts);

                Modules.Add(moduleVM);
            }
            SelectedModule = Modules.FirstOrDefault();
        }
    }
}