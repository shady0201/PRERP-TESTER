using PRERP_TESTER.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PRERP_TESTER.Views.UserControls
{
    public partial class ModuleView : UserControl
    {
        public ModuleView()
        {
            InitializeComponent();
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DependencyObject originalSource = e.OriginalSource as DependencyObject;

                // Tránh kích hoạt kéo thả khi nhấn vào nút Xóa
                if (FindAncestor<Button>(originalSource) != null) return;

                var item = FindAncestor<ListBoxItem>(originalSource);

                if (item != null)
                {
                    // Khởi tạo dữ liệu kéo thả
                    var data = new DataObject("AccountVM", item.DataContext);
                    DragDrop.DoDragDrop(item, data, DragDropEffects.Move);
                }
            }
        }

        // AccountVM

        private void ListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("AccountVM"))
            {
                ListBox? listBox = sender as ListBox;
                AccountViewModel draggedData = e.Data.GetData("AccountVM") as AccountViewModel;
                ListBoxItem targetItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (targetItem != null && listBox != null)
                {
                    var targetData = targetItem.DataContext as AccountViewModel;
                    var viewModel = listBox.DataContext as ModuleViewModel;

                    if (viewModel != null && draggedData != null && targetData != draggedData)
                    {
                        int oldIndex = viewModel.ModuleAccounts.IndexOf(draggedData);
                        int newIndex = viewModel.ModuleAccounts.IndexOf(targetData);

                        if (oldIndex != -1 && newIndex != -1 && oldIndex != newIndex)
                        {
                            Point relativeMousePos = e.GetPosition(targetItem);
                            bool shouldMove = false;

                            if (oldIndex < newIndex)
                            {
                                if (relativeMousePos.X > targetItem.ActualWidth / 2)
                                {
                                    shouldMove = true;
                                }
                            }
                            else if (oldIndex > newIndex)
                            {
                                if (relativeMousePos.X < targetItem.ActualWidth / 2)
                                {
                                    shouldMove = true;
                                }
                            }

                            if (shouldMove)
                            {
                                viewModel.MoveAccount(oldIndex, newIndex);
                            }
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
            e.Handled = true; // Vị trí đã được cập nhật trong DragOver
        }

        // Helper tìm phần tử cha trong Visual Tree
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