using StaffandTrain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StaffandTrain.Controllers
{
    [NoCache]
    public class ClientHomeController : Controller
    {
        // GET: ClientHome
        public ActionResult Index()
        {
            return View();
        }
    }
}