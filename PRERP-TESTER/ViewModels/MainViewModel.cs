using CommunityToolkit.Mvvm.Input;
using ControlzEx.Standard;
using PRERP_TESTER.Extensions;
using PRERP_TESTER.Helper;
using PRERP_TESTER.Models;
using PRERP_TESTER.Services;
using PRERP_TESTER.Views.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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

        private int _filteredAccountCount;
        public int FilteredAccountCount
        {
            get => _filteredAccountCount;
            set => SetProperty(ref _filteredAccountCount, value);
        }
        public ICollectionView ModuleMenuView { get; }

        private int _filteredModuleCount;
        public int FilteredModuleCount
        {
            get => _filteredModuleCount;
            set => SetProperty(ref _filteredModuleCount, value);
        }

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

            // Data
            AccountMenuView = new ListCollectionView(Accounts);
            AccountMenuView.Filter = FilterAccountMenu;

            ModuleMenuView = CollectionViewSource.GetDefaultView(Modules);
            ModuleMenuView.Filter = FilterModuleMenu;

            // Sort data
            ApplySort();
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
                FilteredModuleCount = Modules.Count(m => m.ModuleEntity.ServerType == ServerType);
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
                FilteredAccountCount = Accounts.Count(a => a.ServerType == ServerType);
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

            var result = MessageBox.Show($"Xóa vĩnh viễn tài khoản {account.DisplayName} khỏi hệ thống?",
                                        "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string SessionAccountFolder = account.SessionFolder;
                string account_id = account.Id;
                Accounts.Remove(account);
                FilteredAccountCount = Accounts.Count(a => a.ServerType == ServerType);
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
                ToastService.Show("Tài khoản đã được xoá khỏi danh sách","", ToastType.Success);
            }
        }
        private void ExecuteRemoveModule(ModuleViewModel moduleViewModel)
        {
            if (moduleViewModel == null) return;

            var result = MessageBox.Show(
                        $"Xóa vĩnh viễn Module '{moduleViewModel.Name}' và toàn bộ các Tab đang mở bên trong?",
                        "Cảnh báo xóa Module",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string targetName = moduleViewModel.Name;
                var modulesToDelete = Modules.Where(m => m.Name == targetName).ToList();

                foreach (var module in modulesToDelete)
                {
                    Modules.Remove(module);
                    FilteredModuleCount = Modules.Count(m => m.ModuleEntity.ServerType == ServerType);
                    if (SelectedModule == module)
                    {
                        SelectedModule = null;
                        SelectedModule = Modules.FirstOrDefault();
                        OnPropertyChanged(nameof(SelectedModule));
                    }
                }

                ModuleMenuView?.Refresh();

                OnPropertyChanged(nameof(FilteredModuleCount));

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
                                System.Diagnostics.Debug.WriteLine($"Successfully deleted session folder for: {accountId}");
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
                    // TODO: Hiển thị lỗi cho người dùng
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
            UpdateFilteredCounts();


        }

        private void UpdateFilteredCounts()
        {
            FilteredAccountCount = Accounts.Count(a => a.ServerType == ServerType);
            FilteredModuleCount = Modules.Count(m => m.ModuleEntity.ServerType == ServerType);
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

                // TODO: Hiển thị thông báo thành công cho người dùng
                MessageBox.Show("Nhập dữ liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
                catch (Exception ex)
                {
                    LogService.LogError(ex, "MainViewModel - ExecuteImportData");
                    // TODO: Hiển thị lỗi cho người dùng
                    MessageBox.Show($"Lỗi khi nhập file: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    // Không xuất lịch sử duyệt web
                    data.History = [];
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(saveFileDialog.FileName, json);

                    MessageBox.Show("Xuất dữ liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    LogService.LogError(ex, "MainViewModel - ExecuteExportData");
                    // TODO: Hiển thị lỗi cho người dùng
                    MessageBox.Show($"Lỗi khi xuất file: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            // Giới hạn khoảng 1000-2000 mục để tránh file JSON quá nặng
            if (History.Count > 2000) History.RemoveAt(0);
        }

        private void DefaultHistory()
        {
            History.Add(new HistoryItem { Title = "PRERP BMU", Url = "https://prerp.bmtu.edu.vn", LastVisited = DateTime.Now });
            History.Add(new HistoryItem { Title = "CAPP BMU", Url = "https://capp.bmtu.edu.vn/mywork", LastVisited = DateTime.Now });
        }

    }
}