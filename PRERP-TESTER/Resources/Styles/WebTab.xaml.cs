using MahApps.Metro.Controls;
using PRERP_TESTER.Models;
using PRERP_TESTER.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PRERP_TESTER.Resources.Templates
{
    public partial class WebTab : ResourceDictionary
    {
        public WebTab()
        {
            InitializeComponent();
        }

        private void OnTabMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (sender is ListBoxItem item && item.ContextMenu != null)
            {
                item.ContextMenu.PlacementTarget = item;
                item.ContextMenu.IsOpen = true;
            }
        }

        private void OnSuggestionItemClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item && item.DataContext is HistoryItem history &&
                item.TryFindParent<ListBox>()?.DataContext is TabViewModel vm)
            {
                vm.ConfirmSuggestion(history);
            }
        }

        private void UrlTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null || textBox.DataContext is not TabViewModel vm || !vm.IsSuggestionOpen)
                return;

            var parentGrid = VisualTreeHelper.GetParent(textBox) as Grid;
            if (parentGrid == null) return;

            Popup popup = null;
            foreach (var child in LogicalTreeHelper.GetChildren(parentGrid))
            {
                if (child is Popup p && p.Name == "AddressPopup")
                {
                    popup = p;
                    break;
                }
            }

            if (popup == null || !popup.IsOpen) return;

            var border = popup.Child as Border;
            var listBox = border?.Child as ListBox;

            if (listBox == null || !listBox.HasItems) return;

            int index = listBox.SelectedIndex;

            switch (e.Key)
            {
                case Key.Down:
                    e.Handled = true;
                    if (listBox.SelectedIndex < listBox.Items.Count - 1)
                        listBox.SelectedIndex++;
                    else
                        listBox.SelectedIndex = 0;
                    listBox.ScrollIntoView(listBox.SelectedItem);
                    break;

                case Key.Up:
                    e.Handled = true;
                    if (listBox.SelectedIndex > 0)
                        listBox.SelectedIndex--;
                    else
                        listBox.SelectedIndex = listBox.Items.Count - 1;
                    listBox.ScrollIntoView(listBox.SelectedItem);
                    break;

                case Key.Enter:
                    if (listBox.SelectedItem is HistoryItem selected)
                    {
                        e.Handled = true;
                        vm.ConfirmSuggestion(selected);
                    }
                    break;

                case Key.Escape:
                    vm.IsSuggestionOpen = false;
                    e.Handled = true;
                    break;
            }
        }
    }
}