using System.Collections.ObjectModel;
using PRERP_TESTER.Models;

namespace PRERP_TESTER.ViewModels
{
    public class AccountViewModel : LazyLoadViewModel
    {
        public Account Account { get; }
        public AccountModule AccountModule { get; }

        public TabWeb SelectTab { get; set; }
        public ObservableCollection<TabWeb> Tabs { get; set; }

        public AccountViewModel(Account account, AccountModule accountModule)
        {
            Account = account;
            AccountModule = accountModule;
            Tabs = [.. accountModule.TabWebItems];
            SelectTab = accountModule.TabWebItems.FirstOrDefault();
        }

        // Binding Properties
        public string DisplayName => Account.DisplayName;
        public string RoleName => Account.Role?.Name ?? "N/A";
        public string AccountId => Account.Id;


    }
}