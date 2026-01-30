using System;
using System.Collections.Generic;
using System.Linq;
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
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string DisplayName { get; private set; }
        public string Stype { get; private set; }

        public AddAccountDialog()
        {
            InitializeComponent();

        }
        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtUsername.Text) && !string.IsNullOrWhiteSpace(TxtPassword.Password))
            {
                Username = TxtUsername.Text;
                Password = TxtPassword.Password;
                DisplayName = TxtDisplayName.Text;
                Stype = RbStaff.IsChecked == true ? "STAFF" : "STUDENT";
                DialogResult = true;
                Close();
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
