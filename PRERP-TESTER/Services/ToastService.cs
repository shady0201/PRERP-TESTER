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
            var delayTime = 3000;
            switch(type)
            {
                case ToastType.Success:
                    delayTime = 2000;
                    break;
                case ToastType.Warning:
                    delayTime = 4000;
                    break;
                case ToastType.Information:
                    delayTime = 3000;
                    break;
                case ToastType.Error:
                    delayTime = 5000;
                    break;
            }
            var toast = new ToastModel { Title = title, Message = message, Type = type };

            toast.DismissCommand = new RelayCommand(() => {
                if (Toasts.Contains(toast)) Toasts.Remove(toast);
            });

            Toasts.Add(toast);

            await Task.Delay(delayTime);
            if (Toasts.Contains(toast))
            {
                Toasts.Remove(toast);
            }
        }
    }
}
