using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using System.Web;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input; // Namespace chứa các entity của bạn

namespace PRERP_TESTER.ViewModels
{
    public class TabViewModel : LazyLoadViewModel
    {
        public string AccountID { get; set; }
        public string ModuleID { get; set; }
        public TabWeb TabData { get; }

        private string _url;
        public string Url
        {
            get => _url;
            set => SetProperty(ref _url, value); // Đảm bảo OnPropertyChanged được gọi
        }
        public string Title => TabData.Title;

        private bool _isSecure;
        public bool IsSecure
        {
            get => _isSecure;
            set => SetProperty(ref _isSecure, value);
        }

        private void UpdateSecurityStatus(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            IsSecure = url.ToLower().StartsWith("https://");
        }

        public ICommand CloseTabCommand { get; }

        public ICommand BackCommand { get; }
        public ICommand ForwardCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand GoToUrlCommand { get; }

        public event Action<string>? NavigationRequested;

        public TabViewModel(TabWeb tabWeb, string username, string moduleID, Action<TabViewModel> closeAction)
        {
            Url = tabWeb.Url;
            UpdateSecurityStatus(Url);
            TabData = tabWeb;
            IsLoaded = false;
            AccountID = username;
            ModuleID = moduleID;

            BackCommand = new RelayCommand(() => NavigationRequested?.Invoke("Back"));
            ForwardCommand = new RelayCommand(() => NavigationRequested?.Invoke("Forward"));
            ReloadCommand = new RelayCommand(() => NavigationRequested?.Invoke("Reload"));
            GoToUrlCommand = new RelayCommand(ExecuteGoToUrl);

            CloseTabCommand = new RelayCommand(() => closeAction(this));
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)=> (sender as TextBox)?.SelectAll();

        private void ExecuteGoToUrl()
        {
            if (string.IsNullOrWhiteSpace(Url)) return;

            string targetUrl = Url.Trim().ToLower();

            if (!targetUrl.StartsWith("http://") && !targetUrl.StartsWith("https://"))
            {
                if (targetUrl.Contains("."))
                {
                    Url = "https://" + targetUrl;
                }
                else
                {
                    Url = "https://www.google.com/search?q=" + Uri.EscapeDataString(targetUrl);
                }
            }
            UpdateSecurityStatus(Url);
            NavigationRequested?.Invoke("GoTo");
        }



    }
}