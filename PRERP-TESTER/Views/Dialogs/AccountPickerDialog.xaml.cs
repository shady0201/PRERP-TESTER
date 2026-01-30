using PRERP_TESTER.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Interaction logic for AccountPickerDialog.xaml
    /// </summary>
    public partial class AccountPickerDialog : Window
    {
        public List<Account> SelectedAccounts { get; private set; } = [];
        public ObservableCollection<Account> Accounts { get; set; } = [];
        public AccountPickerDialog(ObservableCollection<Account> accounts, List<string> moduleAccounts)
        {
            Accounts = accounts;
            InitializeComponent();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
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
