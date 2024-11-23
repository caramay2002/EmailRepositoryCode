using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffandTrain.Models
{
    public class UserData
    {
        public string name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsApproved { get; set; }
     
        public string UserId { get; set; }
        public string[] Roles { get; set; }
        public string RoleSaved { get; set; }

        public bool IsLockedOut { get; set; }

        

    }
}