using MahApps.Metro.IconPacks;
using PRERP_TESTER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PRERP_TESTER.Views.Dialogs
{
    public partial class AddAccountDialog : Window
    {
        private Account _editingAccount;
        private Account[] Accounts;

        public string Username { get; private set; }
        public string Password { get; private set; }
        public string DisplayName { get; private set; }
        public AccountRole Role { get; private set; }
        public ServerType DialogServerType { get; private set; }

        public AddAccountDialog(Account[] accounts)
        {
            Accounts = accounts;
            InitializeComponent();

            if (GobalSetting.ServerType == ServerType.CAPP)
                RbCapp.IsChecked = true;
            else
                RbPrerp.IsChecked = true;
        }
        public AddAccountDialog(Account account) : this(accounts: Array.Empty<Account>())
        {
            _editingAccount = account;

            Title_lable.Text = "Chỉnh Sửa Tài Khoản";
            BtnConfirm.Content = "Lưu thay đổi";

            TxtUsername.Text = account.Username;
            TxtPassword.Password = account.Password;
            TxtPasswordVisible.Text = account.Password;
            TxtDisplayName.Text = account.DisplayName;
            TxtUsername.IsEnabled = false;
            BorderUserName.Background = Brushes.Transparent;

            if (account.Role == AccountRole.STUDENT)
                RbStudent.IsChecked = true;
            else
                RbStaff.IsChecked = true;

            if (account.ServerType == ServerType.CAPP)
                RbCapp.IsChecked = true;
            else
                RbPrerp.IsChecked = true;
        }
        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string password = GetPassword();
            if (_editingAccount != null)
            {
                if(!string.IsNullOrEmpty(password))
                {
                    _editingAccount.Password = GetPassword();
                    _editingAccount.DisplayName = TxtDisplayName.Text;
                    _editingAccount.Role = RbStudent.IsChecked == true ? AccountRole.STUDENT : AccountRole.STAFF;
                    _editingAccount.ServerType = RbCapp.IsChecked == true ? ServerType.CAPP : ServerType.PRERP;
                    DialogResult = true;
                    Close();
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(TxtUsername.Text) && !string.IsNullOrWhiteSpace(password))
                {
                    DialogServerType = RbCapp.IsChecked == true ? ServerType.CAPP : ServerType.PRERP;
                    bool checkDuplicate = Accounts.Any(acc => acc.Username.Equals(TxtUsername.Text.Trim(), StringComparison.OrdinalIgnoreCase)
                                                && acc.ServerType == DialogServerType);
                    if (checkDuplicate)
                    {
                        MessageBox.Show("Tên đăng nhập đã tồn tại trên "+ "" + ". Vui lòng chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    Username = TxtUsername.Text;
                    Password = GetPassword();
                    DisplayName = TxtDisplayName.Text;
                    Role = RbStaff.IsChecked == true ? AccountRole.STAFF : AccountRole.STUDENT;
                    DialogServerType = RbCapp.IsChecked == true ? ServerType.CAPP : ServerType.PRERP;
                    DialogResult = true;
                    Close();
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TxtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtUsername.SelectAll();
        }

        private void BtnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (BtnShowPassword.IsChecked == true)
            {
                IconEye.Kind = PackIconMaterialKind.Eye;
                TxtPasswordVisible.Text = TxtPassword.Password;
                TxtPassword.Visibility = Visibility.Collapsed;
                TxtPasswordVisible.Visibility = Visibility.Visible;
                TxtPasswordVisible.Focus();
            }
            else
            {
                IconEye.Kind = PackIconMaterialKind.EyeOff;
                TxtPassword.Password = TxtPasswordVisible.Text;
                TxtPasswordVisible.Visibility = Visibility.Collapsed;
                TxtPassword.Visibility = Visibility.Visible;
                TxtPassword.Focus();
            }
        }

        private string GetPassword()
        {
            return BtnShowPassword.IsChecked == true ? TxtPasswordVisible.Text : TxtPassword.Password;
        }

        private void TxtUsername_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9.@_-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TxtUsername_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void TxtUsername_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                Regex regex = new Regex(@"^[a-zA-Z0-9.@_-]+$");

                if (!regex.IsMatch(text))
                {
                    e.CancelCommand(); // Hủy thao tác dán nếu chuỗi chứa ký tự lạ
                }
            }
            else
            {
                e.CancelCommand();
            }
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
