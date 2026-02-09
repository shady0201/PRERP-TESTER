using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PRERP_TESTER.Models
{
    public enum ToastType { Success, Warning, Information, Error }
    public class ToastModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public ToastType Type { get; set; }
        public ICommand DismissCommand { get; set; }

    }
}
