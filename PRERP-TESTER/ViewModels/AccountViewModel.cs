using System.Collections.ObjectModel;
using PRERP_TESTER.Models;

namespace PRERP_TESTER.ViewModels
{
    public class AccountViewModel : LazyLoadViewModel
    {
        public Account Account { get; }
        public AccountModule AccountModule { get; }

        public TabViewModel SelectTab { get; set; }
        public ObservableCollection<TabViewModel> TabViewModels { get; set; }

        public AccountViewModel(Account account, AccountModule accountModule)
        {
            Account = account;
            AccountModule = accountModule;
            TabViewModels = accountModule.TabWebItems != null ? [.. accountModule.TabWebItems.Select(tab => new TabViewModel(tab))] : [];
            SelectTab = TabViewModels.FirstOrDefault();
        }

        // Binding Properties
        public string DisplayName => Account.DisplayName;
        public string RoleName => Account.Role?.Name ?? "N/A";
        public string AccountId => Account.Id;


    }
}