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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PRERP_TESTER.Views
{

    public partial class ModuleView : UserControl
    {
        public ModuleView()
        {
            InitializeComponent();
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy ViewModel từ DataContext
            if (this.DataContext is ModuleViewModel vm)
            {
                // Gọi hàm xử lý logic lazy load
                if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabViewModel selectedTab)
                {
                    selectedTab.IsLoaded = true;
                }
            }
        }
    }

    }
