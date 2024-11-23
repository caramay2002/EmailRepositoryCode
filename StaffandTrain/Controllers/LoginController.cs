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
    public class LoginController : Controller
    {
        SendEmail _email = new SendEmail();
        SATConn context = new SATConn();
        Common.Common cm = new Common.Common();
        // GET: Login
        public ActionResult Index()
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Message = TempData["Msg"];
            }
            LoginViewModel objlogin = new LoginViewModel();
            if (Request.Cookies["unm"] != null)
            {
                HttpCookie unm = (HttpCookie)Request.Cookies["unm"];
                objlogin.Email = unm.Value;
            }
            if (Request.Cookies["pas"] != null)
            {
                HttpCookie pas = (HttpCookie)Request.Cookies["pas"];
                objlogin.Password = pas.Value;
            }
            if (Request.Cookies["rem"] != null)
            {
                HttpCookie rem = (HttpCookie)Request.Cookies["rem"];
                if (rem.Value.ToLower() == "true")
                    objlogin.RememberMe = true;
                else
                    objlogin.RememberMe = false;
            }
            return View(objlogin);
           
        }

        public ActionResult Login()
        {

            if (TempData["Msg"] != null)
            {
                ViewBag.Message = TempData["Msg"];
            }
            LoginViewModel objlogin = new LoginViewModel();
            return View(objlogin);
        }

        public ActionResult UserLogin(LoginViewModel objlogin)
        {
            try
            {
                MembershipUser CurrentUser1 = Membership.GetUser(objlogin.Email);
                if (Membership.ValidateUser(objlogin.Email, objlogin.Password))
                {
                    MembershipUser CurrentUser = Membership.GetUser(objlogin.Email);
                    if (CurrentUser.IsApproved)
                    {
                        var roleslist = Roles.GetRolesForUser(objlogin.Email);
                        var userrole = roleslist.FirstOrDefault();
                        Global.Role = userrole;
                        Global.UserName = objlogin.Email;
                        FormsAuthentication.SetAuthCookie(objlogin.Email, true);
                        //#region Remember Me

                        //HttpCookie unm = new HttpCookie("unm", objlogin.Email);
                        //HttpCookie pas = new HttpCookie("pas", objlogin.Password);
                        //HttpCookie rem = new HttpCookie("rem", objlogin.RememberMe.ToString());

                        //if (objlogin.RememberMe)
                        //{
                        //    unm.Expires = DateTime.Now.AddDays(15);
                        //    pas.Expires = DateTime.Now.AddDays(15);
                        //    rem.Expires = DateTime.Now.AddDays(15);
                        //}
                        //else
                        //{
                        //    unm.Expires = DateTime.Now.AddDays(-1);
                        //    pas.Expires = DateTime.Now.AddDays(-1);
                        //    rem.Expires = DateTime.Now.AddDays(-1);
                        //}
                        //System.Web.HttpContext.Current.Response.Cookies.Add(unm);
                        //System.Web.HttpContext.Current.Response.Cookies.Add(pas);
                        //System.Web.HttpContext.Current.Response.Cookies.Add(rem);

                        //#endregion

                        //FormsAuthentication.SetAuthCookie(objlogin.Email, false);
                        UpdateLastLoginDate(CurrentUser.Email);
                        if (userrole =="Admin")
                        {
                            return RedirectToAction("Index", "AdminHome");
                        }
                        else if (userrole == "Recruiter")
                        {
                            return RedirectToAction("Index", "RecruiterHome");
                        }
                        else  
                        {
                            return RedirectToAction("Index", "ClientHome");
                        }
                    }
                    else
                    {
                        TempData["Msg"] = "User account is not activated yet";

                    }
                }
                else
                {
                    TempData["Msg"] = "Invalid User";
                }


            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error";
                cm.ErrorExceptionLogingByService(ex.ToString(), "UserLogin" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
                 
            }
            
            return RedirectToAction("Index");
        }

       


        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Roles.DeleteCookie();
            Session.Clear();
            Global.Role = "";
            Global.UserName = "";
            return RedirectToAction("Index","Home");
        }

        // Update last login date in the database
        private void UpdateLastLoginDate(string email)
        {
            var user = context.aspnet_Membership.Where(x => x.Email == email).FirstOrDefault();
            user.LastLoginDate = DateTime.Now;
            context.SaveChanges();
        }
    }
}