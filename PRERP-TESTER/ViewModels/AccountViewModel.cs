using System.Collections.ObjectModel;
using System.Windows.Media;
using PRERP_TESTER.Models; // Giả sử bạn có AccountModel trong này

namespace PRERP_TESTER.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {

        // Giữ nguyên Entity gốc để xử lý logic nghiệp vụ
        public Account Account { get; private set; }

        public string AccountId => Account.Id;
        public string DisplayName => Account.Username;

        // Danh sách các Tab (Dashboard, Approve...) của tài khoản này
        public ObservableCollection<WebViewTabModel> Tabs { get; set; } = new ObservableCollection<WebViewTabModel>();

        private WebViewTabModel _selectedTab;
        public WebViewTabModel SelectedTab
        {
            get => _selectedTab;
            set { _selectedTab = value; OnPropertyChanged(); }
        }

        public AccountViewModel(Account _account)
        {
            this.Account = _account;
            // Khởi tạo các Tab dựa trên dữ liệu lưu trữ trong Entity
            foreach (var tabInfo in _account.SavedTabs)
            {
                Tabs.Add(new WebViewTabModel { Header = tabInfo.Title, Url = tabInfo.Url });
            }
        }
    }

    // Model đơn giản cho mỗi Tab bên trong Account
    public class WebViewTabModel : ViewModelBase
    {
        public string Header { get; set; }
        public string Url { get; set; }

        // Quan trọng: Giữ instance WebView2 để không bị load lại
        private object _webViewInstance;
        public object WebViewInstance
        {
            get => _webViewInstance;
            set { _webViewInstance = value; OnPropertyChanged(); }
        }
    }
}