using System.Windows;

namespace PRERP_TESTER.Views.Dialogs
{
    public partial class AddModuleDialog : Window
    {
        public string ResultName { get; private set; }

        public AddModuleDialog()
        {
            InitializeComponent();
            TxtModuleName.Focus(); // Tự động focus vào ô nhập khi mở lên
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtModuleName.Text))
            {
                ResultName = TxtModuleName.Text;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TxtModuleName_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtModuleName.SelectAll(); // Tự động bôi đen để dễ nhập mới
        }
    }
}