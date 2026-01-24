using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    public class Module
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public AccountTab[] accountTabs { get; set; } = [];

    }
}
