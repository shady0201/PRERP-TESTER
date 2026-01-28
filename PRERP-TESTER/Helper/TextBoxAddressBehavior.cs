using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PRERP_TESTER.Helpers
{
    public static class TextBoxAddressBehavior
    {
        public static readonly DependencyProperty SelectAllOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllOnFocus",
                typeof(bool),
                typeof(TextBoxAddressBehavior),
                new PropertyMetadata(false, OnSelectAllOnFocusChanged));

        public static bool GetSelectAllOnFocus(DependencyObject obj) => (bool)obj.GetValue(SelectAllOnFocusProperty);
        public static void SetSelectAllOnFocus(DependencyObject obj, bool value) => obj.SetValue(SelectAllOnFocusProperty, value);

        private static void OnSelectAllOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.GotFocus += TextBox_GotFocus;
                    textBox.PreviewMouseLeftButtonDown += TextBox_PreviewMouseLeftButtonDown;
                }
                else
                {
                    textBox.GotFocus -= TextBox_GotFocus;
                    textBox.PreviewMouseLeftButtonDown -= TextBox_PreviewMouseLeftButtonDown;
                }
            }
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        private static void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && !textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }
    }
}
