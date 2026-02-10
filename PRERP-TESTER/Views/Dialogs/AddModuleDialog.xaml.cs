using PRERP_TESTER.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PRERP_TESTER.Views.Dialogs
{
    public partial class AddModuleDialog : Window
    {
        public string ResultName { get; private set; }

        private List<ModuleViewModel> ExistingModules;

        public AddModuleDialog(ObservableCollection<ModuleViewModel> Modules)
        {
            ExistingModules = [.. Modules];
            InitializeComponent();
            TxtModuleName.Focus();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtModuleName.Text))
            {
               if (ExistingModules.Any(m => m.Name == TxtModuleName.Text.Trim()))
                {
                    // TODO: Tạo valid cho form
                    return;
                }
                ResultName = TxtModuleName.Text.Trim();
                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TxtModuleName_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtModuleName.SelectAll();
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