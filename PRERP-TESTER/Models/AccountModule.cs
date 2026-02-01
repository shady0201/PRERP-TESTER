using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    public class AccountModule
    {
        public string AccountID { get; set; }
        public TabWeb[] TabWebItems { get; set; } = [];
    }
}
