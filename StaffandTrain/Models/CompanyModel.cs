using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffandTrain.Models
{
    public class CompanyModel
    {
        public int companyid { get; set; }
        public string name { get; set; }

       
        public Nullable<int> listid { get; set; }
        public string citycircle { get; set; }
        public string biztype { get; set; }
         
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string weburl { get; set; }
        public string phone { get; set; }
        public string adminnotes { get; set; }
        public string notes { get; set; }
        public string oldname { get; set; }
        public Nullable<System.Guid> companyguid { get; set; }
        public Nullable<System.Guid> listguid { get; set; }
        public string listname { get; set; }
        public bool priority { get; set; }
        public bool target { get; set; }
        public string combinednotes { get; set; }
        public string Userid { get; set; }
        public string cssclass { get; set; }
    }
}