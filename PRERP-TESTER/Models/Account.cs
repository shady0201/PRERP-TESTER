using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    public class Account
    {
        public string Id { get; set; } = "";

        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string DisplayName { get; set; } = "";

        public string Stype { get; set; } // STUDENT || STAFF

        public string session { get; set; } = "";

        public Permission[] Permissions { get; set; } = Array.Empty<Permission>();

        public Department[] Departments { get; set; } = Array.Empty<Department>();

    }
}
