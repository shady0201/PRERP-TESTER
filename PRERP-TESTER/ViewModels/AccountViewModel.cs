using System.Collections.ObjectModel;
using PRERP_TESTER.Models;

namespace PRERP_TESTER.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        // 1. Thông tin định danh (Từ class Account)
        public Account AccountInfo { get; }

        // 2. Cấu hình Tab của account này trong Module (Từ class AccountTab)
        public AccountTab AccountTabConfig { get; }

        // 3. Danh sách Tab hiển thị lên giao diện
        public ObservableCollection<TabViewModel> Tabs { get; set; }

        public AccountViewModel(Account account, AccountTab accountTabConfig)
        {
            AccountInfo = account;
            AccountTabConfig = accountTabConfig;
            Tabs = new ObservableCollection<TabViewModel>();

            // Load danh sách TabWebItems từ AccountTab vào ViewModel
            if (AccountTabConfig.TabWebItems != null)
            {
                foreach (var tabWeb in AccountTabConfig.TabWebItems)
                {
                    Tabs.Add(new TabViewModel(tabWeb));
                }
            }
        }

        // Binding Properties
        public string DisplayName => AccountInfo.DisplayName; // Dùng DisplayName như trong model của bạn
        public string RoleName => AccountInfo.Role?.Name ?? "N/A";
        public string AccountId => AccountInfo.Id;
    }
}