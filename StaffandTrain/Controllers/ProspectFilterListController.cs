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
    [Authorize(Roles = "Admin")]
    public class ProspectFilterListController : Controller
    {
        // GET: ProspectFilterList
        SATConn context = new SATConn();
        Common.Common cm = new Common.Common();
        public ActionResult Index()
        {
            try
            {
                var citycirclelist = context.Spgetcitycircleemail().Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
                var Biztypelist = context.Spgetbiztypeemail().Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
                var Titlelist = context.SPgettitle().Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
                ViewData["citycirclelist"] = citycirclelist;
                ViewData["Biztypelist"] = Biztypelist;
                ViewData["Titlelist"] = Titlelist;
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectFilterList" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");

            }
            return View();
        }


        public JsonResult Prospectlistsearch(string CityCircle ,string Biztype,string TitleStandard,string search)
        {
            var data = new List<string>();
            int count = 0;
            try
            {
                if (!string.IsNullOrEmpty(Biztype))
                {
                    Biztype = Biztype.Replace("&amp;", "&");
                }
                if (TitleStandard == "null")
                {
                    TitleStandard = null;
                }
                 data = (from com in context.Companies
                            join con in context.Contacts on com.companyid equals con.companyid
                            where ((CityCircle == null || CityCircle == "") || com.citycircle == CityCircle) &&
                            ((Biztype == null || Biztype == "") || com.biztype == Biztype) &&
                            ((TitleStandard == null || TitleStandard == "") || con.titlestandard.Contains(TitleStandard)) &&
                           ((search == null || search == "") || con.contactfullname.Contains(search) || com.name.Contains(search))
                           && con.contactemail != ""   && con.contactemail !=null
                         select con.contactemail).ToList();
                
                if (data.Count() > 0)
                {
                    data = data.OrderBy(x => x).ToList();
                    count = data.Count();
                } 
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectFilterList" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Prospectlistsearch", "NA", "NA", "NA", "WEB");

            }


            var result = new { data = data, str = count };
            return Json(result, JsonRequestBehavior.AllowGet);

        }


    }
}