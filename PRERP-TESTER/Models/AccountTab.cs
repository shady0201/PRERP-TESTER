using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Chứa danh sách tab account có trong module
// Dùng trong ModuleItem.cs
namespace PRERP_TESTER.Models
{
    public class AccountTab
    {
        public string AccountId { get; set; } = "";
        public TabWebItem[] TabWebItems { get; set; } = [];
    }
}
