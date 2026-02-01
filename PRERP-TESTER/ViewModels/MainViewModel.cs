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
            public int FilteredAccountCount => AccountMenuView?.Cast<object>().Count() ?? 0;
            public ICollectionView ModuleMenuView { get; }
            public int FilteredModuleCount => ModuleMenuView?.Cast<object>().Count() ?? 0;

        private bool _isMenuCollapsed;
            public bool IsMenuCollapsed
            {
                get => _isMenuCollapsed;
                set => SetProperty(ref _isMenuCollapsed, value);
            }

            private string _serverType;
            public string ServerType
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

            public MainViewModel()
            {

                LoadAllData();

                // Commands
                CreateModuleCommand = new RelayCommand(ExecuteCreateModule);
                RemoveModuleCommand = new RelayCommand<ModuleViewModel>(ExecuteRemoveModule);
                CreateAccountCommand = new RelayCommand(ExecuteCreateAccount);
                EditAccountCommand = new RelayCommand<Account>(ExecuteEditAccount);
                RemoveAccountCommand = new RelayCommand<Account>(ExecuteRemoveAccount);

                ToggleMenuCommand = new RelayCommand(() => IsMenuCollapsed = !IsMenuCollapsed);

                AccountMenuView = new ListCollectionView(Accounts);
                AccountMenuView.Filter = FilterAccountMenu;

                ModuleMenuView = CollectionViewSource.GetDefaultView(Modules);
                ModuleMenuView.Filter = FilterModuleMenu;
            }

            private void ExecuteCreateModule()
            {
                var dialog = new AddModuleDialog(Modules) { Owner = Application.Current.MainWindow };

                if (dialog.ShowDialog() == true)
                {
                    var moduleEntityCapp = new ModuleEntity
                    {
                        Name = dialog.ResultName,
                        ServerType = "CAPP",
                        AccountModules = []
                    };
                    var moduleVMCapp = new ModuleViewModel(moduleEntityCapp, Accounts);

                    Modules.Add(moduleVMCapp);
                    var moduleEntityPrerp = new ModuleEntity
                    {
                        Name = dialog.ResultName,
                        ServerType = "PRERP",
                        AccountModules = []
                    };
                    var moduleVMPrerp = new ModuleViewModel(moduleEntityPrerp, Accounts);

                    Modules.Add(moduleVMPrerp);
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
                        ServerType = dialog.ServerType,
                        Stype = dialog.Stype,
                    };
                    Accounts.Add(account);
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
                    string targetServer = moduleViewModel.ModuleEntity.ServerType;
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

                    OnPropertyChanged(nameof(FilteredModuleCount));

                    SaveAllData();
                }
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
                    System.Diagnostics.Debug.WriteLine($"Background Delete Error: {ex.Message}");
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
                var data = DataService.LoadData<ApplicationData>();
                Accounts = new ObservableCollection<Account>(data.Accounts);
                Modules.Clear();
                foreach (var entity in data.Modules)
                {
                    Modules.Add(new ModuleViewModel(entity, Accounts));
                }

                // Cập nhật URL base
                ServerType = data.ServerType;
                UpdateCurrentServer();
            }

            public void SaveAllData()
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
                        }).ToArray();

                        accountModuleList.Add(new AccountModule
                        {
                            AccountID = accVM.Account.Id,
                            TabWebItems = tabEntities
                        });
                    }
                    moduleVM.ModuleEntity.AccountModules = accountModuleList.ToArray();
                }

                var data = new ApplicationData
                {
                    Accounts = Accounts.ToList(),
                    Modules = Modules.Select(m => m.ModuleEntity).ToList(),
                    ServerType = GobalSetting.ServerType
                };

                DataService.SaveData(data);
            }

            private void UpdateCurrentServer()
            {
                GobalSetting.ServerType = ServerType;
                if (ServerType == "CAPP")
                {
                    GobalSetting.CurrentBaseUrl = "https://capp.bmtu.edu.vn/mywork";
                }
                else if (ServerType == "PRERP")
                {
                    GobalSetting.CurrentBaseUrl = "https://prerp.bmtu.edu.vn";
                }

                AccountMenuView?.Refresh();
                ModuleMenuView?.Refresh();
            }
        }
    }