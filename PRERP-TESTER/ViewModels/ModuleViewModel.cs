using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;
using PRERP_TESTER.Views.Dialogs;

namespace PRERP_TESTER.ViewModels
{
    public class ModuleViewModel : LazyLoadViewModel
    {
        public ModuleEntity ModuleEntity { get; }

        public ObservableCollection<AccountViewModel> ModuleAccounts { get; set; } = [];

        private readonly ObservableCollection<Account> AllSystemAccounts;

        public AccountViewModel? SelectedAccountModule { get; set; }

        public ICommand AddAccountToModuleCommand { get; }

        public IRelayCommand<AccountViewModel> RemoveAccountFromModuleCommand { get; }

        public string Name => ModuleEntity.Name;

        public ModuleViewModel(ModuleEntity module, ObservableCollection<Account> allSystemAccounts)
        {
            ModuleEntity = module;
            AllSystemAccounts = allSystemAccounts;

            if (allSystemAccounts.Count > 0)
            {
                for (int i = 0; i < module.AccountModules.Length; i++)
                {
                    var account = allSystemAccounts.First(a => a.Id == module.AccountModules[i].AccountID);
                    if (account != null)
                    {
                        var accountVM = new AccountViewModel(account, module.Id, module.AccountModules[i].TabWebItems);
                        ModuleAccounts.Add(accountVM);
                    }

                }
            }

            AddAccountToModuleCommand = new RelayCommand(ExecuteAddAccountToModule);
            RemoveAccountFromModuleCommand = new RelayCommand<AccountViewModel>(ExecuteRemoveAccount);

            SelectedAccountModule = ModuleAccounts.FirstOrDefault();
        }

        private void ExecuteAddAccountToModule()
        {
            List<string> accountIds = [.. ModuleEntity.AccountModules.Select(am => am.AccountID)];
            var dialog = new AccountPickerDialog(AllSystemAccounts, accountIds)
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                var selectedAccounts = dialog.SelectedAccounts;

                foreach (var acc in selectedAccounts)
                {
                    var newAccountModule = new AccountModule
                    {
                        AccountID = acc.Id,
                        TabWebItems = []
                    };
                    var list = ModuleEntity.AccountModules.ToList();
                    list.Add(newAccountModule);
                    ModuleEntity.AccountModules = list.ToArray();
                    var accountVM = new AccountViewModel(acc, ModuleEntity.Id, newAccountModule.TabWebItems);
                    ModuleAccounts.Add(accountVM);
                }
                SelectedAccountModule = ModuleAccounts.FirstOrDefault();
            }
        }

        private void ExecuteRemoveAccount(AccountViewModel? accountVM)
        {
            if (accountVM == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn bỏ tài khoản '{accountVM.Account.DisplayName}' khỏi module này?",
                                         "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                RemoveAccount(accountVM);
            }
        }

        public void RemoveAccount(AccountViewModel? accountVM)
        {
            if (accountVM == null) return;

            // dọn dẹp tabs account
            for (int i = 0; i < accountVM.TabViewModels.Count; i++)
            {
                accountVM.TabViewModels[i].CloseTabCommand.Execute(null);
            }
            accountVM.TabViewModels.Clear();

            // xoá khỏi danh sách accountviewmodel
            ModuleAccounts.Remove(accountVM);

            // xoá khỏi danh sách entity
            var list = ModuleEntity.AccountModules.ToList();
            var itemToRemove = list.FirstOrDefault(am => am.AccountID == accountVM.Account.Id);
            if (itemToRemove != null)
            {
                list.Remove(itemToRemove);
                ModuleEntity.AccountModules = list.ToArray();
            }

            if (SelectedAccountModule == accountVM)
            {
                SelectedAccountModule = ModuleAccounts.FirstOrDefault();
            }
            
        }

    }
}