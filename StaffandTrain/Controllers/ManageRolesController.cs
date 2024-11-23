using StaffandTrain.Common;
using StaffandTrain.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace StaffandTrain.Controllers
{
    [NoCache]
    [Authorize(Roles = "Admin")]
    public class ManageRolesController : Controller
    {

        // GET: ManageRoles
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();
        public ActionResult Index()
        {
            try
            {
                if (TempData["msg"] != null)
                {
                    ViewBag.Message = TempData["msg"];
                }
                var rolecount = context.SpgetRoles(Roles.ApplicationName).ToList();
                ViewBag.rolecountdata = rolecount;
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageRoles" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");

            }
            return View();
        }
        [HttpPost]

        public JsonResult Save_Role(string RoleName)
        {
            string msg = "";
            try
            {
                
                if (!Roles.RoleExists(RoleName))
                {
                    // Create the role    
                    Roles.CreateRole(RoleName);
                    // Refresh the RoleList Grid    
                    msg = "Success";
                }
                else
                {
                    msg = "Role already exist";

                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageRoles" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Save_Role", "NA", "NA", "NA", "WEB");
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]

        public JsonResult Update_Role(string RoleName,string OldRoleName)
        {
            string msg = "";
            try
            { 
                if (Roles.RoleExists(OldRoleName))
                {
                    if (!Roles.RoleExists(RoleName))
                    {
                        string[] users = Roles.GetUsersInRole(OldRoleName);
                        Roles.CreateRole(RoleName);
                        if (users.Count() != 0)
                        {
                            Roles.AddUsersToRole(users, RoleName);
                            Roles.RemoveUsersFromRole(users, OldRoleName);
                        }
                        Roles.DeleteRole(OldRoleName);
                        msg = "Success";
                    }
                    else
                    {
                        msg = "Role already exist";

                    } 
                }
                else
                {
                    msg = "Role not exist";

                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageRoles" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Update_Role", "NA", "NA", "NA", "WEB");
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Delete_Role(string RoleName)
        {
            string msg = "";
            try
            {
                if (Roles.RoleExists(RoleName))
                {
                        string[] users = Roles.GetUsersInRole(RoleName);
                        if (users.Count() != 0)
                        {
                             
                            Roles.RemoveUsersFromRole(users, RoleName);
                        }
                        Roles.DeleteRole(RoleName);
                    TempData["msg"] = "Role deleted.";

                }
                else
                {
                    TempData["msg"] = "Role not exist";

                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageRoles" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Save_Role", "NA", "NA", "NA", "WEB");
            }
            return RedirectToAction("Index");

        }

        public ActionResult updatedroles()
        {

            try
            {
                var rolecount = context.SpgetRoles(Roles.ApplicationName).ToList();
                ViewBag.rolecountdata = rolecount;
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageRoles" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");

            }
            return View();
        }





    }
}