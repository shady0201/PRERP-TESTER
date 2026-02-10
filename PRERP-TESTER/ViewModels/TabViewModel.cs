using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using System.Web;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using PRERP_TESTER.Services;
using System.Collections.ObjectModel;
using Microsoft.Web.WebView2.Wpf; // Namespace chứa các entity của bạn

namespace PRERP_TESTER.ViewModels
{
    public class TabViewModel : LazyLoadViewModel
    {
        public string ModuleID { get; set; }

        public Account UserAccount { get; }
        public TabWeb TabData { get; }


        public bool IsDefaultPageVisible => string.IsNullOrWhiteSpace(Url) || Url.Equals("about:blank", StringComparison.OrdinalIgnoreCase);

        private string _url;
        public string Url
        {
            get => _url;
            set
            {
                if (SetProperty(ref _url, value))
                {
                    FilterSuggestions(value);
                    OnPropertyChanged(nameof(IsDefaultPageVisible));
                }
            }
        }
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool? _isSecure;
        public bool? IsSecure
        {
            get => _isSecure;
            set => SetProperty(ref _isSecure, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string? _faviconUrl = null;
        public string? FaviconUrl
        {
            get => _faviconUrl;
            set {
                if (SetProperty(ref _faviconUrl, value))
                {
                    OnPropertyChanged(nameof(HasFavicon));
                    if (TabData != null) TabData.FaviconUrl = value;
                }
            }
        }
        public bool HasFavicon => !string.IsNullOrEmpty(FaviconUrl);

        private void UpdateSecurityStatus(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                IsSecure = null;
            }
            else
            {
                IsSecure = url.ToLower().StartsWith("https://");
            }
        }

        public ICommand CloseTabCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ForwardCommand { get; }
        public ICommand ReloadCommand { get; }
        public ICommand GoToUrlCommand { get; }

        // Tab menucontext
        public ICommand CloseOtherTabsCommand { get; }
        public ICommand DuplicateTabCommand { get; }

        // history
        public ICommand RemoveSuggestionCommand { get; }

        public event Action<TabViewModel>? RequestCloseOthers;
        public event Action<TabViewModel>? RequestDuplicate;

        public event Action<string>? NavigationRequested;

        private bool _isSuggestionOpen;
        public bool IsSuggestionOpen
        {
            get => _isSuggestionOpen;
            set => SetProperty(ref _isSuggestionOpen, value);
        }

        private ObservableCollection<HistoryItem> _suggestions = new();
        public ObservableCollection<HistoryItem> Suggestions
        {
            get => _suggestions;
            set => SetProperty(ref _suggestions, value);
        }

        private HistoryItem _selectedSuggestion;
        public HistoryItem SelectedSuggestion
        {
            get => _selectedSuggestion;
            set => SetProperty(ref _selectedSuggestion, value);
        }

        public event Action<TabViewModel>? OnTabClosed;

        public TabViewModel(TabWeb tabWeb, Account account, string moduleID, Action<TabViewModel> closeAction)
        {
            UserAccount = account;
            Url = StandardizationUrl(tabWeb.Url);
            UpdateSecurityStatus(Url);
            Title = tabWeb.Title;
            FaviconUrl = tabWeb.FaviconUrl;
            TabData = tabWeb;
            IsLoaded = false;
            ModuleID = moduleID;

            BackCommand = new RelayCommand(() => NavigationRequested?.Invoke("Back"));
            ForwardCommand = new RelayCommand(() => NavigationRequested?.Invoke("Forward"));
            ReloadCommand = new RelayCommand(() => NavigationRequested?.Invoke("Reload"));
            GoToUrlCommand = new RelayCommand(ExecuteGoToUrl);

            //history
            RemoveSuggestionCommand = new RelayCommand<HistoryItem>(ExecuteRemoveSuggestion);

            // Tab menucontext
            CloseOtherTabsCommand = new RelayCommand(() => {
                RequestCloseOthers?.Invoke(this);
            });

            DuplicateTabCommand = new RelayCommand(() => {
                RequestDuplicate?.Invoke(this);
            });

            CloseTabCommand = new RelayCommand(() => {
                closeAction(this);
                OnTabClosed?.Invoke(this);
            });
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)=> (sender as TextBox)?.SelectAll();

        private void ExecuteGoToUrl()
        {
            if (string.IsNullOrWhiteSpace(Url)) return;

            Url = StandardizationUrl(Url);

            UpdateSecurityStatus(Url);
            NavigationRequested?.Invoke("GoTo");
        }

        private void ExecuteRemoveSuggestion(HistoryItem item)
        {
            if (item == null) return;

            Suggestions.Remove(item);

            var historyItem = MainViewModel.Instance.History
                .FirstOrDefault(h => h.Url == item.Url && h.Title == item.Title);

            if (historyItem != null)
            {
                MainViewModel.Instance.History.Remove(historyItem);
            }
        }

        public void Cleanup()
        {
            try
            {
                NavigationRequested = null;
                OnTabClosed = null;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, "TabViewModel.Cleanup");
            }
        }

        public void ConfirmSuggestion(HistoryItem item)
        {
            if (item == null) return;

            Url = item.Url;
            IsSuggestionOpen = false;
            ExecuteGoToUrl();
        }

        private void FilterSuggestions(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Contains("://"))
            {
                IsSuggestionOpen = false;
                return;
            }

            var filtered = MainViewModel.Instance.History
                .Where(h => h.Url.ToLower().Contains(input.ToLower()))
                .Take(10)
                .ToList();

            if (filtered.Any())
            {
                Suggestions = new ObservableCollection<HistoryItem>(filtered);
                IsSuggestionOpen = true;
            }
            else
            {
                IsSuggestionOpen = false;
            }
        }

        public static string StandardizationUrl(string? input_url)
        {
            if (input_url == null)
            {
                return input_url;
            }
            string targetUrl = input_url.Trim().ToLower();
            string result = input_url;

            if (!targetUrl.StartsWith("http://") && !targetUrl.StartsWith("https://"))
            {
                if (targetUrl.Contains("."))
                {
                    result = "https://" + targetUrl;
                }
                else
                {
                    if(!String.IsNullOrWhiteSpace(targetUrl))
                    {
                        result = "https://www.google.com/search?q=" + Uri.EscapeDataString(targetUrl);
                    }
                }
            }
            return result;
        }

        // Blank tab



    }
}