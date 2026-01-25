using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services; // Namespace chứa WebViewService

namespace PRERP_TESTER.ViewModels
{
    public class ModuleViewModel
    {
        private readonly WebViewService _webViewService;

        public ModuleEntity ModuleEntity { get; }

        public ObservableCollection<AccountViewModel> Accounts { get; set; }

        // Account đang được chọn trên giao diện
        private AccountViewModel SelectedAccount;

        public ICommand OpenAllCommand { get; }

        // Constructor
        public ModuleViewModel(WebViewService webViewService, ModuleEntity moduleData, List<Account> allSystemAccounts)
        {
            _webViewService = webViewService;
            ModuleEntity = moduleData;
            Accounts = new ObservableCollection<AccountViewModel>();

            // Load dữ liệu từ ModuleEntity.AccountTabs
            LoadAccountsFromEntity(allSystemAccounts);

            SelectedAccount = Accounts.FirstOrDefault();

            OpenAllCommand = new RelayCommand(ExecuteOpenAll);
        }

        private void LoadAccountsFromEntity(List<Account> allAccounts)
        {
            if (ModuleEntity.AccountTabs == null) return;

            foreach (var accTabConfig in ModuleEntity.AccountTabs)
            {
                // Tìm thông tin Account gốc dựa vào AccountId
                var accInfo = allAccounts.FirstOrDefault(a => a.Id == accTabConfig.AccountId);

                if (accInfo != null)
                {
                    // Tạo ViewModel kết hợp
                    var accVM = new AccountViewModel(accInfo, accTabConfig);
                    Accounts.Add(accVM);
                }
            }
        }

        private async void ExecuteOpenAll()
        {
            foreach (var accVM in Accounts)
            {
                // Gọi WebViewService để chuẩn bị Environment (Session) cho AccountId này
                await _webViewService.GetEnvironmentAsync(accVM.AccountId);

                // Sau đó khởi tạo WebView cho từng Tab (Logic chi tiết sẽ nằm ở WebViewService)
                // Ví dụ: await _webViewService.InitializeTabsForAccount(accVM);
            }
        }
    }
}