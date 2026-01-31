using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Helper;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;
using PRERP_TESTER.Views.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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

        private string _searchAccountText;
        public string SearchAccountText
        {
            get => _searchAccountText;
            set
            {
                if (SetProperty(ref _searchAccountText, value))
                {
                    AccountMenuView.Refresh();
                }
            }
        }

        public ICollectionView AccountMenuView { get; }

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
        public ICommand EditAccountCommand { get; }
        public ICommand RemoveAccountCommand { get; }

        public MainViewModel()
        {

            LoadAllData();

            // Commands
            CreateModuleCommand = new RelayCommand(ExecuteCreateModule);
            CreateAccountCommand = new RelayCommand(ExecuteCreateAccount);
            EditAccountCommand = new RelayCommand<Account>(ExecuteEditAccount);
            RemoveAccountCommand = new RelayCommand<Account>(ExecuteRemoveAccount);
            ToggleMenuCommand = new RelayCommand(() => IsMenuCollapsed = !IsMenuCollapsed);

            AccountMenuView = new ListCollectionView(Accounts);
            AccountMenuView.Filter = FilterAccountMenu;

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

        private void ExecuteEditAccount(Account account) {
            if (account == null) return;

            var dialog = new AddAccountDialog(account);
            dialog.Owner = Application.Current.MainWindow;

            if (dialog.ShowDialog() == true)
            {
                int index = Accounts.IndexOf(account);
                if (index >= 0)
                {
                    Accounts[index] = account;
                }
                SaveAllData();
            }

        }
        private void ExecuteRemoveAccount(Account account) {
            if (account == null) return;

            var result = MessageBox.Show($"Xóa vĩnh viễn tài khoản {account.DisplayName} khỏi hệ thống?",
                                        "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Accounts.Remove(account);

                foreach (var module in Modules)
                {
                    var accountVMInModule = module.ModuleAccounts.FirstOrDefault(vm => vm.Account.Id == account.Id);

                    if (accountVMInModule != null)
                    {

                        module.ModuleAccounts.Remove(accountVMInModule);

                        var list = module.ModuleEntity.AccountModules.ToList();
                        list.RemoveAll(am => am.AccountID == account.Id);
                        module.ModuleEntity.AccountModules = list.ToArray();

                        if (module.SelectedAccountModule == accountVMInModule)
                        {
                            module.SelectedAccountModule = module.ModuleAccounts.FirstOrDefault();
                        }
                    }
                }

                AccountMenuView.Refresh();
                SaveAllData();
            }
        }

        private bool FilterAccountMenu(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchAccountText)) return true;

            if (obj is Account acc)
            {
                string search = StringHelper.RemoveSign4VietnameseString(SearchAccountText.ToLower().Trim());
                string name = StringHelper.RemoveSign4VietnameseString(acc.DisplayName?.ToLower() ?? "");
                string user = StringHelper.RemoveSign4VietnameseString(acc.Username?.ToLower() ?? "");

                return name.Contains(search) || user.Contains(search);
            }
            return false;
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
                        AccountId = t.UserAccount.Username,
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
                Accounts = Accounts.ToList(),
                Modules = Modules.Select(m => m.ModuleEntity).ToList()
            };

            DataService.SaveData(data);
        }


        

    }
}