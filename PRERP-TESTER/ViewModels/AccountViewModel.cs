using System.Collections.ObjectModel;
using PRERP_TESTER.Models;

namespace PRERP_TESTER.ViewModels
{
    public class AccountViewModel
    {
        public Account Account { get; }
        public AccountTab AccountTabs { get; }
        public ObservableCollection<TabViewModel> Tabs { get; set; }

        public AccountViewModel(Account account, AccountTab accountTabs)
        {
            Account = account;
            AccountTabs = accountTabs;
            Tabs = new ObservableCollection<TabViewModel>();

            // Load danh sách TabWebItems từ AccountTabs vào ViewModel
            if (AccountTabs.TabWebItems != null)
            {
                foreach (var tabWeb in AccountTabs.TabWebItems)
                {
                    Tabs.Add(new TabViewModel(tabWeb));
                }
            }
        }

        // Binding Properties
        public string DisplayName => Account.DisplayName;
        public string RoleName => Account.Role?.Name ?? "N/A";
        public string AccountId => Account.Id;

    }
}