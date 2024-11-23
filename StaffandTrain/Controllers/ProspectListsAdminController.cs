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
    [Authorize(Roles = "Admin,Recruiter")]
    public class ProspectListsAdminController : Controller
    {
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();
        // GET: ProspectListsAdmin
        public ActionResult Index(string listid="")
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
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectListsAdmin" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
            }
            if (listid != "")
            {
                ViewBag.listid = listid;
            }
            return View(ProspectList);
        }

        public JsonResult Save_List(string ListName, string restricted)
        {
            string str = "";
            if (Request.IsAuthenticated)
            {
                try
                {
                    byte res = 0;
                    if (restricted == "Yes")
                    {
                        res = 1;
                    }
                    else
                    {
                        res = 0;
                    }
                    var countlst = context.Prospecting_Lists.Where(x => x.listname == ListName).Count();
                    if (countlst == 0)
                    {
                        context.SPInsertProspectlist(ListName, res, null);
                        context.SaveChanges();
                        str = "Success";
                    }
                    else
                    {
                        str = "List already exist";

                    }
                }
                catch (Exception ex)
                {
                    cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectListsAdmin" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Save_List", "NA", "NA", "NA", "WEB");
                    str = "Error Occured";

                }
            }
            else
            {
                str = "Session Expired";

            }
            return Json(str, JsonRequestBehavior.AllowGet);
        }

        public ActionResult updatedlist()
        {
            var ProspectList = new List<SPGetProspectlist_Result>();
            try
            {
                ProspectList = context.SPGetProspectlist().ToList();//.OrderByDescending(x => x.listid).ToList();

            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectListsAdmin" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "updatedlist", "NA", "NA", "NA", "WEB");
            }
            return View(ProspectList);
        }

        public JsonResult Update_List(string ListName, string restricted, int Listid)
        {
            string str = "";
            if (Request.IsAuthenticated)
            {
                try
                {
                    byte res = 0;
                    if (restricted == "Yes")
                    {
                        res = 1;
                    }
                    else
                    {
                        res = 0;
                    }
                    var countlst = context.Prospecting_Lists.Where(x => x.listname == ListName && Listid != Listid).Count();
                    if (countlst == 0)
                    {
                        context.SPUpdateProspectList(ListName, res, Listid);
                        context.SaveChanges();
                        str = "Success";
                    }
                    else
                    {
                        str = "List already exist";
                    }
                }
                catch (Exception ex)
                {
                    cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectListsAdmin" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Save_List", "NA", "NA", "NA", "WEB");
                    str = "Error Occured";

                }
            }
            else
            {
                str = "Session Expired";
            }
            return Json(str, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete_Prospective(string listid)
        {
            string msg = "";
            try
            {
                if (listid != "")
                {

                    var data = context.UserProspectingLists.Where(x => x.listid.Contains(listid)).ToList();
                    foreach (var i in data)
                    {
                        string[] ids = Array.ConvertAll(i.listid.Split(','), element => element.ToString());
                        if (ids.Contains(listid))
                        {
                            ids = ids.Where(val => val != listid).ToArray();
                            if (ids.Count() > 0)
                            {
                                string idString = String.Join(",", ids);
                                context.SpUpdateUserPRos(@i.UserId, idString);
                                context.SaveChanges();
                            }
                            else
                            {
                                context.Spdeletedatauserprospecting(@i.Id);
                                context.SaveChanges();
                            }
                        }
                    }
                    context.SpdeleteProspect(Convert.ToInt32(listid));
                    context.SaveChanges();
                    msg = "Record Deleted";
                    TempData["Message"] = msg;

                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageRoles" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Save_Role", "NA", "NA", "NA", "WEB");
                msg = "Error Occured";
                TempData["Message"] = msg;
            }
            return RedirectToAction("Index");

        }

        public ActionResult TestDemo()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetList(int PageNumber,int PageSize)
        {

            var ProspectList = new List<SPGetProspectlistTest_Result>();
            ProspectList = context.SPGetProspectlistTest(PageNumber, PageSize).ToList();
            return Json(ProspectList,JsonRequestBehavior.AllowGet);
        }

        #region Functionality for Moving one list's companies and their contacts with notes to another list
        [HttpGet]
        public JsonResult GetDataForMoveFunctionality(int ListID)
        {
            var listName = context.Prospecting_Lists.Where(x => x.listid != ListID).Select(x => new { x.listid, x.listname }).OrderBy(x => x.listname).ToList();
            var ListCompany = context.Companies.Where(x => x.listid == ListID).Select(x => new { x.companyid, x.name }).OrderBy(x => x.name).ToList();

            return Json(new { listName, ListCompany }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult MoveListDataToAnotherList(int ListID_from, string companyIds, int ListID_to)
        {
            bool status = false;

            try
            {
                if (!string.IsNullOrEmpty(companyIds))
                {
                    string[] str = companyIds.Split(',');

                    foreach (var item in str)
                    {
                        var company_id = Convert.ToInt32(item);
                        var record = context.Companies.Where(x => x.companyid == company_id).FirstOrDefault();

                        if (record != null)
                        {
                            record.listid = ListID_to;
                            context.SaveChanges();

                            status = true;
                        }
                        else
                        {
                            status = false;
                        }
                    }
                }
                else
                {
                    status = false;
                }                
            }
            catch (Exception ex)
            {
                status = false;
                cm.ErrorExceptionLogingByService(ex.ToString(), "MoveListDataToAnotherList" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "MoveListDataToAnotherList", "NA", "NA", "NA", "WEB");
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}