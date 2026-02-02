using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    public class ModuleEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public ServerType ServerType { get; set; }
        public AccountModule[] AccountModules { get; set; } = [];
        public TestCase[] TestCases { get; set; } = [];

    }
}
