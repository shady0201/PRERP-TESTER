using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;

namespace PRERP_TESTER.ViewModels
{
    public class ModuleViewModel : LazyLoadViewModel
    {

        public ModuleEntity ModuleEntity { get; }

        public ObservableCollection<AccountViewModel> ModuleAccounts { get; set; } = [];

        public AccountViewModel? SelectedAccountModule { get; set; }

        public ICommand OpenAllCommand { get; }

        public string Name => ModuleEntity.Name;

        public ModuleViewModel( ModuleEntity module, List<Account> allSystemAccounts)
        {
            ModuleEntity = module;

            for (int i = 0; i < module.AccountModules.Length; i++)
            {
                var account = allSystemAccounts.Find(a => a.Id == module.AccountModules[i].AccountID);
                if (account != null)
                {
                    var accountVM = new AccountViewModel(account,module.Id, module.AccountModules[i].TabWebItems);
                    ModuleAccounts.Add(accountVM);
                }
            }

            SelectedAccountModule = ModuleAccounts.FirstOrDefault();

        }

        public void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabViewModel tab)
            {
                tab.IsLoaded = true;
            }
        }
    }
}