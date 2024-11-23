using StaffandTrain.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StaffandTrain.Models;
using System.Web.Security;
using StaffandTrain.Common;

namespace StaffandTrain.Controllers
{
    [NoCache]
    public class SignUpController : Controller
    {
        // GET: SignUp
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();
        Common.SendEmail sm = new Common.SendEmail();
        public ActionResult Index()
        {

            MemberSignUp objsignup = new MemberSignUp();
            if (TempData["Message"] != null)
            {
                ViewBag.message = TempData["Message"];
            }
            return View(objsignup);
        }
        [HttpPost]
        public ActionResult Save_User(MemberSignUp objsignup)
        {
            TempData["Message"] = "";

            try
            {
                var usercount= context.aspnet_Users.Where(x => x.UserName == objsignup.UserName).Count();
                if (usercount == 0)
                {
                    MembershipCreateStatus status;
                    var user = Membership.CreateUser(objsignup.UserName, objsignup.Password, objsignup.Email, null, null, false, out status);
                    if (Convert.ToString(status) == "Success")
                    {
                        try
                        {
                            var callbackUrl = Url.Action("ActivateUser", "SignUp", new { userIdval = user.ProviderUserKey }, protocol: Request.Url.Scheme);
                            string varificationMsg = "Dear " + objsignup.UserName + ",<br/>";
                            varificationMsg += "Thank you for signing up.<br/>";
                            varificationMsg += "Please visit <a target='_blank' href=" + callbackUrl + ">to activate your account and login.</a>";
                            varificationMsg += "<br/>Regards!<br/>";
                            varificationMsg += "StaffandTrain";
                            string body = varificationMsg;
                            body = body.Replace("resetpasswordlink", callbackUrl);
                            sm.SendToEmail("Account activation mail -StaffandTrain", body, "prachi.s@cisinlabs.com");
                            TempData["Message"] = "Success";
                        }
                        catch (Exception ex)
                        {
                            Membership.DeleteUser(user.UserName);
                            TempData["Message"] = "Some Error Occured";
                        }
                    }
                    else
                    {
                        Membership.DeleteUser(user.UserName);
                        TempData["Message"] = "Some Error Occured";
                    }
                }
                else
                {
                    TempData["Message"] = "User already exist";

                }
            }
            catch (MembershipCreateUserException e)
            {
                var msg = GetErrorMessage(e.StatusCode);
                TempData["Message"] = msg;
            }

            return RedirectToAction("Index");
        }

        public ActionResult ActivateUser(string userIdval)
        {
            try
            {
                var userID = new Guid(userIdval);
                var userdetails = context.aspnet_Membership.Where(x => x.UserId == userID).FirstOrDefault();
                if (userdetails != null)
                {
                    userdetails.IsApproved = true;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {


            } 
            return View();
        }


        public JsonResult checkuser(string Username)
        {
            int usercount = 0;
            usercount = context.aspnet_Users.Where(x => x.UserName == Username).Count();
            return Json(usercount, JsonRequestBehavior.AllowGet);
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
    }
}