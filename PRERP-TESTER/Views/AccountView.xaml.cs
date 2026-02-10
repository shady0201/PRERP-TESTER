using PRERP_TESTER.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PRERP_TESTER.Views
{
    public partial class AccountView : UserControl
    {
        public AccountView()
        {
            InitializeComponent();
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DependencyObject originalSource = e.OriginalSource as DependencyObject;

                if (FindAncestor<Button>(originalSource) != null) return;

                var item = FindAncestor<ListBoxItem>(originalSource);

                if (item != null)
                {
                    var data = new DataObject("TabVM", item.DataContext);
                    DragDrop.DoDragDrop(item, data, DragDropEffects.Move);
                }
            }
        }

        private void ListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("TabVM"))
            {
                ListBox? listBox = sender as ListBox;
                var draggedData = e.Data.GetData("TabVM") as TabViewModel;

                // Tìm Tab đang nằm dưới con trỏ chuột
                ListBoxItem targetItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (targetItem != null)
                {
                    var targetData = targetItem.DataContext as TabViewModel;

                    // Giả định DataContext của AccountView là AccountViewModel
                    // Nơi chứa ObservableCollection<TabViewModel> TabViewModels
                    var viewModel = listBox.DataContext as AccountViewModel;

                    if (viewModel != null && draggedData != null)
                    {
                        int oldIndex = viewModel.TabViewModels.IndexOf(draggedData);
                        int newIndex = viewModel.TabViewModels.IndexOf(targetData);

                        if (oldIndex != -1 && newIndex != -1 && oldIndex != newIndex)
                        {
                            // Thực hiện hoán đổi vị trí trong ViewModel
                            viewModel.TabViewModels.Move(oldIndex, newIndex);
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

        // Helper tìm phần tử cha
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