using PRERP_TESTER.Models; // Namespace chứa các entity của bạn

namespace PRERP_TESTER.ViewModels
{
    public class TabViewModel : ViewModelBase
    {
        // Entity gốc: TabWeb (chứa Title, Url, LastActiveUtc...)
        public TabWeb TabData { get; }

        // Instance của WebView2 control (để giữ tab sống)
        private object _webViewInstance;
        public object WebViewInstance
        {
            get => _webViewInstance;
            set => SetProperty(ref _webViewInstance, value);
        }

        public TabViewModel(TabWeb tabWeb)
        {
            TabData = tabWeb;
        }

        // Binding: Lấy Title trực tiếp từ TabWeb
        public string Header
        {
            get => TabData.Title;
            set { TabData.Title = value; OnPropertyChanged(); }
        }

        public string Url => TabData.Url;
    }
}