using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Nodes;
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

        private string _fullName;
        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }

        private bool _isLoggedIn = false;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        private string _avatarUrl = "";
        public string AvatarUrl
        {
            get => _avatarUrl;
            set => SetProperty(ref _avatarUrl, value);
        }

        public string LastSessionValue { get; set; } = "";
        public string UserInfoJsonString { get; set; } = "";


        private AccountRole _role;
        public AccountRole Role { get => _role; set => SetProperty(ref _role , value); }

        private ServerType _serverType;
        public ServerType ServerType { get => _serverType; set => SetProperty(ref _serverType, value); }

      
        public string SessionFolder => Id + "_" + Username; // folder lưu session của webview, đã xử lý xong

        public Permission[] PermissionsSFT { get; set; } = Array.Empty<Permission>();
        public Permission[] PermissionsMASSET { get; set; } = Array.Empty<Permission>();
        public Permission[] PermissionsHASSET { get; set; } = Array.Empty<Permission>();

        public Department[] Departments { get; set; } = Array.Empty<Department>();


        public void GetData(string JsonStringData)
        {
            IsLoggedIn = true;
            UserInfoJsonString = JsonStringData;

            var data = JsonNode.Parse(JsonStringData);
            var query = data["query"];

            // info
            FullName = query["user_fullname"]?.ToString() ?? Username;

            // permissions
            PermissionsSFT = ParsePermissions(query["permissions_sft"]);
            PermissionsMASSET = ParsePermissions(query["permissions_masset"]);
            PermissionsHASSET = ParsePermissions(query["permissions_hasset"]);

            // departments
        }

        public void CleanData()
        {
            IsLoggedIn = false;
            LastSessionValue = "";
            UserInfoJsonString = "";
            PermissionsSFT = Array.Empty<Permission>();
            PermissionsMASSET = Array.Empty<Permission>();
            PermissionsHASSET = Array.Empty<Permission>();
            Departments = Array.Empty<Department>();
        }

        // PRIVATE
        private Permission[] ParsePermissions(JsonNode? node)
        {
            if (node == null || node is not JsonArray jsonArray)
                return Array.Empty<Permission>();

            var resultList = new List<Permission>();

            foreach (var item in jsonArray)
            {
                string? sPermission = item?.ToString();
                if (string.IsNullOrEmpty(sPermission)) continue;

                var perArr = sPermission.Split(["$$$"], StringSplitOptions.None);

                if (perArr.Length >= 2)
                {
                    resultList.Add(new Permission
                    {
                        Name = perArr[0],
                        Role = perArr[1].Split("$")
                    });
                }
            }

            return resultList.ToArray();
        }

    }
}
