using PRERP_TESTER.Models; // Namespace chứa các entity của bạn

namespace PRERP_TESTER.ViewModels
{
    public class TabViewModel : LazyLoadViewModel
    {

        public TabWeb TabData { get; }

        public string Url => TabData.Url;
        public string Title => TabData.Title;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value) && value)
                {
                    // Tự động set IsLoaded = true ngay khi tab được chọn lần đầu
                    IsLoaded = true;
                }
            }
        }

        public TabViewModel(TabWeb tabWeb)
        {
            TabData = tabWeb;
            IsLoaded = false;
        }


    }
}