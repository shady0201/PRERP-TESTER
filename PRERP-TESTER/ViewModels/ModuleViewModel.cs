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

        private ObservableCollection<Account> AllSystemAccounts;

        public AccountViewModel? SelectedAccountModule { get; set; }

        public ICommand AddAccountToModuleCommand { get; }

        public string Name => ModuleEntity.Name;

        public ModuleViewModel( ModuleEntity module, ObservableCollection<Account> allSystemAccounts)
        {
            ModuleEntity = module;
            AllSystemAccounts = allSystemAccounts;

            for (int i = 0; i < module.AccountModules.Length; i++)
            {
                var account = allSystemAccounts.First(a => a.Id == module.AccountModules[i].AccountID);
                if (account != null)
                {
                    var accountVM = new AccountViewModel(account,module.Id, module.AccountModules[i].TabWebItems);
                    ModuleAccounts.Add(accountVM);
                }
            }

            AddAccountToModuleCommand = new RelayCommand(ExecuteAddAccountToModule);

            SelectedAccountModule = ModuleAccounts.FirstOrDefault();
        }

        private void ExecuteAddAccountToModule()
        {
            List<string> accountIds = ModuleEntity.AccountModules.Select(am => am.AccountID).ToList();
            var dialog = new AccountPickerDialog(AllSystemAccounts, accountIds)
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                var selectedAccounts = dialog.SelectedAccounts;

                foreach (var acc in selectedAccounts)
                {
                    
                }
            }
        }

    }
}