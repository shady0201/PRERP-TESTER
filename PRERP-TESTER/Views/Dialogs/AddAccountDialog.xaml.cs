using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (!string.IsNullOrWhiteSpace(TxtUsername.Text))
            {
                Username = TxtUsername.Text;
                Password = TxtPassword.Text;
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

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
