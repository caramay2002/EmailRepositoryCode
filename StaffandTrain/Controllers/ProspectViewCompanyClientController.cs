using StaffandTrain.Common;
using StaffandTrain.DataModel;
using StaffandTrain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StaffandTrain.Controllers
{
    [NoCache]
    [Authorize(Roles = "Client")]
    public class ProspectViewCompanyClientController : Controller
    {
        // GET: ProspectViewCompanyClient
        SATConn context = new SATConn();
        Common.Common cm = new Common.Common();
        public ActionResult Index(string Compid)
        {
            if (string.IsNullOrEmpty(Compid))
            {
                return RedirectToAction("Index", "ProspectListsClient");
            }
            CompContact objcomp = new CompContact();
            try
            {
                int CompidIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(Compid))));
                var data = context.SPgetCompdetailsbycompid(CompidIddecrypt).FirstOrDefault();
                if (data != null)
                {
                    objcomp.name = data.name;
                    objcomp.biztype = data.biztype;
                    objcomp.addr2 = data.addr2;
                    objcomp.addr1 = data.addr1;
                    objcomp.weburl = data.weburl;
                    if (data.weburl != "" && data.weburl != null)
                    {
                        if (!data.weburl.Contains("http://") || !data.weburl.Contains("https://"))
                        {
                            objcomp.weburl = "http://" + data.weburl;

                        }

                    }
                    objcomp.city = data.city;
                    objcomp.state = data.state;
                    objcomp.zip = data.zip;
                    if (data.target == null)
                    {
                        objcomp.target = false;
                    }
                    else
                    {
                        objcomp.target = Convert.ToBoolean(data.target);
                    }
                    if (data.priority == null)
                    {
                        objcomp.priority = false;
                    }
                    else
                    {
                        objcomp.priority = Convert.ToBoolean(data.priority);
                    }

                    objcomp.phone = data.phone;
                    objcomp.citycircle = data.citycircle;
                    objcomp.listid = data.listid;
                    objcomp.companyid = CompidIddecrypt;
                    var CompContacts = context.SpGetContactlistbyCompid(CompidIddecrypt, objcomp.listid).ToList();
                    if (CompContacts != null)
                    {
                        foreach (var i in CompContacts)
                        {
                            ContactDetails objcondetails = new ContactDetails();
                            objcondetails.contactid = i.contactid;
                            objcondetails.companyid = i.companyid;
                            objcondetails.contactfullname = i.contactfullname;
                            objcondetails.titlestandard = i.titlestandard;
                            objcondetails.contactphone = i.contactphone;
                            objcondetails.contactcellphone = i.contactcellphone;
                            objcondetails.contactemail = i.contactemail;
                            objcondetails.listid = i.listid;
                            objcondetails.linkedinprofileurl = i.linkedinprofileurl;
                            objcondetails.combinednotes = i.combinednotes;
                            objcondetails.contactemail = i.contactemail;
                        }
                    }
                }

                var Complst = context.SPgetCompanies(objcomp.listid).ToList().Select(xx => new SelectListItem { Value = xx.companyid.ToString(), Text = xx.name + " - Business Type : " + xx.biztype }).ToList();
                ViewData["Complst"] = Complst;
                if (TempData["ConEmail"] != null)
                {
                    ViewBag.ConEmail = TempData["ConEmail"];
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompanyClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
            }
            return View(objcomp);
        }


        public ActionResult AddUpdateContact(ContactDetails objcont)
        {
            var Titlelist = context.SPgettitle().Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
            ViewData["Titlelist"] = Titlelist;
            if (Request.QueryString["CompanyId"] != null)
            {
                int CompidIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(Request.QueryString["CompanyId"]))));
                objcont.companyid = CompidIddecrypt;
            }

            if (Request.QueryString["contactid"] != null)
            {
                int ConIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(Request.QueryString["contactid"]))));
                objcont.contactid = ConIddecrypt;
                // objcont.contactid = Convert.ToInt32(Request.QueryString["contactid"]);
            }
            if (Request.QueryString["compname"] != null)
            {
                ViewBag.compname = Convert.ToString(Request.QueryString["compname"]);
            }
            if (objcont.contactid != 0)
            {
                var contdata = context.SpGetContactDataforeditClient(objcont.contactid,User.Identity.Name, objcont.companyid).FirstOrDefault();
                if (contdata != null)
                {
                    // objcont.contactid = contdata.contactid;
                    objcont.contactfullname = contdata.contactfullname;
                    objcont.contactemail = contdata.contactemail;
                    objcont.contactcellphone = contdata.contactcellphone;
                    objcont.contactphone = contdata.contactphone;
                    objcont.titlestandard = contdata.titlestandard;
                    objcont.linkedinprofileurl = contdata.linkedinprofileurl;
                    objcont.combinednotes = contdata.contactnotes;
                    objcont.companyid = contdata.companyid;
                }


            }

            return View(objcont);

        }


        public ActionResult SaveContact(ContactDetails objcont)
        {
            try
            {
                objcont.companyid = Convert.ToInt32(Request.Form["txtcompid"]);
                objcont.contactid = Convert.ToInt32(Request.Form["txtconid"]);
                if (objcont.contactid == 0)
                {
                    ObjectParameter objParam = new ObjectParameter("NewContactID", typeof(int));
                    var NewContactid= context.SpaddContactClient(objcont.contactfullname, objcont.titlestandard, objcont.contactphone, objcont.contactcellphone, objcont.contactemail, objcont.linkedinprofileurl, "", objcont.companyid,"", objcont.combinednotes,User.Identity.Name , objParam).FirstOrDefault();
                    context.SaveChanges();
                    objcont.contactid = Convert.ToInt32(NewContactid);
                }
                else
                {
                    context.SpupdateContactClient(objcont.contactfullname, objcont.titlestandard, objcont.contactphone, objcont.contactcellphone, objcont.contactemail, objcont.linkedinprofileurl, objcont.combinednotes, objcont.contactid,"");
                    context.SaveChanges();
                }
                var data = context.Spgetclientnote(objcont.contactid, User.Identity.Name);
                if (data.Count() == 0)
                {
                    context.SpInsertclientnote(objcont.contactid, User.Identity.Name, objcont.combinednotes);
                    context.SaveChanges();
                       
                }
                else
                {

                    context.Spupdateclientnote(objcont.contactid, User.Identity.Name, objcont.combinednotes);
                    context.SaveChanges();

                }
             
                
                TempData["ConEmail"] = objcont.contactemail;
            }
            catch (Exception ex)
            {

                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompanyClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "SaveContact", "NA", "NA", "NA", "WEB");
            }


            return RedirectToAction("Index", new { @Compid = @cm.Code_Encrypt(CryptorEngine.Encrypt(objcont.companyid.ToString())) });

        }

        public ActionResult DeleteContact(string contactid, string compid)
        {
            try
            {
                int CompidIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(compid))));
                int ConIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(contactid))));
                context.SPdeleteContact(ConIddecrypt);
                context.SaveChanges();
            }
            catch (Exception ex)
            {

                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompanyClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "DeleteContact", "NA", "NA", "NA", "WEB");
            }
            return RedirectToAction("Index", new { @Compid = compid });


        }
        public ActionResult Getlist(string name, string Notes, int listid, int companyid, string emailaddes)
        {
            var data = new List<ContactDetails>();
            try
            {

                var contactdata = context.SpContactFilterClient(listid, companyid,User.Identity.Name).ToList();
                if (contactdata.Count() > 0)
                {
                    var companyname = context.SPgetCompdetailsbycompid(companyid).FirstOrDefault().name;
                    data = (from con in contactdata
                            where
                              ((name == null || name == "") || con.contactfullname.ToUpper().Contains(name.ToUpper())) &&
               ((Notes == null || Notes == "") || con.contactnotes.Trim().Replace("\r", "").Replace("\n", "").Replace(System.Environment.NewLine, "").Replace(" ", "").Contains(Notes.Trim().Replace("\r", "").Replace("\n", "").Replace(System.Environment.NewLine, "").Replace(" ", "")))
                            select new ContactDetails
                            {
                                contactid = con.contactid,
                                companyid = con.companyid,
                                contactfullname = con.contactfullname,
                                titlestandard = con.titlestandard,
                                contactphone = con.contactphone,
                                contactcellphone = con.contactcellphone,
                                contactemail = con.contactemail,
                                listid = listid,
                                linkedinprofileurl = con.linkedinprofileurl,
                                combinednotes = con.contactnotes,
                                compname = companyname,
                                contactnotes = con.contactnotes,
                                Userid=con.Userid
                                

                            }).Distinct().ToList();

                    ViewBag.ConEmail = emailaddes;
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompanyClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Getlist", "NA", "NA", "NA", "WEB");

            }
            return View(data);
        }
        public JsonResult GetConResume(int contactid)
        {
            string msg = "";
            try
            {
                var contactResumedata = context.Contacts.Where(X => X.contactid == contactid).FirstOrDefault();
                if (contactResumedata != null)
                {
                    msg = contactResumedata.resumes;
                }
                if (msg == null)
                {
                    msg = "";
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompanyClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "GetConResume", "NA", "NA", "NA", "WEB");
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateConResume(int contactid, string resumes)
        {
            string msg = "";
            try
            {
                context.spupdatecontactresumes(resumes, contactid);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompanyClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "UpdateConResume", "NA", "NA", "NA", "WEB");
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MoveContact(int contactid, int companyid, int companyidold)
        {
            string msg = "";
            try
            {
                context.SpMoveContact(companyid, contactid, companyidold);
                context.SaveChanges();
                msg = Url.Action("Index", "ProspectViewCompanyClient", new { @Compid = @cm.Code_Encrypt(CryptorEngine.Encrypt(companyid.ToString())) });

            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompanyClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "MoveContact", "NA", "NA", "NA", "WEB");
            }

            return Json(msg, JsonRequestBehavior.AllowGet);


        }
        public ActionResult DeleteCompany(int companyidedit, int listid)
        {
            try
            {
                if (companyidedit != 0)
                {
                    context.DeleteContactCompanybyCompanyId(companyidedit);
                    context.SaveChanges();
                    TempData["Message"] = "Record Deleted";
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "DeleteCompany" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "DeleteCompany", "NA", "NA", "NA", "WEB");
            } 
            return RedirectToAction("Index", new { @listid = listid, @lstname = "" });
        }

        public JsonResult GetContactDetails(string contactid)
        {
            ContactDetails objcont = new Models.ContactDetails(); 
            var Titlelist = context.SPgettitle().Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
            ViewData["Titlelist"] = Titlelist;
            //if (Request.QueryString["CompanyId"] != null)
            //{
            //    int CompidIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(Request.QueryString["CompanyId"]))));
            //    objcont.companyid = CompidIddecrypt;
            //}
            if (contactid != null)
            {
                int ConIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(contactid)));
                objcont.contactid = ConIddecrypt;
                // objcont.contactid = Convert.ToInt32(Request.QueryString["contactid"]);
            }
            if (objcont.contactid != 0)
            {
                var contdata = context.SpGetContactDataforeditClient(objcont.contactid, User.Identity.Name, objcont.companyid).FirstOrDefault();
                if (contdata != null)
                {
                    // objcont.contactid = contdata.contactid;
                    objcont.contactfullname = contdata.contactfullname;
                    objcont.contactemail = contdata.contactemail;
                    objcont.contactcellphone = contdata.contactcellphone;
                    objcont.contactphone = contdata.contactphone;
                    objcont.titlestandard = contdata.titlestandard;
                    objcont.linkedinprofileurl = contdata.linkedinprofileurl;
                    objcont.combinednotes = contdata.contactnotes;
                    objcont.companyid = contdata.companyid;
                } 
            }
            return Json(new { data = objcont, Titlelist = Titlelist }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateContact(ContactDetails objcont)
        {
            string strmsg = "";
            try
            {
                // objcont.companyid = Convert.ToInt32(Request.Form["txtcompid"]);
                objcont.contactid = Convert.ToInt32(objcont.contactid);
                context.SpupdateContactClient(objcont.contactfullname, objcont.titlestandard, objcont.contactphone, objcont.contactcellphone, objcont.contactemail, objcont.linkedinprofileurl, objcont.combinednotes, objcont.contactid, "");
                context.SaveChanges();
                var data = context.Spgetclientnote(objcont.contactid, User.Identity.Name);
                if (data.Count() == 0)
                {
                    context.SpInsertclientnote(objcont.contactid, User.Identity.Name, objcont.combinednotes);
                    context.SaveChanges();
                }
                else
                {
                    context.Spupdateclientnote(objcont.contactid, User.Identity.Name, objcont.combinednotes);
                    context.SaveChanges();
                }
                strmsg = "Success";
                TempData["ConEmail"] = objcont.contactemail;
            }
            catch (Exception ex)
            {
                strmsg = "Error";
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompanyClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "SaveContact", "NA", "NA", "NA", "WEB");
            }
            return Json(strmsg, JsonRequestBehavior.AllowGet);

        }
    }
}