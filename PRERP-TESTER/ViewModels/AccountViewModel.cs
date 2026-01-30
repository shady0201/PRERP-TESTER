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
            ObservableCollection<TabViewModel> tabs = [];
            foreach (var tab in tabWebs)
            {
                TabViewModels.Add(CreateTab(tab));
            }

            SelectedTab = TabViewModels.FirstOrDefault();

            AddTabCommand = new RelayCommand(ExecuteAddTab);
        }

        public void ExecuteAddTab()
        {
            var tabWeb = new TabWeb { Title = "Thẻ mới", Url = "https://prerp.bmtu.edu.vn" };
            var tabViewModel = CreateTab(tabWeb);
            TabViewModels.Add(tabViewModel);
            SelectedTab = tabViewModel;
        }
        
        private TabViewModel CreateTab(TabWeb tab)
        {
            return new TabViewModel(tab, Account.Username,Account.Password,Account.Stype, ModuleID, (tabToDelete) => {
                TabViewModels.Remove(tabToDelete);
                if (SelectedTab == null && TabViewModels.Count > 0)
                {
                    SelectedTab = TabViewModels.Last();
                }
            });
        }

        // Binding Properties
        public string DisplayName => Account.DisplayName;
        public string AccountId => Account.Id;


    }
}