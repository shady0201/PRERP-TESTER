using System.Collections.ObjectModel;
using System.Windows.Media;
using PRERP_TESTER.Models; // Giả sử bạn có AccountModel trong này

namespace PRERP_TESTER.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        public string AccountId { get; set; }
        public string DisplayName { get; set; }

        // Danh sách các Tab (Dashboard, Approve...) của tài khoản này
        public ObservableCollection<WebViewTabModel> Tabs { get; set; } = new ObservableCollection<WebViewTabModel>();

        private WebViewTabModel _selectedTab;
        public WebViewTabModel SelectedTab
        {
            get => _selectedTab;
            set { _selectedTab = value; OnPropertyChanged(); }
        }

        public AccountViewModel(string id, string name, string hexColor)
        {
            AccountId = id;
            DisplayName = name;
            // Khởi tạo một tab mặc định
            Tabs.Add(new WebViewTabModel { Header = "Dashboard", Url = "about:blank" });
            SelectedTab = Tabs[0];
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