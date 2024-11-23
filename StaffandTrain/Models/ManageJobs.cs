using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffandTrain.Models
{
    public class ManageJobs
    {
        public int jobid { get; set; }
        public int jobiddecypt { get; set; }
        public string jobtitle { get; set; }
        public string jobdescr { get; set; }
        public string submittals { get; set; }
        public int? WhiteboardID { get; set; }
        public int? RowNumber { get; set; }
    }
}