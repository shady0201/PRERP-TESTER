using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PRERP_TESTER.Models
{
    public class Account : ObservableObject
    {
        public string Id { get; set; } = "";

        public string Username { get; set; } = "";

        private string? _password;
        public string? Password { get => _password; set => SetProperty(ref _password , value); }

        private string? _displayName;
        public string? DisplayName { get => _displayName; set => SetProperty(ref _displayName, value);}

        private string? _fullName;
        public string? FullName { get => _fullName; set => SetProperty(ref _fullName, value); }

        private string? _sId;
        public string? sId { get => _sId; set => SetProperty(ref _sId, value); }

        private string? _email;
        public string? Email { get => _email; set => SetProperty(ref _email, value); }

        private string? _staffType;
        public string? StaffType { get => _staffType; set => SetProperty(ref _staffType, value); }

        private string? _education;
        public string? Education { get => _education; set => SetProperty(ref _education, value); }

        private string? _academicRank;
        public string? AcademicRank { get => _academicRank; set => SetProperty(ref _academicRank, value); }

        private string? _status;
        public string? Status { get => _status; set => SetProperty(ref _status, value); }

        private string? _mobile;
        public string? Mobile { get => _mobile; set => SetProperty(ref _mobile, value); }


        private bool _isLoggedIn = false;
        [JsonIgnore]
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }


        private string? _sessionExpiry;
        public string? SessionExpiry
        {
            get => _sessionExpiry;
            set => SetProperty(ref _sessionExpiry, value);
        }

        private string? _avatarUrl = null;
        public string? AvatarUrl
        {
            get => _avatarUrl;
            set => SetProperty(ref _avatarUrl, value);
        }

        [JsonIgnore]
        public string LastSessionValue { get; set; } = "";
        [JsonIgnore]
        public string UserInfoJsonString { get; set; } = "";


        private AccountRole _role;
        public AccountRole Role { get => _role; set => SetProperty(ref _role , value); }

        private ServerType _serverType;
        public ServerType ServerType { get => _serverType; set => SetProperty(ref _serverType, value); }

        
        public string SessionFolder => Id + "_" + Username;

        [JsonIgnore]
        public Permission[] PermissionsSFT { get; set; } = Array.Empty<Permission>();

        [JsonIgnore]
        public Permission[] PermissionsMASSET { get; set; } = Array.Empty<Permission>();

        [JsonIgnore]
        public Permission[] PermissionsHASSET { get; set; } = Array.Empty<Permission>();

        [JsonIgnore]
        public Department[] _departments = Array.Empty<Department>();

        public Department[] Departments
        {
            get => _departments;
            set => SetProperty(ref _departments, value);
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Initials => !string.IsNullOrWhiteSpace(DisplayName)
                        ? DisplayName.Trim().Substring(0, 1).ToUpper()
                        : "?";

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string AvatarColor => GetColorFromName(DisplayName);

        // details view
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        private bool _isSftExpanded = false;
        [Newtonsoft.Json.JsonIgnore]
        public bool IsSftExpanded { get => _isSftExpanded; set => SetProperty(ref _isSftExpanded, value); }

        [Newtonsoft.Json.JsonIgnore]
        private bool _isMassetExpanded = false;
        [Newtonsoft.Json.JsonIgnore]
        public bool IsMassetExpanded { get => _isMassetExpanded; set => SetProperty(ref _isMassetExpanded, value); }

        [Newtonsoft.Json.JsonIgnore]
        private bool _isHassetExpanded = false;
        [Newtonsoft.Json.JsonIgnore]
        public bool IsHassetExpanded { get => _isHassetExpanded; set => SetProperty(ref _isHassetExpanded, value); }

        public void GetData(string JsonStringData)
        {
            UserInfoJsonString = JsonStringData;

            var data = JObject.Parse(JsonStringData);
            var query = data["query"];

            // info
            FullName = query["user_fullname"]?.ToString() ?? "No Name";

            var use_info = query["user_info"];
            if (use_info != null)
            {
                sId = use_info["sid"]?.ToString() ?? "";
                Email = use_info["email"]?.ToString() ?? "";
                StaffType = use_info["staff_type"]?.ToString() ?? "";
                Education = use_info["education"]?.ToString() ?? "";
                AcademicRank = use_info["academic_rank"]?.ToString() ?? "";
                Status = use_info["status"]?.ToString() ?? "";
                Mobile = use_info["mobile"]?.ToString() ?? "";
            }

            AvatarUrl = $"{GobalSetting.CurrentBaseUrl.Replace("/mywork","")}/mdata/hrm/" + (query["user_avatar"]?.ToString());

            // permissions
            PermissionsSFT = ParsePermissions(query["permissions_sft"]);
            PermissionsMASSET = ParsePermissions(query["permissions_masset"]);
            PermissionsHASSET = ParsePermissions(query["permissions_hasset"]);

            // departments
            //Departments = ParseDepartments(query["departments"]);
            // mock data
            Departments =
            [
                    new() { Id = 2, Name = "Phòng Đào Tạo" , PositionJobs = [
                                                                                new PositionJob {Id=3,Name="Phó phòng" },
                                                                                new PositionJob {Id=3,Name="Nhân viên" },
                                                                             ]
                    },
                    new() { Id = 2, Name = "Bộ môn nội" , PositionJobs = [
                                                                            new PositionJob {Id=3,Name="Giảng viên" },
                                                                            new PositionJob {Id=4,Name="Phó bộ môn" },
                                                                        ]
                    }
            ];

            IsLoggedIn = true;
            SessionExpiry = query["expired"]?.ToString();
        }

        public bool IsSessionExpired()
        {
            if (string.IsNullOrEmpty(SessionExpiry)) return true;
            if (DateTime.TryParse(SessionExpiry, out DateTime expiredDateTime))
            {
                return DateTime.Now >= expiredDateTime;
            }
            return true;
        }

        public void CleanData()
        {
            IsLoggedIn = false;
            LastSessionValue = "";
            UserInfoJsonString = "";
            AvatarUrl = null;
            PermissionsSFT = Array.Empty<Permission>();
            PermissionsMASSET = Array.Empty<Permission>();
            PermissionsHASSET = Array.Empty<Permission>();
            Departments = Array.Empty<Department>();
        }

        // PRIVATE



        private Department[] ParseDepartments(JToken? node)
        {
            if (node == null || node.Type != JTokenType.Array)
                return Array.Empty<Department>();

            var resultList = new List<Department>();
            var jsonArray = (JArray)node;
            foreach (var item in jsonArray)
            {
                var deptObj = item as JObject;
                if (deptObj == null) continue;
                var department = new Department
                {
                    Id = deptObj["id"]?.ToObject<int>() ?? 0,
                    Name = deptObj["name"]?.ToString() ?? ""
                };
                resultList.Add(department);
            }
            return resultList.ToArray();
        }

        private Permission[] ParsePermissions(JToken? node)
        {
            if (node == null || node.Type != JTokenType.Array)
                return Array.Empty<Permission>();

            var resultList = new List<Permission>();

            var jsonArray = (JArray)node;

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

        private string GetColorFromName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return "#FF757575";
            string[] colors = { "#FF2196F3", "#FF4CAF50", "#FFF44336", "#FFFF9800", "#FF9C27B0", "#FF00BCD4" };
            int hash = Math.Abs(name.GetHashCode());
            return colors[hash % colors.Length];
        }

    }
}
