using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRERP_TESTER.Services
{
    public class ThemeService
    {
        public void SetTheme(bool isDark)
        {
            var themeName = isDark ? "DarkMode.xaml" : "LightMode.xaml";
            var dict = new ResourceDictionary
            {
                Source = new Uri($"/Themes/{themeName}", UriKind.Relative)
            };

            // Tìm và thay thế ResourceDictionary cũ trong App
            var appResources = System.Windows.Application.Current.Resources.MergedDictionaries;
            appResources.Clear();
            appResources.Add(dict);
        }
    }
}
