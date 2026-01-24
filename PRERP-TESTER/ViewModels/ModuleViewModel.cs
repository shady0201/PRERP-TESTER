using System.Collections.ObjectModel;
using PRERP_TESTER.Services;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace PRERP_TESTER.ViewModels
{
    public class ModuleViewModel : ViewModelBase
    {
        private readonly WebViewService _webViewService;

        // Danh sách Account hiển thị trên giao diện
        public ObservableCollection<AccountViewModel> Accounts { get; set; }

        private AccountViewModel _selectedAccount;
        public AccountViewModel SelectedAccount
        {
            get => _selectedAccount;
            set => SetProperty(ref _selectedAccount, value);
        }

        public ICommand OpenAllCommand { get; }

        public ModuleViewModel(WebViewService webViewService)
        {
            _webViewService = webViewService;
            Accounts = new ObservableCollection<AccountViewModel>();

            // Command thực thi logic Open All
            OpenAllCommand = new RelayCommand(async () => await ExecuteOpenAll());

            // Load dữ liệu mẫu (Sau này bạn sẽ load từ Database/Model)
            LoadMockData();
        }

        private void LoadMockData()
        {
            Accounts.Add(new AccountViewModel("acc01", "acc01 - Admin", "#78B688"));
            Accounts.Add(new AccountViewModel("acc02", "acc02 - GV", "#7A96D0"));
            Accounts.Add(new AccountViewModel("acc03", "acc03 - SV", "#A689C8"));

            if (Accounts.Count > 0) SelectedAccount = Accounts[0];
        }

        private async System.Threading.Tasks.Task ExecuteOpenAll()
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