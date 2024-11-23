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
    [Authorize(Roles ="Client")]
    public class ProspectListsClientController : Controller
    {
        // GET: ProspectListsClient
        SATConn context = new SATConn();
        Common.Common cm = new Common.Common();
        public ActionResult Index()
        {
            var listdata = new List<SpGetprospectinglistforclient_Result>();
            string username = User.Identity.Name;
            if (username != "")
            {
                var data = context.UserProspectingLists.Where(x => x.UserId == username).FirstOrDefault();
                listdata = context.SpGetprospectinglistforclient(username).ToList();
                if (data != null)
                {
                    List<int> listIds = data.listid.Split(',').Select(int.Parse).ToList();
                    listdata = listdata.Where(x => x.Userid.ToUpper() == username.ToUpper() || listIds.Contains(x.listid)).ToList();// (from d in listdata where (d.Userid.ToUpper() == username.ToUpper()) ||  (listIds.Contains(d.listid)) select d).ToList();
                }
                else
                {
                    listdata = listdata.Where(x => x.Userid.ToUpper() == username.ToUpper()).ToList();
                }
            }
            return View(listdata);
        }

        public JsonResult Save_List(string ListName, string restricted)
        {
            string str = "";
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
                var username = User.Identity.Name;
                if (countlst == 0)
                {
                    context.SPInsertProspectlist(ListName, res, username);
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
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectListsClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Save_List", "NA", "NA", "NA", "WEB");
                str = "Error Occured";

            }
            return Json(str, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Update_List(string ListName, string restricted, int Listid)
        {
            string str = "";
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
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectListsClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Save_List", "NA", "NA", "NA", "WEB");
                str = "Error Occured";

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
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectListsClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Save_Role", "NA", "NA", "NA", "WEB");
                msg = "Error Occured";
                TempData["Message"] = msg;
            }
            return RedirectToAction("Index");

        }

        public ActionResult updatedlist()
        {
            var listdata = new List<SpGetprospectinglistforclient_Result>();
            try
            {
                string username = User.Identity.Name;
                if (username != "")
                {
                    var data = context.UserProspectingLists.Where(x => x.UserId == username).FirstOrDefault();
                    listdata = context.SpGetprospectinglistforclient(username).ToList();
                    if (data != null)
                    {
                        List<int> listIds = data.listid.Split(',').Select(int.Parse).ToList();
                        listdata = listdata.Where(x => x.Userid.ToUpper() == username.ToUpper() || listIds.Contains(x.listid)).ToList();// (from d in listdata where (d.Userid.ToUpper() == username.ToUpper()) ||  (listIds.Contains(d.listid)) select d).ToList();
                    }
                    else
                    {
                        listdata = listdata.Where(x => x.Userid.ToUpper() == username.ToUpper()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectListsClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "updatedlist", "NA", "NA", "NA", "WEB");
            }
            return View(listdata);
        }
    }
}