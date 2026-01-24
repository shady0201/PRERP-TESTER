using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PRERP_TESTER.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Hàm này sẽ được gọi mỗi khi bạn muốn cập nhật dữ liệu lên UI
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Hàm tiện ích để gán giá trị và tự động thông báo thay đổi
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}