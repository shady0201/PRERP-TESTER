using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    public class WorkspaceState
    {
        public WorkspaceKey Key { get; }
        public ObservableCollection<TabWebItem> Tabs { get; } = new();
        public string? SelectedTabId { get; set; }

        public WorkspaceState(WorkspaceKey key) => Key = key;
    }
}
