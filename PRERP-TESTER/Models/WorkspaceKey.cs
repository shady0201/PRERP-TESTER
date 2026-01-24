using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    public readonly record struct WorkspaceKey(string ModuleId, string AccountId)
    {
        public override string ToString() => $"{ModuleId}__{AccountId}";
    }
}
