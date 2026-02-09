using PRERP_TESTER.Models;
using PRERP_TESTER.Services;
using PRERP_TESTER.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PRERP_TESTER.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();

            this.SizeChanged += (s, e) => ResetPopupPosition();
            this.LocationChanged += (s, e) => ResetPopupPosition();
        }

        private void ResetPopupPosition()
        {
            var offset = ToastPopup.VerticalOffset;
            ToastPopup.VerticalOffset = offset + 0.01;
            ToastPopup.VerticalOffset = offset;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(btn);
                while (!(parent is Border)) { parent = VisualTreeHelper.GetParent(parent); }

                Border border = parent as Border;
                if (border != null && border.ContextMenu != null)
                {
                    border.ContextMenu.PlacementTarget = border;
                    border.ContextMenu.IsOpen = true;
                }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SaveAllData();
            }
        }
    }


}
