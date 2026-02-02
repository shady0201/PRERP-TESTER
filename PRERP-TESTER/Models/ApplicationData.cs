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

    }
}
