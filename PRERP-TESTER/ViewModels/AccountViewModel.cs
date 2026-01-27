using System.Collections.ObjectModel;
using PRERP_TESTER.Models;

namespace PRERP_TESTER.ViewModels
{
    public class AccountViewModel : LazyLoadViewModel
    {
        public Account Account { get; }

        public ObservableCollection<TabViewModel> TabViewModels { get; set; }
        public TabViewModel SelectTab { get; set; }

        public AccountViewModel(Account account, ObservableCollection<TabViewModel> tabViewModels)
        {
            Account = account;
            TabViewModels = tabViewModels;
            SelectTab = TabViewModels.FirstOrDefault();
        }

        // Binding Properties
        public string DisplayName => Account.DisplayName;
        public string RoleName => Account.Role?.Name ?? "N/A";
        public string AccountId => Account.Id;


    }
}