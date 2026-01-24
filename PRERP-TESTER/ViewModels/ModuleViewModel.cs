using System.Collections.ObjectModel;
using PRERP_TESTER.Services;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Views.Pages;
using System.Reflection;
using PRERP_TESTER.Models;

namespace PRERP_TESTER.ViewModels
{
    public class ModuleViewModel : ViewModelBase
    {
        private readonly WebViewService _webViewService;
        private readonly ModuleEntity _moduleEntity;

        public string DisplayName => _moduleEntity.Name;

        // Danh sách Account hiển thị trên giao diện
        public ObservableCollection<AccountViewModel> Accounts { get; set; }
        public ObservableCollection<TestCase> TestCases => _moduleEntity.TestCases;

        private AccountViewModel _selectedAccount;
        public AccountViewModel SelectedAccount
        {
            get => _selectedAccount;
            set => SetProperty(ref _selectedAccount, value);
        }

        public ICommand OpenAllCommand { get; }

        public ModuleViewModel(WebViewService webService, ModuleEntity entity, List<Account> allAccounts)
        {
            _webViewService = webService;
            _moduleEntity = entity;

            // Lọc ra các Account thuộc về Module này từ danh sách tổng
            var relevantAccounts = allAccounts.Where(a => _moduleEntity.AssignedAccountIds.Contains(a.Id));

            foreach (var acc in relevantAccounts)
            {
                Accounts.Add(new AccountViewModel(acc));
            }
        }

        private void LoadMockData()
        {
            
        }

        private async Task ExecuteOpenAll()
        {
            foreach (var acc in Accounts)
            {
                // Gọi Service để lấy Environment chung cho Account ID này
                var env = await _webViewService.GetEnvironmentAsync(acc.AccountId);
                // Logic tiếp theo: Khởi tạo các WebView control cho từng Tab
            }
        }
    }
}