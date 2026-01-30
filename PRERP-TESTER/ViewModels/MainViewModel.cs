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

            //LoadMockSystemAccounts();
            //CreateDemoModule();
            LoadAllData();

            // Commands
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
                    Id= Guid.NewGuid().ToString("N"),
                    Username = dialog.Username,
                    Password = dialog.Password,
                    DisplayName = dialog.DisplayName,
                    Stype = dialog.Stype,
                };
                Accounts.Add(account);
            }
        }

        public void LoadAllData()
        {
            var data = DataService.LoadData<ApplicationData>();
            Accounts = new ObservableCollection<Account>(data.Accounts);
            Modules.Clear();
            foreach (var entity in data.Modules)
            {
                Modules.Add(new ModuleViewModel(entity, Accounts));
            }
        }

        public void SaveAllData()
        {
            foreach (var moduleVM in Modules)
            {
                var accountModuleList = new List<AccountModule>();

                foreach (var accVM in moduleVM.ModuleAccounts)
                {
                    var tabEntities = accVM.TabViewModels.Select(t => new TabWeb
                    {
                        ModuleId = t.ModuleID,
                        AccountId = t.UserName,
                        Title = t.Title,
                        Url = t.Url,
                    }).ToArray();

                    accountModuleList.Add(new AccountModule
                    {
                        AccountID = accVM.Account.Id,
                        TabWebItems = tabEntities
                    });
                }
                moduleVM.ModuleEntity.AccountModules = accountModuleList.ToArray();
            }

            var data = new ApplicationData
            {
                Accounts = this.Accounts.ToList(),
                Modules = this.Modules.Select(m => m.ModuleEntity).ToList()
            };

            DataService.SaveData(data);
        }

        /**
         * 
         * new Account
                {
                    Id = "acc_admin",
                    DisplayName = "Giảng Viên 1",
                    Username = "bmtu02",
                    Password = "12345678Aa@",
                    Stype = "STAFF",
                },
         * ***/

        private void LoadMockSystemAccounts()
        {
            // Dựa trên class AccountID trong file classes.txt
            Accounts =
            [
                
                new Account
                {
                    Id = "acc_teacher",
                    DisplayName = "Giảng viên 2",
                    Username = "BMTU00953",
                    Password="12345678Aa@",
                    Stype= "STAFF"
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