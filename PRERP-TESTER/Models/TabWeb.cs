using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    public class TabWeb 
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ModuleId { get; set; } = "";
        public string AccountId { get; set; } = "";

        public string Title { get; set; } = "";
        public string? Url { get; set; }

        public string? FaviconUrl { get; set; }

        public DateTime LastActiveUtc { get; set; } = DateTime.UtcNow;
    }
}
