using StaffandTrain.Common;
using StaffandTrain.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace StaffandTrain.Controllerss
{
    [NoCache]
    [Authorize(Roles ="Admin")]
    public class AdminHomeController : Controller
    {
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();
        // GET: AdminHome
        public ActionResult Index()
        {
            var userlist = new List<GetNewUsers_Result>();
            try
            {
                userlist= context.GetNewUsers(Roles.ApplicationName, 20, 30).ToList();
                var rolecount = context.SpgetRoles(Roles.ApplicationName).Where(x=>x.UserCount !=0).ToList();
                ViewBag.rolecountdata = rolecount;
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "AdminHome" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");

            }
            return View(userlist);
        }
    }
}