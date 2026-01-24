using PRERP_TESTER.ViewModels;
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

using PRERP_TESTER.Models;

namespace PRERP_TESTER.Views
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            
            InitializeComponent();
        }

        private void ModuleItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement fe) return;
            if (fe.DataContext is not Models.ModuleItem module) return;

            if (DataContext is ShellViewModel vm)
                vm.SelectModule(module);

            // điều hướng frame nếu cần
        }
    }
}
