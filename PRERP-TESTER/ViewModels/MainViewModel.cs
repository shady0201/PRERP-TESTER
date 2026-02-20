using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using PRERP_TESTER.Extensions;
using PRERP_TESTER.Helper;
using PRERP_TESTER.Messages;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;
using PRERP_TESTER.Views.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PRERP_TESTER.ViewModels
{
    public class MainViewModel : LazyLoadViewModel
    {
        private ObservableCollection<Account> _accounts = [];
        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }


        private Account? _selectedPopupAccount;
        public Account? SelectedPopupAccount
        {
            get => _selectedPopupAccount;
            set => SetProperty(ref _selectedPopupAccount, value);
        }
        private bool _isAccountPopupOpen;
        public bool IsAccountPopupOpen
        {
            get => _isAccountPopupOpen;
            set => SetProperty(ref _isAccountPopupOpen, value);
        }

        private string _searchAccountText;
        public string SearchAccountText
        {
            get => _searchAccountText;
            set
            {
                if (SetProperty(ref _searchAccountText, value))
                {
                    AccountMenuView.Refresh();
                }
            }
        }

        private string _searchModuleText;
        public string SearchModuleText
        {
            get => _searchModuleText;
            set
            {
                if (SetProperty(ref _searchModuleText, value))
                {
                    ModuleMenuView?.Refresh();
                }
            }
        }

        public ICollectionView AccountMenuView { get; }

        public ICollectionView ModuleMenuView { get; }

        private bool _isMenuCollapsed;
        public bool IsMenuCollapsed
        {
            get => _isMenuCollapsed;
            set => SetProperty(ref _isMenuCollapsed, value);
        }
            
        private bool _isModuleExpanded = true;
        public bool IsModuleExpanded
        {
            get => _isModuleExpanded;
            set => SetProperty(ref _isModuleExpanded, value);
        }

        private bool _isDarkMode = true;
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set => SetProperty(ref _isDarkMode, value);
        }

        private bool _isAccountExpanded = true;
        public bool IsAccountExpanded
        {
            get => _isAccountExpanded;
            set => SetProperty(ref _isAccountExpanded, value);
        }

        private ServerType _serverType;
        public ServerType ServerType
        {
            get => _serverType;
            set 
            {
                if (SetProperty(ref _serverType, value))
                {
                    UpdateCurrentServer();
                }
            }
        }

        private bool _isAccountListCollapsed = true;
        public bool IsAccountListCollapsed
        {
            get => _isAccountListCollapsed;
            set => SetProperty(ref _isAccountListCollapsed, value);
        }

        public ICommand ToggleAccountListCommand { get; }

        public ObservableCollection<ModuleViewModel> Modules { get; set; } = [];

        public ModuleViewModel? SelectedModule { get; set; }

        // command
        public ICommand CreateModuleCommand { get; }
        public ICommand CreateAccountCommand { get; }
        public ICommand ToggleMenuCommand { get; }
        public ICommand EditAccountCommand { get; }
        public ICommand RemoveAccountCommand { get; }
        public ICommand RemoveModuleCommand { get; }
        public ICommand PinModuleCommand { get; }

        public ICommand ExportDataCommand { get; }
        public ICommand ImportDataCommand { get; }

        public ICommand ToggleModuleExpandCommand { get; }
        public ICommand ToggleAccountExpandCommand { get; }
        public ICommand TestCommand { get; }
        public ICommand ShowAccountDetailCommand { get; }
        public static MainViewModel Instance { get; private set; }

        public ObservableCollection<HistoryItem> History { get; set; } = new();
        public MainViewModel()
        {
            Instance = this;
            LoadAllData();
            // Commands
            CreateModuleCommand = new RelayCommand(ExecuteCreateModule);
            RemoveModuleCommand = new RelayCommand<ModuleViewModel>(ExecuteRemoveModule);
            PinModuleCommand = new RelayCommand<ModuleViewModel>(ExecutePinModule);
            CreateAccountCommand = new RelayCommand(ExecuteCreateAccount);
            EditAccountCommand = new RelayCommand<Account>(ExecuteEditAccount);
            RemoveAccountCommand = new RelayCommand<Account>(ExecuteRemoveAccount);

            ToggleMenuCommand = new RelayCommand(() => IsMenuCollapsed = !IsMenuCollapsed);

            ExportDataCommand = new RelayCommand(ExecuteExportData);
            ImportDataCommand = new RelayCommand(ExecuteImportData);

            ToggleModuleExpandCommand = new RelayCommand(() => IsModuleExpanded = !IsModuleExpanded);
            ToggleAccountExpandCommand = new RelayCommand(() => IsAccountExpanded = !IsAccountExpanded);

            ShowAccountDetailCommand = new RelayCommand<Account>(ShowAccountDetail);

            ToggleAccountListCommand = new RelayCommand(() => IsAccountListCollapsed = !IsAccountListCollapsed);

            TestCommand = new RelayCommand(ExecuteTest);

            // Data
            AccountMenuView = new ListCollectionView(Accounts);
            AccountMenuView.Filter = FilterAccountMenu;

            ModuleMenuView = CollectionViewSource.GetDefaultView(Modules);
            ModuleMenuView.Filter = FilterModuleMenu;

            // Sort data
            ApplySort();

            // message popup
            WeakReferenceMessenger.Default.Register<ShowAccountDetailMessage>(this, (r, m) =>
            {
                SelectedPopupAccount = m.Value;
                IsAccountPopupOpen = true;
            });
        }

        public void ShowAccountDetail(Account account)
        {
            if (account == null) return;
            WeakReferenceMessenger.Default.Send(new ShowAccountDetailMessage(account));
        }

        private void ExecuteTest()
        {
            
        }

        private void ExecuteCreateModule()
        {
            var dialog = new AddModuleDialog(Modules) { Owner = Application.Current.MainWindow };

            if (dialog.ShowDialog() == true)
            {
                var moduleEntityCapp = new ModuleEntity
                {
                    Name = dialog.ResultName,
                    ServerType = ServerType.CAPP,
                    AccountModules = []
                };
                var moduleVMCapp = new ModuleViewModel(moduleEntityCapp, Accounts);

                Modules.Add(moduleVMCapp);
                var moduleEntityPrerp = new ModuleEntity
                {
                    Name = dialog.ResultName,
                    ServerType = ServerType.PRERP,
                    AccountModules = []
                };
                var moduleVMPrerp = new ModuleViewModel(moduleEntityPrerp, Accounts);

                Modules.Add(moduleVMPrerp);
                ToastService.Show("Tạo Module Thành Công", string.Format("Đã thêm \"{0}\" vào danh sách!", moduleEntityCapp.Name), ToastType.Success);
            }
        }
        private void ExecuteCreateAccount()
        {
            var dialog = new AddAccountDialog([..Accounts]) { Owner = Application.Current.MainWindow };

            if (dialog.ShowDialog() == true)
            {
                var account = new Account
                {
                    Id= Guid.NewGuid().ToString("N"),
                    Username = dialog.Username,
                    Password = dialog.Password,
                    DisplayName = dialog.DisplayName,
                    ServerType = dialog.DialogServerType,
                    Role = dialog.Role,
                };
                Accounts.Add(account);
                ToastService.Show("Tạo Account Thành Công", "", ToastType.Success);
            }
        }
        private void ExecuteEditAccount(Account account) {
            if (account == null) return;

            var dialog = new AddAccountDialog(account);
            dialog.Owner = Application.Current.MainWindow;

            if (dialog.ShowDialog() == true)
            {
                int index = Accounts.IndexOf(account);
                if (index >= 0)
                {
                    Accounts[index] = account;
                }
                SaveAllData();
            }

        }
        private void ExecuteRemoveAccount(Account account) {
            if (account == null) return;

            string displayName = string.IsNullOrWhiteSpace(account.DisplayName) ? account.Username : account.DisplayName;

            bool bConfirm = DialogService.ShowConfirm("Bạn có muốn xoá tài khoản?", $"Thao tác nay sẽ xoá toàn bộ tài khoản '{displayName}' khỏi các module đang sử dụng");

            if (bConfirm)
            {
                string SessionAccountFolder = account.SessionFolder;
                string account_id = account.Id;
                Accounts.Remove(account);
                foreach (var module in Modules)
                {
                    var accountVMM = module.ModuleAccounts.FirstOrDefault(vm => vm.Account.Id == account_id);

                    if (accountVMM != null)
                    {
                        module.RemoveAccount(accountVMM);
                    }
                }

                WebView2Extensions.RemoveFromCache(SessionAccountFolder);
                _= DeleteAccountSessionFolderAsync(SessionAccountFolder);

                AccountMenuView.Refresh();

                SaveAllData();
                ToastService.Show("Xoá tài khoản",String.Format("Tài khoản {0} đã xoá khỏi hệ thống", displayName), ToastType.Success);
            }
        }
        private void ExecuteRemoveModule(ModuleViewModel moduleViewModel)
        {
            if (moduleViewModel == null) return;

            bool bConfirm = DialogService.ShowConfirm("Bạn có muốn xoá Module này?", $"Thao tác này sẽ xoá module '{moduleViewModel.Name}' và các thông tin có trong module");

            if (bConfirm)
            {
                string targetName = moduleViewModel.Name;
                var modulesToDelete = Modules.Where(m => m.Name == targetName).ToList();

                foreach (var module in modulesToDelete)
                {
                    Modules.Remove(module);
                    if (SelectedModule == module)
                    {
                        SelectedModule = null;
                        SelectedModule = Modules.FirstOrDefault();
                        OnPropertyChanged(nameof(SelectedModule));
                    }
                }

                ModuleMenuView?.Refresh();

                SaveAllData();
            }
        }
        private void ExecutePinModule(ModuleViewModel moduleViewModel)
        {
            if (moduleViewModel == null) return;

            moduleViewModel.IsPinned = !moduleViewModel.IsPinned;
            ModuleMenuView?.Refresh();
            SaveAllData();
        }
        private async Task DeleteAccountSessionFolderAsync(string accountId, int delayMs = 3000)
        {
            await Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(delayMs);

                    string userDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sessions", $"Account_{accountId}");

                    if (Directory.Exists(userDataFolder))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            try
                            {
                                Directory.Delete(userDataFolder, true);
                                System.Diagnostics.Debug.WriteLine($"Successfully deleted StringJsonSession folder for: {accountId}");
                                break;
                            }
                            catch (IOException)
                            {
                                if (i == 2) throw;
                                await Task.Delay(1000);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogError(ex, "DeleteAccountSessionFolderAsync");
                    ToastService.Show("Lỗi xóa dữ liệu phiên", $"Không thể xóa dữ liệu phiên cho tài khoản {accountId}. Vui lòng kiểm tra quyền truy cập file.", ToastType.Error);
                }
            });
        }

        private bool FilterAccountMenu(object obj)
        {
            if (obj is Account acc)
            {
                if (acc.ServerType != ServerType) return false;

                if (string.IsNullOrWhiteSpace(SearchAccountText)) return true;

                string search = StringHelper.RemoveSign4VietnameseString(SearchAccountText.ToLower().Trim());
                string name = StringHelper.RemoveSign4VietnameseString(acc.DisplayName?.ToLower() ?? "");
                string user = StringHelper.RemoveSign4VietnameseString(acc.Username?.ToLower() ?? "");

                return name.Contains(search) || user.Contains(search);
            }
            return false;
        }

        private bool FilterModuleMenu(object obj)
        {
            if (obj is ModuleViewModel moduleVM)
            {
                if (moduleVM.ModuleEntity.ServerType != ServerType) return false;
                if (string.IsNullOrWhiteSpace(SearchModuleText)) return true;
                string search = StringHelper.RemoveSign4VietnameseString(SearchModuleText.ToLower().Trim());
                string moduleName = StringHelper.RemoveSign4VietnameseString(moduleVM.Name?.ToLower() ?? "");

                return moduleName.Contains(search);
            }
            return false;
        }

        public void LoadAllData()
        {
            Modules.Clear();

            var data = DataService.LoadData<ApplicationData>();

            Accounts = new ObservableCollection<Account>(data.Accounts);
            foreach (var acc in Accounts)
            {
                if (acc.IsSessionExpired())
                {
                    acc.CleanData();
                }
            }
            foreach (var entity in data.Modules)
            {
                Modules.Add(new ModuleViewModel(entity, Accounts));
            }

            History = [.. data.History];
            if (History.Count == 0)
            {
                DefaultHistory();
            }

            // Cập nhật server type
            ServerType = data.ServerType;

            UpdateCurrentServer();
        }

        private void ApplySort()
        {
            if (AccountMenuView != null)
            {
                AccountMenuView.SortDescriptions.Clear();
                AccountMenuView.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));
            }

            if (ModuleMenuView != null)
            {
                ModuleMenuView.SortDescriptions.Clear();
                ModuleMenuView.SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
                ModuleMenuView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
        }

        public void SaveAllData()
        {
            DataService.SaveData(GetAppData());
        }

        private ApplicationData GetAppData()
        {
            foreach (var moduleVM in Modules)
            {
                var accountModuleList = new List<AccountModule>();

                foreach (var accVM in moduleVM.ModuleAccounts)
                {
                    var tabEntities = accVM.TabViewModels.Select(t => new TabWeb
                    {
                        ModuleId = t.ModuleID,
                        AccountId = t.UserAccount.Username,
                        Title = t.Title,
                        Url = t.Url,
                        FaviconUrl = t.FaviconUrl
                    }).ToArray();

                    accountModuleList.Add(new AccountModule
                    {
                        AccountID = accVM.Account.Id,
                        TabWebItems = tabEntities
                    });
                }
                moduleVM.ModuleEntity.AccountModules = accountModuleList.ToArray();
            }

            return new ApplicationData
            {
                Accounts = Accounts.ToList(),
                Modules = Modules.Select(m => m.ModuleEntity).ToList(),
                ServerType = GobalSetting.ServerType,
                History = History.ToList(),
            };
        }

        private void UpdateCurrentServer()
        {
            GobalSetting.ServerType = ServerType;
            if (ServerType == ServerType.CAPP)
            {
                GobalSetting.CurrentBaseUrl = "https://capp.bmtu.edu.vn/mywork";
            }
            else if (ServerType == ServerType.PRERP)
            {
                GobalSetting.CurrentBaseUrl = "https://prerp.bmtu.edu.vn";
            }

            AccountMenuView?.Refresh();
            ModuleMenuView?.Refresh();

        }

        private void ExecuteImportData()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Chọn file dữ liệu để nhập"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    var importedData = Newtonsoft.Json.JsonConvert.DeserializeObject<ApplicationData>(json);

                    if (importedData == null) throw new Exception("File không đúng định dạng dữ liệu Prerp.");

                    foreach (var acc in importedData.Accounts)
                    {
                        var checkDuplicate = Accounts.Any(a => a.Username.Equals(acc.Username.Trim(), StringComparison.OrdinalIgnoreCase)
                                                    && a.ServerType == acc.ServerType);
                        if (!checkDuplicate)
                        {
                            Accounts.Add(acc);
                        }
                    }

                    foreach (var entity in importedData.Modules)
                    {
                        var checkDuplicateModule = Modules.Any(m => m.Name == entity.Name);
                        if (!checkDuplicateModule)
                        {
                            Modules.Add(new ModuleViewModel(entity, Accounts));
                        }
                    }
                    ServerType = importedData.ServerType;
                    UpdateCurrentServer();
                    SaveAllData();

                    ToastService.Show("Nhập dữ liệu thành công!", "Dữ liệu đã được nhập và lưu thành công.", ToastType.Success);
                }
                catch (Exception ex)
                {
                    LogService.LogError(ex, "MainViewModel - ExecuteImportData");
                    ToastService.Show("Lỗi khi nhập dữ liệu", $"Không thể nhập dữ liệu từ file: {ex.Message}", ToastType.Error);
                }
            }
        }

        private void ExecuteExportData()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                FileName = $"Data_TestApp_{DateTime.Now:yyyyMMdd_HHmm}",
                Title = "Xuất dữ liệu"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var data = GetAppData();
                    data.History = [];
                    string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    File.WriteAllText(saveFileDialog.FileName, json);

                    ToastService.Show("Xuất dữ liệu thành công!", "Dữ liệu đã được xuất ra file thành công.", ToastType.Success);
                }
                catch (Exception ex)
                {
                    LogService.LogError(ex, "MainViewModel - ExecuteExportData");
                    ToastService.Show("Lỗi khi xuất dữ liệu", $"Không thể xuất dữ liệu ra file: {ex.Message}", ToastType.Error);
                    
                }
            }
        }

        // Web history
        public void AddHistory(string title, string url ,string? faviconUrl)
        {
            if (string.IsNullOrWhiteSpace(url) || url.StartsWith("about:blank")) return;

            var existing = History.FirstOrDefault(h => h.Url == url);
            if (existing != null)
            {
                existing.LastVisited = DateTime.Now;
                existing.Title = title;
            }
            else
            {
                History.Add(new HistoryItem { Title = title, Url = url, FaviconBase = faviconUrl, LastVisited = DateTime.Now });
            }
            if (History.Count > 2000) History.RemoveAt(0);
        }

        private void DefaultHistory()
        {
            History.Add(new HistoryItem { Title = "PRERP BMU", Url = "https://prerp.bmtu.edu.vn", LastVisited = DateTime.Now });
            History.Add(new HistoryItem { Title = "CAPP BMU", Url = "https://capp.bmtu.edu.vn/mywork", LastVisited = DateTime.Now });
        }

        // Item mouse event
        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Đánh dấu sự kiện đã được xử lý (Handled = true)
            // Điều này sẽ chặn ListBox không nhận được tín hiệu nhấn chuột phải, 
            // do đó nó sẽ không thay đổi SelectedItem.
            e.Handled = true;

            // LƯU Ý: Vì chúng ta chặn sự kiện, ContextMenu có thể không tự mở.
            // Chúng ta sẽ ép ContextMenu mở thủ công khi nhấn chuột phải.
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem != null)
            {
                // Tìm Border hoặc Content bên trong có chứa ContextMenu
                // Ở đây chúng ta tìm đến Border đầu tiên bên trong ListBoxItem
                var frameworkElement = listBoxItem.ContentTemplate.LoadContent() as FrameworkElement;

                // Hoặc đơn giản hơn, nếu ContextMenu nằm trên Border mà bạn đã gán Tag:
                if (listBoxItem.ContextMenu != null)
                {
                    listBoxItem.ContextMenu.PlacementTarget = listBoxItem;
                    listBoxItem.ContextMenu.IsOpen = true;
                }
                else
                {
                    // Nếu bạn đặt ContextMenu bên trong DataTemplate (trên Border như đoạn code trước)
                    // Chúng ta cần tìm element đó và mở menu của nó.
                    // Nhưng cách nhanh nhất là dùng ContextMenuService:
                    var border = FindVisualChild<Border>(listBoxItem); // Hàm bổ trợ tìm Border
                    if (border != null && border.ContextMenu != null)
                    {
                        border.ContextMenu.PlacementTarget = border;
                        border.ContextMenu.IsOpen = true;
                    }
                }
            }
        }

        // Hàm bổ trợ để tìm Border bên trong ListBoxItem (nếu cần)
        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T t) return t;
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }

    }
}