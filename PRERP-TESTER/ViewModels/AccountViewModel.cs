using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;

namespace PRERP_TESTER.ViewModels
{
    public class AccountViewModel : LazyLoadViewModel
    {
        public Account Account { get; }
        public string ModuleID { get; set; }

        public ObservableCollection<TabViewModel> TabViewModels { get; set; } = [];

        private TabViewModel? _selectedTab;
        public TabViewModel? SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public ICommand AddTabCommand { get; }


        public AccountViewModel(Account account,string moduleID, TabWeb[] tabWebs)
        {
            Account = account;
            ModuleID = moduleID;

            TabViewModels.Clear();
            foreach (var tab in tabWebs)
            {
                var tabVM = CreateTab(tab);
                tabVM.ModuleID = ModuleID;

                TabViewModels.Add(tabVM);
            }

            SelectedTab = TabViewModels.FirstOrDefault();

            AddTabCommand = new RelayCommand(ExecuteAddTab);
        }

        public void ExecuteAddTab()
        {
            var tabWeb = new TabWeb { Title = "Thẻ mới"};
            var tabViewModel = CreateTab(tabWeb);
            TabViewModels.Add(tabViewModel);
            SelectedTab = tabViewModel;
        }
        
        private TabViewModel CreateTab(TabWeb tab)
        {
            var vm = new TabViewModel(tab, Account, ModuleID, (tabToDelete) => {
                TabViewModels.Remove(tabToDelete);
                if (SelectedTab == null && TabViewModels.Count > 0)
                {
                    SelectedTab = TabViewModels.Last();
                }
            });

            vm.RequestCloseOthers += (currentTab) => {
                var otherTabs = TabViewModels.Where(t => t != currentTab).ToList();
                foreach (var t in otherTabs)
                {
                    t.CloseTabCommand.Execute(null);
                }
            };

            vm.RequestDuplicate += (currentTab) => {
                var duplicateData = new TabWeb
                {
                    ModuleId = currentTab.ModuleID,
                    AccountId = currentTab.UserAccount.Id,
                    Url = currentTab.Url,
                    Title = currentTab.Title + " (Bản sao)",
                    FaviconUrl = currentTab.FaviconUrl
                };

                var duplicateVM = CreateTab(duplicateData);
                TabViewModels.Add(duplicateVM);
                SelectedTab = duplicateVM;
            };
            return vm;
        }

        // Binding Properties
        public string DisplayName => Account.DisplayName;
        public string AccountId => Account.Id;


    }
}