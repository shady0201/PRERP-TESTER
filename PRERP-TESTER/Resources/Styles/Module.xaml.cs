using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PRERP_TESTER.Resources.Styles
{
    public partial class Module : ResourceDictionary
    {
        public Module()
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
