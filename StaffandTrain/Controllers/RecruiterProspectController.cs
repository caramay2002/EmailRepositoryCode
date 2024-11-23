using StaffandTrain.Common;
using StaffandTrain.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StaffandTrain.Controllers
{
    [NoCache]
    public class RecruiterProspectController : Controller
    {
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();
        // GET: RecruiterProspect Not in use
        public ActionResult Index()
        {
            var ProspectList = new List<SPGetProspectlist_Result>();
            try
            {
                if (TempData["Message"] != null)
                {
                    ViewBag.message = TempData["Message"];
                }
                ProspectList = context.SPGetProspectlist().ToList();

            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "RecruiterProspect" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
            }
            return View(ProspectList);
        }
    }
}