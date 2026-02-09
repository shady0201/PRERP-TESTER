using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Services
{
    public class ToastService
    {
        public static ObservableCollection<ToastModel> Toasts { get; } = new();

        public static async void Show(string title, string message, ToastType type)
        {
            var toast = new ToastModel { Title = title, Message = message, Type = type };

            toast.DismissCommand = new RelayCommand(() => {
                if (Toasts.Contains(toast)) Toasts.Remove(toast);
            });

            Toasts.Insert(0, toast);
            await Task.Delay(3000);
            if (Toasts.Contains(toast))
            {
                Toasts.Remove(toast);
            }
        }
    }
}
