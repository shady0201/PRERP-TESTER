using System.Collections.ObjectModel;
using PRERP_TESTER.Models;

namespace PRERP_TESTER.ViewModels
{
    public class AccountViewModel : LazyLoadViewModel
    {
        public Account Account { get; }
        public string ModuleID { get; set; }

        public ObservableCollection<TabViewModel> TabViewModels { get; set; } = [];
        public TabViewModel? SelectedTab { get; set; }

        public AccountViewModel(Account account,string moduleID, TabWeb[] tabWebs)
        {
            Account = account;
            ModuleID = moduleID;
            ObservableCollection<TabViewModel> tabs = [];
            foreach (var tab in tabWebs)
            {
                TabViewModels.Add(new TabViewModel(tab,account.Id,ModuleID));
            }

            SelectedTab = TabViewModels.FirstOrDefault();
        }

        // Binding Properties
        public string DisplayName => Account.DisplayName;
        public string AccountId => Account.Id;


    }
}