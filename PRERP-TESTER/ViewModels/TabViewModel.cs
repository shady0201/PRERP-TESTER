using PRERP_TESTER.Models;
using System.Web; // Namespace chứa các entity của bạn

namespace PRERP_TESTER.ViewModels
{
    public class TabViewModel : LazyLoadViewModel
    {
        public string AccountID { get; set; }
        public string ModuleID { get; set; }
        public TabWeb TabData { get; }

        public string Url => TabData.Url;
        public string Title => TabData.Title;

        public TabViewModel(TabWeb tabWeb, string username, string moduleID)
        {
            TabData = tabWeb;
            IsLoaded = false;
            AccountID = username;
            ModuleID = moduleID;
        }

    }
}