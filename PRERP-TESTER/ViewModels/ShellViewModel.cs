using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PRERP_TESTER.Models;

namespace PRERP_TESTER.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        public ObservableCollection<Models.Module> Modules { get; } = new();
        [ObservableProperty]
        private Models.Module? selectedModule;

        public ShellViewModel()
        {
            Modules.Add(new Models.Module { Id = "1", Name = "Module Thi TA", Description = "Soạn đề, duyệt đề, bộ đề" });
            Modules.Add(new Models.Module { Id = "2", Name = "Bổ nhiệm - Miễn nhiệm", Description = "Quy trình đề xuất/duyệt" });
            Modules.Add(new Models.Module { Id = "3", Name = "Cơ cấu tổ chức", Description = "Đơn vị, phòng ban" });
            Modules.Add(new Models.Module { Id = "4", Name = "LMS", Description = "Học tập trực tuyến" });
            Modules.Add(new Models.Module { Id = "5", Name = "Chấm công", Description = "Chấm công, tổng hợp" });
            Modules.Add(new Models.Module { Id = "6", Name = "Cập nhật / Hỗ trợ", Description = "Cập nhật, hỗ trợ vận hành" });

            selectedModule = Modules[0];
        }

        [RelayCommand]
        public void SelectModule(Models.Module? module)
        {
            if (module is null) return;
            selectedModule = module;
        }
    }
}
