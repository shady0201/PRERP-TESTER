using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;

namespace PRERP_TESTER.ViewModels
{
    public class ModuleViewModel : LazyLoadViewModel
    {
        private readonly WebViewService _webViewService;

        public ModuleEntity ModuleEntity { get; }

        public ObservableCollection<AccountViewModel> ModuleAccounts { get; set; }

        public AccountViewModel? SelectedAccountModule { get; set; }

        public ICommand OpenAllCommand { get; }

        // Tempalte menuitem
        public string Name => ModuleEntity.Name;

        public ModuleViewModel(WebViewService webViewService, ModuleEntity module, List<Account> allSystemAccounts)
        {
            _webViewService = webViewService;
            ModuleEntity = module;
            
            ModuleAccounts = new ObservableCollection<AccountViewModel>();

            // Load dữ liệu từ ModuleEntity.Tabs
            LoadAccountsFromEntity(allSystemAccounts);

            SelectedAccountModule = ModuleAccounts.FirstOrDefault();

            OpenAllCommand = new RelayCommand(ExecuteOpenAll);
        }

        private void LoadAccountsFromEntity(List<Account> allAccounts)
        {
            if (ModuleEntity.AccountModule == null) return;

            foreach (var tabs in ModuleEntity.AccountModule)
            {
                // Tìm thông tin AccountID gốc dựa vào AccountId
                var account = allAccounts.FirstOrDefault(a => a.Id == tabs.AccountID);

                if (account != null)
                {
                    // Tạo ViewModel kết hợp
                    var accVM = new AccountViewModel(account, tabs);
                    ModuleAccounts.Add(accVM);
                }
            }
        }

        private async void ExecuteOpenAll()
        {
            foreach (var accVM in ModuleAccounts)
            {
                // Gọi WebViewService để chuẩn bị Environment (Session) cho AccountId này
                await _webViewService.GetEnvironmentAsync(accVM.AccountId);

                // Sau đó khởi tạo WebView cho từng Tab (Logic chi tiết sẽ nằm ở WebViewService)
                // Ví dụ: await _webViewService.InitializeTabsForAccount(accVM);
            }
        }
    }
}