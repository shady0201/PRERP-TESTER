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
    public partial class MenuLeft : ResourceDictionary
    {
        public MenuLeft()
        {
            InitializeComponent();
        }

        private void OnMouseRightButtonDownItemLeft(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (sender is ListBoxItem item && item.ContextMenu != null)
            {
                item.ContextMenu.PlacementTarget = item;
                item.ContextMenu.IsOpen = true;
            }
        }
    }
}
