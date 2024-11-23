using StaffandTrain.Common;
using StaffandTrain.DataModel;
using StaffandTrain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StaffandTrain.Controllers
{
    [NoCache]
    [Authorize(Roles = "Admin,Recruiter")]
    public class ProspectViewCompanyController : Controller
    {
        // GET: ProspectViewCompany
        SATConn context = new SATConn();
        Common.Common cm = new Common.Common();
        public ActionResult Index(string Compid)

        {
            int n;
            bool isNumeric = int.TryParse(Compid, out n);
            if (isNumeric == true)
            {
                Compid = cm.Code_Encrypt(CryptorEngine.Encrypt(Compid));
            }

            if (string.IsNullOrEmpty(Compid))
            {
                return RedirectToAction("ProspectListsAdmin", "Index");
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
                            objcomp.objcontacts.Add(objcondetails);
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
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompany" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
            }
            return View(objcomp);
        }



        public ActionResult AddUpdateContact(ContactDetails objcont)
        {
            var Titlelist = context.SPgettitle().Select(xx => new SelectListItem
            {
                Value = xx.ToString(),
                Text = xx.ToString()
            }).ToList();
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
                var contdata = context.SpGetContactDataforedit(objcont.contactid).FirstOrDefault();
                if (contdata != null)
                {
                    // objcont.contactid = contdata.contactid;
                    objcont.contactfullname = contdata.contactfullname;
                    objcont.contactemail = contdata.contactemail;
                    objcont.contactcellphone = contdata.contactcellphone;
                    objcont.titlestandard = contdata.titlestandard;
                    objcont.linkedinprofileurl = contdata.linkedinprofileurl;
                    objcont.combinednotes = contdata.combinednotes;
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
                    context.SpaddContact(objcont.contactfullname, objcont.titlestandard, "", objcont.contactcellphone, objcont.contactemail, objcont.linkedinprofileurl, objcont.combinednotes, objcont.companyid);
                    context.SaveChanges();

                }
                else
                {
                    context.SpupdateContact(objcont.contactfullname, objcont.titlestandard, "", objcont.contactcellphone, objcont.contactemail, objcont.linkedinprofileurl, objcont.combinednotes, objcont.contactid);
                    context.SaveChanges();
                }
                TempData["ConEmail"] = objcont.contactemail;
            }
            catch (Exception ex)
            {

                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompany" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "SaveContact", "NA", "NA", "NA", "WEB");
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

                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompany" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "DeleteContact", "NA", "NA", "NA", "WEB");
            }
            return RedirectToAction("Index", new { @Compid = compid });
        }
        public ActionResult Getlist(string name, string Notes, int listid, int companyid, string emailaddes)
        {
            var data = new List<ContactDetails>();
            try
            {

                var contactdata = context.SpContactFilter(listid, companyid).ToList();
                if (contactdata.Count() > 0)
                {
                    var companyname = context.SPgetCompdetailsbycompid(companyid).FirstOrDefault().name;

                    if ((name != "" && Notes != ""))
                    {

                        data = (from con in contactdata
                                where
                                  (
                                  (name == null || name == "") || con.contactfullname.ToUpper().Contains(name.ToUpper())) &&
                                ((Notes == "" || Notes == null) || con.combinednotes.Trim().Replace("\r", "").
                                Replace("\n", "").Replace(System.Environment.NewLine, "").Replace(" ", "").
                      Contains(Notes.Trim().Replace("\r", "").Replace("\n", "").Replace(System.Environment.NewLine, "").Replace(" ", "")))
                                select new ContactDetails
                                {
                                    contactid = con.contactid,
                                    companyid = con.companyid,
                                    contactfullname = con.contactfullname,
                                    titlestandard = con.titlestandard,
                                    contactphone = con.contactphone,
                                    contactcellphone = con.contactcellphone,
                                    contactemail = con.contactemail,
                                    listid = con.listid,
                                    linkedinprofileurl = con.linkedinprofileurl,
                                    combinednotes = con.combinednotes,
                                    compname = companyname,
                                    ResumeFile = con.ResumeFilePath
                                }).Distinct().ToList();

                        ViewBag.ConEmail = emailaddes;
                    }
                    else if (Notes != "" && Notes != null)
                    {
                        contactdata = contactdata.Where(x => x.combinednotes != null).ToList();
                        data = (from con in contactdata
                                where
                                ((con.combinednotes.Trim().Replace("\r", "").
                                Replace("\n", "").Replace(System.Environment.NewLine, "").Replace(" ", "").
                                Contains(Notes.Trim().Replace("\r", "").
                                Replace("\n", "").Replace(System.Environment.NewLine, "").Replace(" ", ""))))
                                select new ContactDetails
                                {
                                    contactid = con.contactid,
                                    companyid = con.companyid,
                                    contactfullname = con.contactfullname,
                                    titlestandard = con.titlestandard,
                                    contactphone = con.contactphone,
                                    contactcellphone = con.contactcellphone,
                                    contactemail = con.contactemail,
                                    listid = con.listid,
                                    linkedinprofileurl = con.linkedinprofileurl,
                                    combinednotes = con.combinednotes,
                                    compname = companyname,
                                    ResumeFile = con.ResumeFilePath
                                }).Distinct().ToList();

                        ViewBag.ConEmail = emailaddes;
                    }
                    else if (name != "" && name != null)
                    {
                        contactdata = contactdata.Where(x => x.contactfullname != null).ToList();
                        data = (from con in contactdata
                                where
                                (con.contactfullname.ToUpper().Contains(name.ToUpper()))
                                select new ContactDetails
                                {
                                    contactid = con.contactid,
                                    companyid = con.companyid,
                                    contactfullname = con.contactfullname,
                                    titlestandard = con.titlestandard,
                                    contactphone = con.contactphone,
                                    contactcellphone = con.contactcellphone,
                                    contactemail = con.contactemail,
                                    listid = con.listid,
                                    linkedinprofileurl = con.linkedinprofileurl,
                                    combinednotes = con.combinednotes,
                                    compname = companyname,
                                    ResumeFile = con.ResumeFilePath
                                }).Distinct().ToList();
                        ViewBag.ConEmail = emailaddes;
                    }
                    else
                    {

                        //  data = (from con in contactdata
                        //          where
                        //            (
                        //            (name == null || name == "") || con.contactfullname.ToUpper().Contains(name.ToUpper())) ||
                        //          ((Notes == "" || Notes == null) || con.combinednotes.Trim().Replace("\r", "").
                        //          Replace("\n", "").Replace(System.Environment.NewLine, "").Replace(" ", "").
                        //Contains(Notes.Trim().Replace("\r", "").Replace("\n", "").Replace(System.Environment.NewLine, "").Replace(" ", "")))

                        data = (from con in contactdata
                                select new ContactDetails
                                {
                                    contactid = con.contactid,
                                    companyid = con.companyid,
                                    contactfullname = con.contactfullname,
                                    titlestandard = con.titlestandard,
                                    contactphone = con.contactphone,
                                    contactcellphone = con.contactcellphone,
                                    contactemail = con.contactemail,
                                    listid = con.listid,
                                    linkedinprofileurl = con.linkedinprofileurl,
                                    combinednotes = con.combinednotes,
                                    compname = companyname,
                                    ResumeFile = con.ResumeFilePath
                                }).Distinct().ToList();

                        ViewBag.ConEmail = emailaddes;
                    }
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompany" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Getlist", "NA", "NA", "NA", "WEB");

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
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompany" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "GetConResume", "NA", "NA", "NA", "WEB");
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
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompany" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "UpdateConResume", "NA", "NA", "NA", "WEB");
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateConResumeFile(FormCollection collection)
        {

            Int32 contactid = Convert.ToInt32(Request.Form["contactid"]);
            string resumes = (Request.Form["resumes"]);
            // Checking no of files injected in Request object
            if (Request.Files.Count > 0)
            {
                try
                {
                    // Get all files from Request object
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string CustomFileName;
                        string FilePath;
                        CustomFileName = contactid + Path.GetExtension(file.FileName);
                        var DbFilePath = context.Contacts.Where(x => x.contactid == contactid).Select(x => x.ResumeFilePath).FirstOrDefault();

                        if (DbFilePath != null)
                        {
                            string FullFilePath = Server.MapPath(DbFilePath);
                            if (System.IO.File.Exists(FullFilePath))
                            {
                                System.IO.File.Delete(FullFilePath);
                            }
                        }

                        // Get the complete folder path and store the file inside it.
                        FilePath = Server.MapPath("/UploadedResumes/" + CustomFileName);
                        file.SaveAs(FilePath);

                        //save the file in database
                        var stdComplete = context.Contacts.Where(x => x.contactid == contactid).SingleOrDefault();
                        stdComplete.ResumeFilePath = "/UploadedResumes/" + CustomFileName;
                        context.Entry(stdComplete).State = EntityState.Modified;
                        context.SaveChanges();
                        if (resumes != null)
                        {
                            context.spupdatecontactresumes(resumes, contactid);
                            context.SaveChanges();
                        }
                    }
                    // Returns message that successfully uploaded
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                if (resumes != null)
                {
                    context.spupdatecontactresumes(resumes, contactid);
                    context.SaveChanges();
                }
                return Json("Resume Uploaded Successfully");
            }
        }

        public JsonResult MoveContact(int contactid, int companyid, int companyidold)
        {
            string msg = "";
            try
            {
                context.SpMoveContact(companyid, contactid, companyidold);
                context.SaveChanges();
                msg = Url.Action("Index", "ProspectViewCompany", new { @Compid = @cm.Code_Encrypt(CryptorEngine.Encrypt(companyid.ToString())) });

            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompany" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "MoveContact", "NA", "NA", "NA", "WEB");
            }

            return Json(msg, JsonRequestBehavior.AllowGet);


        }


        public JsonResult GetContactDetails(string contactid)
        {
            ContactDetails objcont = new Models.ContactDetails();

            //if (Request.QueryString["CompanyId"] != null)
            //{
            //    int CompidIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(Request.QueryString["CompanyId"]))));
            //    objcont.companyid = CompidIddecrypt;
            //}
            if (contactid != null)
            {
                int ConIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(contactid)));
                objcont.contactid = ConIddecrypt;

            }
            var Titlelist = context.SPgettitle().Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
            ViewData["Titlelist"] = Titlelist;

            if (objcont.contactid != 0)
            {
                var contdata = context.SpGetContactDataforedit(objcont.contactid).FirstOrDefault();
                if (contdata != null)
                {
                    // objcont.contactid = contdata.contactid;
                    objcont.contactfullname = contdata.contactfullname;
                    objcont.contactemail = contdata.contactemail;
                    objcont.contactcellphone = contdata.contactcellphone;
                    objcont.contactphone = contdata.contactphone;
                    objcont.titlestandard = contdata.titlestandard;
                    objcont.linkedinprofileurl = contdata.linkedinprofileurl;
                    objcont.combinednotes = contdata.combinednotes;
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
                context.SpupdateContact(objcont.contactfullname, objcont.titlestandard, objcont.contactphone, objcont.contactcellphone, objcont.contactemail, objcont.linkedinprofileurl, objcont.combinednotes, objcont.contactid);
                context.SaveChanges();
                strmsg = "Success";
                TempData["ConEmail"] = objcont.contactemail;
            }
            catch (Exception ex)
            {
                strmsg = "Error";
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewCompany" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "SaveContact", "NA", "NA", "NA", "WEB");
            }
            return Json(strmsg, JsonRequestBehavior.AllowGet);

        }
    }
}