using PRERP_TESTER.Helper;
using PRERP_TESTER.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PRERP_TESTER.Views.Dialogs
{
    public partial class AccountPickerDialog : Window
    {
        private ICollectionView _accountsView;
        public List<Account> SelectedAccounts { get; private set; } = [];
        public ObservableCollection<Account> Accounts { get; set; } = [];
        public AccountPickerDialog(ObservableCollection<Account> accounts, List<string> moduleAccounts)
        {
            InitializeComponent();
            Accounts = [.. accounts.Where(acc => acc.ServerType == GobalSetting.ServerType && !moduleAccounts.Contains(acc.Id))];
            DataContext = this;

            _accountsView = new ListCollectionView(Accounts);
            _accountsView.Filter = FilterAccounts;

            AccountListBox.ItemsSource = _accountsView;

        }

        private bool FilterAccounts(object obj)
        {
            
            if (obj is Account acc)
            {
                if (TxtSearch == null || string.IsNullOrEmpty(TxtSearch.Text))
                    return true;

                if (acc.ServerType != GobalSetting.ServerType) return false;

                string searchText = StringHelper.RemoveSign4VietnameseString(TxtSearch.Text.Trim().ToLower());
                string displayName = StringHelper.RemoveSign4VietnameseString(acc.DisplayName?.ToLower() ?? "");
                string username = StringHelper.RemoveSign4VietnameseString(acc.Username?.ToLower() ?? "");

                return displayName.Contains(searchText) || username.Contains(searchText);
            }
            return false;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _accountsView.Refresh();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in AccountListBox.SelectedItems)
            {
                if (item is Account acc) SelectedAccounts.Add(acc);
            }
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
