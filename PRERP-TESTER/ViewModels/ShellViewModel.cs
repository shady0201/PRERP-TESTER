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
        public ObservableCollection<ModuleItem> Modules { get; } = new();
        [ObservableProperty]
        private ModuleItem? selectedModule;

        public ShellViewModel()
        {
            Modules.Add(new ModuleItem { Id = "1", Name = "Module Thi TA", Description = "Soạn đề, duyệt đề, bộ đề" });
            Modules.Add(new ModuleItem { Id = "2", Name = "Bổ nhiệm - Miễn nhiệm", Description = "Quy trình đề xuất/duyệt" });
            Modules.Add(new ModuleItem { Id = "3", Name = "Cơ cấu tổ chức", Description = "Đơn vị, phòng ban" });
            Modules.Add(new ModuleItem { Id = "4", Name = "LMS", Description = "Học tập trực tuyến" });
            Modules.Add(new ModuleItem { Id = "5", Name = "Chấm công", Description = "Chấm công, tổng hợp" });
            Modules.Add(new ModuleItem { Id = "6", Name = "Cập nhật / Hỗ trợ", Description = "Cập nhật, hỗ trợ vận hành" });

            selectedModule = Modules[0];
        }

        [RelayCommand]
        public void SelectModule(ModuleItem? module)
        {
            if (module is null) return;
            selectedModule = module;
        }
    }
}
