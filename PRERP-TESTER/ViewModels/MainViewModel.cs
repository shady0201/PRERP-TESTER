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
        private ObservableCollection<Account> _accounts = [];
        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        private bool _isMenuCollapsed;
        public bool IsMenuCollapsed
        {
            get => _isMenuCollapsed;
            set => SetProperty(ref _isMenuCollapsed, value);
        }

       

        public ObservableCollection<ModuleViewModel> Modules { get; set; } = [];

        public ModuleViewModel? SelectedModule { get; set; }

        // command
        public ICommand CreateModuleCommand { get; }
        public ICommand CreateAccountCommand { get; }
        public ICommand ToggleMenuCommand { get; }

        public MainViewModel()
        {

            LoadMockSystemAccounts();
            CreateDemoModule();

            // Command list
            CreateModuleCommand = new RelayCommand(ExecuteCreateModule);
            CreateAccountCommand = new RelayCommand(ExecuteCreateAccount);

            ToggleMenuCommand = new RelayCommand(() => IsMenuCollapsed = !IsMenuCollapsed);

        }

        private void ExecuteCreateModule()
        {
            var dialog = new AddModuleDialog { Owner = Application.Current.MainWindow };

            if (dialog.ShowDialog() == true)
            {
                var moduleEntity = new ModuleEntity
                {
                    Name = dialog.ResultName,
                    AccountModules = []
                };

                var moduleVM = new ModuleViewModel(moduleEntity, Accounts);
                Modules.Add(moduleVM);
            }
        }

        private void ExecuteCreateAccount()
        {
            var dialog = new AddAccountDialog { Owner = Application.Current.MainWindow };

            if (dialog.ShowDialog() == true)
            {
                var account = new Account
                {
                    Username = dialog.Username,
                    Password = dialog.Password,
                    DisplayName = dialog.DisplayName,
                    Stype = dialog.Stype,
                };
                Accounts.Add(account);
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
                    Username = "bmtu02",
                    DisplayName = "Giảng Viên 1",
                    Password = "12345678Aa@",
                },
                new Account
                {
                    Id = "acc_teacher",
                    Username = "teacher_ha",
                    DisplayName = "Giảng viên 2",
                }
            ];
        }

        private void CreateDemoModule()
        {
            for (int i = 0; i < 5; i++)
            {
                var moduleEntity = new ModuleEntity
                {
                    Name = $"Module Test {Modules.Count + 1}",
                    Description = "Kịch bản kiểm thử tự động hệ thống đào tạo",
                    AccountModules =
                    [
                        new() {
                            AccountID = "acc_admin",
                            TabWebItems =
                            [
                                new() { Title = "Q.Lý Hệ Thống", Url = "https://prerp.bmtu.edu.vn" },
                                new() { Title = "Prerp - debug", Url = "https://prerp.bmtu.edu.vn/regulation/index?lang=vi" },
                                new() { Title = "SFT debug", Url = "https://prerp.bmtu.edu.vn/sftraining/debug?lang=vi" }
                            ]
                        },
                        // Cấu hình cho AccountID Teacher
                        new AccountModule
                        {
                            AccountID = "acc_teacher",
                            TabWebItems =
                            [
                                new() { Title = "Chấm điểm", Url = "https://prerp.bmtu.edu.vn/" },
                                new() { Title = "Trang duyệt", Url = "https://prerp.bmtu.edu.vn/" },
                            ]
                        }
                    ]
                };
                var moduleVM = new ModuleViewModel(moduleEntity, Accounts);

                Modules.Add(moduleVM);
            }
            SelectedModule = Modules.FirstOrDefault();
        }
    }
}