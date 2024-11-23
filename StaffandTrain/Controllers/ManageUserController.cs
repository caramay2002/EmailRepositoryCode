
using StaffandTrain.Common;
using StaffandTrain.DataModel;
using StaffandTrain.Models;
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
    public class ManageUserController : Controller
    {
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();
        // GET: ManageUser
        public ActionResult Index()
        {
            var userlist = new List<Spgetusers_Result>();
            try
            {
                userlist = context.Spgetusers().ToList();
                ViewData["City"] = context.SPgetProspectinglists().Select(xx => new SelectListItem { Value = xx.listid.ToString(), Text = xx.listname + " /User Count" + xx.Cnt }).ToList();
                if (TempData["Message"] != null)
                {
                    ViewBag.message = TempData["Message"];
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageUser" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");

            }
            return View(userlist);
        }
        public ActionResult SaveUser()
        {
            UserData objuser = new UserData();
            var Roleslst = context.SpgetRolelist().ToList().Select(xx => new SelectListItem { Value = xx.RoleName.ToString(), Text = xx.RoleName }).ToList();
            ViewData["Roles"] = Roleslst;
            if (Request.QueryString["Userid"] != null)
            {
                Guid newid = new Guid(Request.QueryString["Userid"]);
                MembershipUser u = Membership.GetUser(newid);
                if (u != null)
                {
                    objuser.UserId = u.ProviderUserKey.ToString();
                    objuser.name = u.UserName;
                    objuser.Email = u.Email;
                    objuser.IsApproved = u.IsApproved;
                    objuser.IsLockedOut = u.IsLockedOut;
                    objuser.Roles = Roles.GetRolesForUser(u.UserName);
                    objuser.RoleSaved = string.Join(",", objuser.Roles);
                }

            }
            if (TempData["Message"] != null)
            {
                ViewBag.message = TempData["Message"];
            }
            return View(objuser);
        }
        [HttpPost]
        public ActionResult CreateUser(UserData objuser)
        {
            try
            {
                if (objuser.UserId == null)
                {
                    MembershipUser newUser = Membership.CreateUser(objuser.name, objuser.Password);
                    if (newUser != null)
                    {
                        newUser.IsApproved = objuser.IsApproved;
                        newUser.Email = objuser.Email;
                        Membership.UpdateUser(newUser);
                        Roles.AddUserToRoles(newUser.UserName, objuser.Roles);
                        TempData["Message"] = "User Created";
                        return RedirectToAction("Index", "ManageUser");
                    }
                }
                else
                {
                    Guid newid = new Guid(objuser.UserId);
                    MembershipUser u = Membership.GetUser(newid);
                    if (u != null)
                    {
                        //u.Email=objuser.Email ;
                        //Membership.UpdateUser(u);
                        //u.IsApproved= objuser.IsApproved;
                        //Membership.UpdateUser(u);
                        var rolessaved = Roles.GetRolesForUser(u.UserName);
                        if (!string.IsNullOrEmpty(objuser.Password))
                        {
                            u.ChangePassword(u.ResetPassword(), objuser.Password);
                        }

                        if (rolessaved.Count() > 0)
                        {
                            Roles.RemoveUserFromRoles(u.UserName, rolessaved);
                        }
                        Roles.AddUserToRoles(u.UserName, objuser.Roles);
                        var checkusername = context.aspnet_Users.Where(x => x.UserName == objuser.name && x.UserId != newid).Count();
                        var userdetails = context.aspnet_Membership.Where(x => x.UserId == newid).FirstOrDefault();
                        if (userdetails != null)
                        {
                            userdetails.Email = objuser.Email;
                            userdetails.IsApproved = objuser.IsApproved;
                            userdetails.IsLockedOut = objuser.IsLockedOut;
                            userdetails.FailedPasswordAttemptCount = 0;
                            context.SaveChanges();
                        }
                        if (checkusername == 0)
                        {
                            var usernamedata = context.aspnet_Users.Where(x => x.UserId == newid).FirstOrDefault();
                            usernamedata.UserName = objuser.name;
                            context.SaveChanges();
                            TempData["Message"] = "User Updated";
                            return RedirectToAction("Index", "ManageUser");
                        }
                        else
                        {
                            TempData["Message"] = "Username already exist";
                        }


                    }
                    else
                    {
                        TempData["Message"] = "Username not exist";
                    }
                }
            }
            catch (MembershipCreateUserException e)
            {
                cm.ErrorExceptionLogingByService(e.ToString(), "ManageUser" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "CreateUser", "NA", "NA", "NA", "WEB");
                var msg = GetErrorMessage(e.StatusCode);
                TempData["Message"] = msg;
            }
            return RedirectToAction("SaveUser", new { UserId = objuser.UserId });
        }


        public string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }


        public JsonResult checkuser(string Username, string action, string userid)
        {
            int usercount = 0;

            if (action == "Insert")
            {
                usercount = context.aspnet_Users.Where(x => x.UserName == Username).Count();
            }
            if (action == "Update")
            {
                Guid useridguid = new Guid(userid);
                usercount = context.aspnet_Users.Where(x => x.UserName == Username && x.UserId != useridguid).Count();
            }
            return Json(usercount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateProspective(string Username, string[] listids)
        {
            string str = "";
            try
            {
                var result = string.Join(",", listids);
                var UserPros = context.UserProspectingLists.Where(x => x.UserId == Username);

                if (UserPros.Count() == 0)
                {
                    context.SpInsertUserPRos(Username, result);
                    context.SaveChanges();
                    str = "Record Saved";
                }
                else
                {
                    context.SpUpdateUserPRos(Username, result);
                    context.SaveChanges();
                    str = "Record Inserted";
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageUser" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "UpdateProspective", "NA", "NA", "NA", "WEB");
                str = "Error";
            }

            return Json(str, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Deleteuser(string userid)
        {
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    Guid userguid = new Guid(userid);
                    var userdata = context.aspnet_Users.Where(x => x.UserId == userguid).ToList();
                    if (userdata != null && userdata.Count() > 0)
                    {
                        var username = userdata.FirstOrDefault().UserName;
                        context.SPDeleteUser(userguid, username);
                        context.SaveChanges();
                        TempData["Message"] = "User Deleted";

                    }
                    else
                    {
                        TempData["Message"] = "User Not Exist";
                    }
                }
                else
                {
                    TempData["Message"] = "User Not Found";
                }



            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageUser" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Deleteuser", "NA", "NA", "NA", "WEB");

            }

            return RedirectToAction("Index");

        }

        public ActionResult Assigncity(string UserName)
        {
            listdetails objlst = new listdetails();
            try
            {
                objlst.objlist = context.SPgetProspectinglists().ToList();
                var selectedlist = context.SPgetprospectinglistbyuserid(UserName).FirstOrDefault();
                ViewBag.UserName = UserName;
                List<int> selectedlistIds = new List<int>();
                if (selectedlist != null && selectedlist.Count() > 0)
                {
                    selectedlistIds = selectedlist.Split(',').Select(int.Parse).ToList();
                }
                objlst.selectedlist = selectedlistIds;
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ManageUser" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Assigncity", "NA", "NA", "NA", "WEB");
            }
            return View(objlst);
        }




    }
}
