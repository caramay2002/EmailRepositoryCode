using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffandTrain.Models
{
    public class WorkerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public System.TimeSpan CheckIn { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime ModifiedDate { get; set; }
    }
}