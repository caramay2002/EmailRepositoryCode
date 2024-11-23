using StaffandTrain.Common;
using StaffandTrain.Models;
using StaffandTrain.Utility;
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
    public class ForgotPassWordController : Controller
    {
        // GET: ForgotPassWord
        Common.SendEmail sm = new Common.SendEmail();
        Common.Common cm = new Common.Common();
        
        public ActionResult Index()
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Message = TempData["Msg"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    MembershipUser CurrentUser = Membership.GetUser(model.Email);
                    if (CurrentUser != null)
                    {
                        var userid = CurrentUser.ProviderUserKey;
                        var callbackUrl = Url.Action("ResetPassword", "ForgotPassWord", new { userIdval = userid }, protocol: Request.Url.Scheme);

                        //Send reset password email

                        string varificationMsg = "Dear UserName,<br/>";
                        varificationMsg += "Your link to reset password is<br/>";
                        varificationMsg += "<a target='_blank' href=" + callbackUrl + ">Please Click on the link to reset password</a>";
                        varificationMsg += "<br/>Regards!<br/>";
                        varificationMsg += "StaffandTrain";
                        string body = varificationMsg;
                        body = body.Replace("resetpasswordlink", callbackUrl);
                        string UserName = CurrentUser.UserName;
                        body = body.Replace("UserName", UserName);
                        sm.SendToEmail("Reset Password", body, "prachi.s@cisinlabs.com");

                        TempData["Msg"] = "Reset Password link has been sent to your email";
                    }
                    else
                    {
                        TempData["Msg"] = "User not exist";
                    }
                    //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                }
                catch (Exception ex)
                {
                    TempData["Msg"] = "Error";
                    cm.ErrorExceptionLogingByService(ex.ToString(), "ForgotPassWord" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "ForgotPassWord", "NA", "NA", "NA", "WEB");

                }
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [HttpGet]
        public ActionResult ResetPassword(string userIdval)
        {
            ResetPasswordViewModel objreset = new ResetPasswordViewModel();
            try
            {
                if (TempData["Msg"] != null)
                {
                    ViewBag.Message = TempData["Msg"];
                }

                Guid GuidUserId = new Guid(userIdval);
                objreset.UserId = GuidUserId.ToString();
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ForgotPassWord" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "ResetPassword", "NA", "NA", "NA", "WEB");

            }
            return View(objreset);
        }
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel objreset)
        {
            try
            {
                objreset.UserId = Request.Form["txtuserid"];
                Guid newid = new Guid(objreset.UserId);
                MembershipUser u = Membership.GetUser(newid);
                if (u != null)
                {
                 var status=u.ChangePassword(u.ResetPassword(), objreset.Password);
                    if (status)
                    {
                        TempData["Msg"] = "Your password has been reset.";
                    }
                }
                
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Some Error Occured.";
                cm.ErrorExceptionLogingByService(ex.ToString(), "ForgotPassWord" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "ResetPassword", "NA", "NA", "NA", "WEB");

            }
            return RedirectToAction("ResetPassword", new { userIdval = objreset.UserId });
        }




    }
}