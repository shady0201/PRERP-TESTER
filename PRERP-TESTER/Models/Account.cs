using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Models
{
    public class Account : ObservableObject
    {
        public string Id { get; set; } = "";

        public string Username { get; set; } = "";

        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password , value); }

        private string _displayName;
        public string DisplayName { get => _displayName; set => SetProperty(ref _displayName, value);}

        private string _stype;  // STUDENT || STAFF
        public string Stype { get => _stype; set => SetProperty(ref _stype , value); }

        private string _serverType;  // CAPP || PRERP
        public string ServerType { get => _serverType; set => SetProperty(ref _serverType, value); }

        public string session { get; set; } = "";

        public string SessionFolder => Id + "_" + Username;

        public Permission[] Permissions { get; set; } = Array.Empty<Permission>();

        public Department[] Departments { get; set; } = Array.Empty<Department>();

    }
}
