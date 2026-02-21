using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    class ApplicationData
    {
        public List<Account> Accounts { get; set; } = [];
        public List<ModuleEntity> Modules { get; set; } = [];

        public List<HistoryItem> History { get; set; } = new();
        public ServerType ServerType { get; set; }

        public bool IsAccountExpanded { get; set; } = false;
        public bool IsModuleExpanded { get; set; } = false;
        public bool IsDarkMode { get; set; } = false;
        public bool IsAccountListCollapsed { get; set; } = false;
        public bool IsMenuCollapsed { get; set; } = false;
        public string? SelectedModuleId { get; set; } = null;

    }
}
