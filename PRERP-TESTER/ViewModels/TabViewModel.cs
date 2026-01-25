using PRERP_TESTER.Models; // Namespace chứa các entity của bạn

namespace PRERP_TESTER.ViewModels
{
    public class TabViewModel
    {
        public TabWeb TabData { get; }

        public TabViewModel(TabWeb tabWeb)
        {
            TabData = tabWeb;
        }

        public string Url => TabData.Url;
    }
}