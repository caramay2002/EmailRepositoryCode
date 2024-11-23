using StaffandTrain.Common;
using StaffandTrain.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StaffandTrain.Controllers
{
    [NoCache]
    public class EmailTemplateController : Controller
    {
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();

        // GET: EmailTemplate
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var emaillist = new List<SpGetEmailtemplatelist_Result>();

            try
            {
                emaillist = context.SpGetEmailtemplatelist().ToList();
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "EmailTemplate" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
            }

            return View(emaillist);
        }

        public ActionResult SaveEmailtemplate(string TemplateId)
        {
            StaffandTrain.Models.EmailTemplate objemail = new StaffandTrain.Models.EmailTemplate();

            try
            {
                if (TempData["Message"] != null)
                {
                    ViewBag.Message = TempData["Message"];
                }

                if (!string.IsNullOrEmpty(TemplateId))
                {
                    int TemplateIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(TemplateId))));
                    var emailtempdata = context.SPgetemailtemplatebytemplateid(TemplateIddecrypt).FirstOrDefault();

                    if (emailtempdata != null)
                    {
                        objemail.TemplateId = emailtempdata.TemplateId;
                        objemail.TemplateIdDecrypt = emailtempdata.TemplateId;
                        objemail.TemplateName = emailtempdata.TemplateName;
                        //objemail.EmailBody = cm.StripHTML(emailtempdata.EmailBody.Replace("&nbsp;", "").Replace("nbsp;", ""));
                        objemail.EmailBody = (emailtempdata.EmailBody.Replace("&nbsp;", " ").Replace("nbsp;", " "));
                        objemail.Subject = emailtempdata.Subject;
                        objemail.GroupingNumber = emailtempdata.GroupingNumber;
                    }
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "EmailTemplate" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "SaveEmailtemplate", "NA", "NA", "NA", "WEB");
            }

            return View(objemail);
        }

        public ActionResult DeleteEmailtemplate(string TemplateId)
        {
            try
            {
                if (!string.IsNullOrEmpty(TemplateId))
                {
                    int TemplateIddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(TemplateId))));
                    context.SpDeleteEmailTemplate(TemplateIddecrypt);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "EmailTemplate" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "DeleteEmailtemplate", "NA", "NA", "NA", "WEB");
            }

            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult AddeditEmailtemplate(StaffandTrain.Models.EmailTemplate objemail)
        {
            try
            {
                if (objemail.TemplateIdDecrypt == 0)
                {
                    context.SpInsertEmailTemplate(objemail.TemplateName, objemail.Subject, objemail.GroupingNumber, objemail.EmailBody);
                    context.SaveChanges();
                    TempData["Message"] = "Record Saved";
                }
                else
                {
                    context.SpUpdateEmailTemplate(objemail.TemplateName, objemail.Subject, objemail.GroupingNumber, objemail.EmailBody, objemail.TemplateIdDecrypt);
                    context.SaveChanges();
                    TempData["Message"] = "Record Updated";
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "EmailTemplate" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "AddeditEmailtemplate", "NA", "NA", "NA", "WEB");
            }

            if (objemail.TemplateIdDecrypt != 0)
            {
                return RedirectToAction("SaveEmailtemplate", new { TemplateId = CryptorEngine.Encrypt(cm.Code_Decrypt(Convert.ToString(objemail.TemplateIdDecrypt))) });
            }
            else
            {
                return RedirectToAction("SaveEmailtemplate");
            }
        }
    }
}