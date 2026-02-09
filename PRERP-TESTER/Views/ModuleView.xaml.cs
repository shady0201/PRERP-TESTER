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
        private Point _startPoint;

        private DragAdorner _ghostTab;
        private AdornerLayer _adornerLayer;
        public ModuleView()
        {
            InitializeComponent();
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DependencyObject originalSource = e.OriginalSource as DependencyObject;
                if (FindAncestor<Button>(originalSource) != null)
                {
                    return;
                }

                var listBox = sender as ListBox;
                var item = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (item != null && _ghostTab == null)
                {
                    var layer = AdornerLayer.GetAdornerLayer(listBox);
                    if (layer == null) return;

                    // Tạo ghost tab
                    _ghostTab = new DragAdorner(listBox, item);
                    layer.Add(_ghostTab);

                    // Đăng ký sự kiện cập nhật vị trí
                    listBox.GiveFeedback += ListBox_GiveFeedback;

                    var data = new DataObject("AccountVM", item.DataContext);
                    DragDrop.DoDragDrop(item, data, DragDropEffects.Move);

                    // Sau khi thả chuột, dọn dẹp hiện trường
                    listBox.GiveFeedback -= ListBox_GiveFeedback;
                    layer.Remove(_ghostTab);
                    _ghostTab = null;
                }
            }
        }

        private void ListBox_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (_ghostTab != null)
            {
                // Lấy vị trí chuột hiện tại so với ListBox
                var listBox = sender as ListBox;
                Point mousePos = Mouse.GetPosition(listBox);
                _ghostTab.UpdatePosition(mousePos);
            }
        }

        private void ListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("AccountVM"))
            {
                ListBox listBox = sender as ListBox;
                AccountViewModel draggedData = e.Data.GetData("AccountVM") as AccountViewModel;

                // Tìm item bên dưới con trỏ chuột hiện tại
                ListBoxItem targetItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (targetItem != null)
                {
                    AccountViewModel targetData = targetItem.DataContext as AccountViewModel;
                    var viewModel = listBox.DataContext as ModuleViewModel;

                    if (viewModel != null && draggedData != null)
                    {
                        int oldIndex = viewModel.ModuleAccounts.IndexOf(draggedData);
                        int newIndex = viewModel.ModuleAccounts.IndexOf(targetData);

                        // Nếu di chuyển sang vị trí mới, thực hiện Move ngay lập tức
                        if (oldIndex != -1 && newIndex != -1 && oldIndex != newIndex)
                        {
                            viewModel.MoveAccount(oldIndex, newIndex);
                        }
                    }
                }
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        // Hàm helper để tìm phần tử cha (ListBoxItem) từ vị trí click
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T ancestor) return ancestor;
                current = VisualTreeHelper.GetParent(current);
            } while (current != null);
            return null;
        }
    }

}
