using StaffandTrain.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Net.Mail;
using System.IO;
using System.Text;
using System.Net.Mime;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using StaffandTrain.Common;
using System.Threading;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Data.Entity.Core.Objects;
using StaffandTrain.Models;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.EMMA;
using System.Web.Optimization;
using System.Net;

namespace StaffandTrain.Controllers
{
    [NoCache]
    public class EmailSendFromController : Controller
    {
        // GET: EmailSendFrom
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();
        private static String ErrorlineNo, Errormsg, extype, exurl, hostIp, ErrorLocation, HostAdd;

        List<EmailBatch> BatchValuesList = new List<EmailBatch>();  // List to manage the batch functionality with Email
        int ThumbnailHeight = 0;
        int ProductSearchHeight = 0;
        int ImageDetailHeight = 0;
        int ThumbnailWidth = 0;
        int ProductSearchWidth = 0;
        int ImageDetailWidth = 0;
        int Zoomerheight = 0;
        int ZommerWidth = 0;

        public ActionResult Index()
        {
            try
            {
                ViewData["CityCircle"] = context.SpgetProspectingselectlist().Select(xx => new SelectListItem { Value = xx.listid.ToString(), Text = xx.listname }).ToList();
                ViewData["Titlelist"] = context.SPgettitle().Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
                
                var templateList = context.SpGetEmailtemplatelist().GroupBy(e=>e.GroupingNumber);
                
                var templateListData = new List<SelectListItem>();
                foreach (var group in templateList)
                {
                    var optionGroup = new SelectListGroup() { Name = $"Group {group.Key}" };
                    foreach (var item in group)
                    {
                        templateListData.Add(new SelectListItem()
                        {
                            Value = item.TemplateId.ToString(),
                            Text = item.TemplateName.ToString(),
                            Group = group.Key != null ? optionGroup : null,
                        });
                    }
                }

                ViewData["Templatelist"] = templateListData;

                if (TempData["Message"] != null)
                {
                    ViewBag.Message = TempData["Message"];
                }
            }
            catch(Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "EmailSendFrom" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
            }

            return View();
        }

        public JsonResult Getbiztype(int id)
        {
            List<SelectListItem> biztype = new List<SelectListItem>();

            try
            {
                biztype = context.SPgetbiztype(id).Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
                biztype.Insert(0, new SelectListItem { Value = "", Text = "No Options" });
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "EmailSendFrom" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Getbiztype", "NA", "NA", "NA", "WEB");
            }

            return Json(biztype);
        }

        public JsonResult GetTemplatedata(int id)
        {
            if (Request.IsAuthenticated)
            {
                var data = new SPgetemailtemplatebytemplateid_Result();

                try
                {
                    data = context.SPgetemailtemplatebytemplateid(id).FirstOrDefault();

                    if (data != null)
                    {
                        data.EmailBody = (data.EmailBody).Replace("&nbsp;", " ").Replace("nbsp;", " ");
                    }
                }
                catch (Exception ex)
                {
                    cm.ErrorExceptionLogingByService(ex.ToString(), "EmailSendFrom" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Getbiztype", "NA", "NA", "NA", "WEB");
                }

                return Json(data);
            }
            else
            {
                return Json("LoginRequired");
            }
        }

        //OLD SEND EMAIL METHOD [NOT IN USE NOW] - Commented by Shivam
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult Sendemail(EmailTemplate obj)
        //{
        //    try
        //    {
        //        int prospectList = 0;

        //        string prospectListvalue = Request.Form["ddlCityCircle"];

        //        if (prospectListvalue != "")
        //        {
        //            prospectList = Convert.ToInt32(Request.Form["ddlCityCircle"]);
        //        }

        //        string BizzTYpe, titleStandard;

        //        if (Request.Form["ddlbiztype"] == "")
        //        {
        //            BizzTYpe = "";
        //        }
        //        else
        //        {
        //            BizzTYpe = Request.Form["ddlbiztype"];
        //        }

        //        if (Request.Form["ddltitle"] == "")
        //        {
        //            titleStandard = "";
        //        }
        //        else
        //        {
        //            titleStandard = Request.Form["ddltitle"];
        //        }

        //        string TemplateId = Request.Form["ddltemplate"];
        //        bool enablessl = false;

        //        if (Request.Form["ddlenablessl"] == "1")
        //        {
        //            enablessl = true;
        //        }
        //        else
        //        {
        //            enablessl = false;
        //        }

        //        int intmailCount = 0;

        //        var templatedata = context.SPgetemailtemplatebytemplateid(Convert.ToInt32(TemplateId)).FirstOrDefault();
        //        templatedata.EmailBody = obj.EmailBody;

        //        string ImageName = Request.Form["hiddenImageName"].ToString();
        //        string EmailBatchVal = Request.Form["EmailBatchVal"].ToString();

        //        if (BizzTYpe == "")
        //        {
        //            int BatchCount = context.SPgetconformailwithoutbiztype(prospectList, titleStandard).Count();
        //            var EmailBatchData = BatchFunction(BatchCount);

        //            int batchStartingVal = 0;
        //            int batchEndingVal = 0;

        //            for (int i = 0; i < EmailBatchData.ToList().Count; i++)
        //            {
        //                if (EmailBatchVal == BatchValuesList[i].BatchName)
        //                {
        //                    batchStartingVal = BatchValuesList[i].BatchStartingCount;
        //                    batchEndingVal = BatchValuesList[i].BatchEndingCount;
        //                    break;
        //                }
        //            }

        //            var condata = context.SPgetconformailwithoutbiztype_batchEmail(prospectList, titleStandard, batchStartingVal, batchEndingVal).ToList();
        //            //var condata = context.SPgetconformailwithoutbiztype(prospectList, titleStandard).ToList();

        //            foreach (var i in condata)
        //            {
        //                try
        //                {
        //                    int value = sendemailcontact(i.contactfullname, i.contactemail, Request.Form["txtsubject"], templatedata.EmailBody, Request.Form["txtemail"], Request.Form["txtservername"], Request.Form["txtpassword"], Request.Form["txtportno"], enablessl, ImageName);

        //                    if (value == 1)
        //                    {
        //                        intmailCount++;
        //                    }
        //                }
        //                catch(Exception ex)
        //                {
        //                    continue;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // Applied by omkar start here 
        //            int BatchCount = context.SPgetcontactformailsend(prospectList, BizzTYpe, titleStandard).Count();

        //            var EmailBatchData = BatchFunction(BatchCount);

        //            int batchStartingVal = 0;
        //            int batchEndingVal = 0;

        //            for (int i = 0; i < EmailBatchData.ToList().Count; i++)
        //            {
        //                if (EmailBatchVal == BatchValuesList[i].BatchName)
        //                {
        //                    batchStartingVal = BatchValuesList[i].BatchStartingCount;
        //                    batchEndingVal = BatchValuesList[i].BatchEndingCount;
        //                    break;
        //                }
        //            }

        //            var condata = context.SPgetcontactformailsend_batchEmail(prospectList, BizzTYpe, titleStandard, batchStartingVal, batchEndingVal).ToList();
        //            // Applied by omkar end here 


        //            //var condata = context.SPgetcontactformailsend(prospectList, BizzTYpe, titleStandard).ToList();
        //            foreach (var i in condata)
        //            {
        //                // convert and split the base 64 to image and store it in a file.
        //                //string str = templatedata.EmailBody;
        //                //string[] strArr = null, strArr2 = null;
        //                //string[] stringSeparators = new string[] { "base64," };
        //                //strArr = str.Split(stringSeparators, StringSplitOptions.None);
        //                //if (strArr.Length > 1)
        //                //{
        //                //    strArr2 = strArr[1].Split('"');
        //                //    byte[] bytes = Convert.FromBase64String(strArr2[0].Trim());
        //                //    int aa = strArr2.Length;
        //                //    Image image;
        //                //    Random randm = new Random();
        //                //    String r = randm.Next(0, 999999).ToString("D6");
        //                //    using (MemoryStream ms = new MemoryStream(bytes))
        //                //    {
        //                //        image = Image.FromStream(ms);
        //                //        if (!Directory.Exists(Server.MapPath("~/ImagesEmail")))
        //                //            Directory.CreateDirectory(Server.MapPath("~/ImagesEmail"));
        //                //        image.Save(Server.MapPath("~/ImagesEmail/" + r + ".jpg"));
        //                //    }

        //                //    string strpath = Server.MapPath("~/ImagesEmail/" + r + ".jpg");
        //                //    string[] newstring = null;
        //                //    string[] stringSeparator = new string[] { "<img" };
        //                //    newstring = strArr[0].Split(stringSeparator, StringSplitOptions.None);
        //                //    if (newstring.Length > 1)
        //                //    {
        //                //        //newstring[0] = newstring[0] + "<img src=" + "\"" + strpath + "\""+ "/>";
        //                //        newstring[0] = newstring[0] + "<p><img src=" + "\"" + strpath + "\"" + "/></p>";
        //                //        templatedata.EmailBody = newstring[0];
        //                //    }
        //                //}



        //                int value = sendemailcontact(i.contactfullname, i.contactemail, Request.Form["txtsubject"], templatedata.EmailBody, Request.Form["txtemail"], Request.Form["txtservername"], Request.Form["txtpassword"], Request.Form["txtportno"], enablessl, ImageName);
        //                if (value == 1)
        //                {
        //                    intmailCount++;
        //                }

        //            }

        //        }
        //        TempData["Message"] = "Total Mail sent : " + intmailCount;
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Message"] = "Error";
        //        cm.ErrorExceptionLogingByService(ex.ToString(), "EmailSendFrom" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Sendemail", "NA", "NA", "NA", "WEB");
        //    }

        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Sendemail(DataModel.EmailTemplate obj)
        {
            try
            {
                bool enablessl = false;

                if (Request.Form["ddlenablessl"] == "1")
                {
                    enablessl = true;
                }
                else
                {
                    enablessl = false;
                }
                //sendemailcontact("Noman", "noman.ali@arcanainfo.com", "Testing", "Asad khan Bhestang", "asad@dominicanrecruiters.com", "server.nearshore-usa.com", "asadpassword", "587", true, "");
                //int intmailCount =  sendemailcontact("Asad - BLC", "dev@talkboxsolutions.com", Request.Form["txtsubject"], "Asad Tesing normal email from somee", Request.Form["txtemail"], Request.Form["txtservername"], Request.Form["txtpassword"], Request.Form["txtportno"], enablessl, int.Parse(Request.Form["ddlspacing"].ToString()), "");



                int prospectList = 0;

                string prospectListvalue = Request.Form["ddlCityCircle"];

                if (prospectListvalue != "")
                {
                    prospectList = Convert.ToInt32(Request.Form["ddlCityCircle"]);
                }

                string BizzTYpe, titleStandard;

                if (Request.Form["ddlbiztype"] == "")
                {
                    BizzTYpe = "";
                }
                else
                {
                    BizzTYpe = Request.Form["ddlbiztype"];
                }

                if (Request.Form["ddltitle"] == "")
                {
                    titleStandard = "";
                }
                else
                {
                    titleStandard = Request.Form["ddltitle"];
                }

                string TemplateId = Request.Form["ddltemplate"];

                //bool enablessl = false;

                //if (Request.Form["ddlenablessl"] == "1")
                //{
                //    enablessl = true;
                //}
                //else
                //{
                //    enablessl = false;
                //}

                int spacing = 0;
                if(int.Parse(Request.Form["ddlspacing"].ToString()) != 0)
                {
                    spacing = int.Parse(Request.Form["ddlspacing"].ToString());
                }

                int intmailCount = 0;

                var templatedata = context.SPgetemailtemplatebytemplateid(Convert.ToInt32(TemplateId)).FirstOrDefault();

                templatedata.EmailBody = obj.EmailBody;

                string ImageName = Request.Form["hiddenImageName"].ToString();

                // Logic starts here for Batch email processing [SHIVAM]
                string EmailBatchVal = Request.Form["EmailBatchVal"].ToString();
                string BatchEmailCount = Request.Form["BatchEmailCount"].ToString();

                int start_index = 0;
                int end_index = 0;

                if (string.IsNullOrEmpty(EmailBatchVal))
                {
                    throw new ArgumentNullException(nameof(EmailBatchVal));
                }
                if (string.IsNullOrEmpty(BatchEmailCount))
                {
                    throw new ArgumentNullException(nameof(BatchEmailCount));
                }

                string lastBatchString = EmailBatchVal.Substring(EmailBatchVal.Length - 1);
                int BatchNumber = Convert.ToInt32(lastBatchString);
                int BatchEmailCountNumber = Convert.ToInt32(BatchEmailCount);
                var EmailBatchSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EmailBatchSize"]);
                for (int i = 0; i < BatchNumber; i++)
                {
                    if (i == 0)
                    {
                        start_index = start_index + 0;

                        if (BatchEmailCountNumber < EmailBatchSize)
                        {
                            if (BatchNumber - i == 1)
                            {
                                end_index = BatchEmailCountNumber;
                            }
                            else
                            {
                                end_index = EmailBatchSize;
                            }
                        }
                        else
                        {
                            end_index = end_index + EmailBatchSize;
                        }
                    }
                    else
                    {
                        if (BatchNumber - i == 1)
                        {
                            start_index = end_index;
                            end_index = start_index + BatchEmailCountNumber;
                        }
                        else
                        {
                            start_index = end_index;
                            end_index = EmailBatchSize * (i + 1);
                        }
                    }
                }

                end_index = BatchEmailCountNumber;

                if (BizzTYpe == "")
                {
                    var contact_details = context.SPGetDataForSendingEmailWithoutBizType(prospectList, titleStandard, start_index, end_index).ToList();
                    for (int i = 0; i < contact_details.Count; i++)
                    {
                        var item = contact_details[i];
                        int value = sendemailcontact(item.contactfullname, item.contactemail, Request.Form["txtsubject"], templatedata.EmailBody, Request.Form["txtcompany"], Request.Form["txtemail"], Request.Form["txtservername"], Request.Form["txtpassword"], Request.Form["txtportno"], enablessl, spacing, ImageName);
                        if (value == 1) intmailCount++;
                    }
                }
                else
                {
                    var contact_details = context.SPGetDataForSendingEmailWithBizType(prospectList, titleStandard, BizzTYpe, start_index, end_index).ToList();
                    for (int i = 0; i < contact_details.Count; i++)
                    {
                        var item = contact_details[i];
                        int value =     sendemailcontact(item.contactfullname, item.contactemail, Request.Form["txtsubject"], templatedata.EmailBody, Request.Form["txtcompany"], Request.Form["txtemail"], Request.Form["txtservername"], Request.Form["txtpassword"], Request.Form["txtportno"], enablessl, spacing, ImageName);
                        if (value == 1) intmailCount++;
                    }
                }

                TempData["Message"] = "Total Mail sent : " + intmailCount;

                // TempData["Message"] = "Total Mail sent : " + intmailCount;
            }
            catch (Exception ex)
            {
                var message = "Error: " + (ex.InnerException?.Message ?? ex.Message);
                TempData["Message"] = message;
                cm.ErrorExceptionLogingByService(ex.ToString(), "EmailSendFrom" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Sendemail", "NA", "NA", "NA", "WEB");
            }
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public int sendemailcontact(string Name, string ContactEmail, string Subject, string EmailBody,string companyname, string senderemail, string servername, string password, string portno, bool enablessl, int spacing, string ImageName = "")
        {
            int returnvalue = 0;

            try
            {
                string fullbody = "";
                string[] fName = string.IsNullOrEmpty(Name) ? new string[] { "" } : Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string str = EmailBody;
                string subj = Subject;

                string[] strArr = null;
                string[] stringSeparators = new string[] { "<p>" };
                strArr = str.Split(stringSeparators, StringSplitOptions.None);

                if (strArr.Length > 1)
                {
                    foreach (var item in strArr)
                    {
                        try
                        {
                            if (item.Contains("img"))
                            {
                                fullbody += "<p>" + item;
                            }
                            else
                            {
                                if (item != "")
                                {
                                    if (item != "" && !item.Contains("<br></p>"))
                                    {
                                        fullbody += "<p>" + item;
                                    }
                                    else
                                    {
                                        fullbody += "<p>" + item;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    fullbody = str;
                }


                string addfnamesubject = "";
                // Logic for font size for Name section in Email Body Starts here [SHIVAM]
                var placeholders = new Dictionary<string, string>();
                if (fName.Length > 0 && !string.IsNullOrEmpty(fName[0]))
                {
                    placeholders.Add("FirstName", fName[0]);
                    addfnamesubject = "Hi "+ fName[0].ToString()+",  "+Subject;
                }
                var placeholders2 = new Dictionary<string, string>();
                if (Subject.Length > 0 && !string.IsNullOrEmpty(Subject))
                {
                    placeholders2.Add("name_script", fName[0]);
                    placeholders2.Add("FirstName", fName[0]);
                }

                //if (addfnamesubject.Contains("{{name_script}}")){
                //    addfnamesubject = addfnamesubject.Replace(" Hi {{name_script}}, ", "");
                //}


                subj = Regex.Replace(Subject, @"\{\{(\w+)\}\}", match =>
                {
                    string placeholder = match.Groups[1].Value;
                    if (placeholders2.ContainsKey(placeholder))
                    {
                        return placeholders2[placeholder];
                    }
                    else
                    {
                        return match.Value; // Keep original if no match
                    }
                });
                // Replace placeholders with values using regular expressions
                str = Regex.Replace(EmailBody, @"\{\{(\w+)\}\}", match =>
                {
                    string placeholder = match.Groups[1].Value;
                    if (placeholders.ContainsKey(placeholder))
                    {
                        return placeholders[placeholder];
                    }
                    else
                    {
                        return match.Value; // Keep original if no match
                    }
                });

                


                // Logic for font size for Name section in Email Body Ends here [SHIVAM]
                var css = @"
                        <style>
                            .email-preview {font-family:'Roboto', sans-serif !important; color: #2f2f2f !imporatnt;}
                            .email-preview h1{font-size:24px;font-weight:normal;line-height:1.1;margin-bottom:10px !important; color: #070707 !important}
                            .email-preview h2{font-size:22px;font-weight:normal;line-height:1.1;margin-bottom:10px !important; color: #070707 !important}
                            .email-preview h3{font-size:20px;font-weight:normal;line-height:1.1;margin-bottom:10px !important; color: #070707 !important}
                            .email-preview h4{font-size:18px;font-weight:normal;line-height:1.1;margin-bottom:10px !important; color: #070707 !important}
                            .email-preview h5,
                            .email-preview p{font-size:16px;margin-bottom:10px !important;font-weight: 400;}
                            .email-preview h6{font-size:14px;margin-bottom:10px !important}
                            .email-preview ol,
                            .email-preview ul{font-size:16px;margin:10px 0;padding-left:20px !important}
                            .email-preview b,
                            .email-preview strong{font-weight:700 !important}
                            .email-preview address,
                            .email-preview em,
                            .email-preview i{font-style:italic !important}
                            .email-preview u{text-decoration:underline !important}
                            .email-preview div{margin:10px 0;padding:10px;border:1px solid #ccc !important}
                        </style>";

                var htmlbody = $@"
                    <html>
                    <head>{css}</head>
                    <body><div class=""email-preview"">{str}</div></body>
                    </html>";

                string mailBody = Server.HtmlDecode(htmlbody);
                string Email = ContactEmail.Trim();

                // Regular expression for basic email validation
                string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
                Regex regex = new Regex(pattern);

                // Use the Regex.IsMatch method to check if the email is valid
                var validEmail = regex.IsMatch(Email);
                if (!validEmail) throw new Exception($"Invalid Email: {Email}");

  
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true;
               

                MailMessage message = new MailMessage();

                //message.To.Add("info.usamaali@gmail.com");
                message.To.Add(ContactEmail);
                //message.To.Add("dev@talkboxsolutions.com");
                message.Subject = subj;// addfnamesubject;
                message.From = new System.Net.Mail.MailAddress(senderemail,companyname);

                var plainText = AlternateView.CreateAlternateViewFromString("This is the plain text version of the email.", null, "text/plain");
                var htmlBody = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/html");

                message.AlternateViews.Add(plainText);
                message.AlternateViews.Add(htmlBody);

                message.IsBodyHtml = true;
                message.AlternateViews.Add(Mail_Body(ImageName, mailBody));
                message.Headers.Add("Message-ID", $"<{Guid.NewGuid()}@nearshore-usa.com>");
                message.Headers.Add("X-Mailer", "Near Shore SMTP v1.0");
                using (SmtpClient SmtpMail = new SmtpClient())
                {
                    SmtpMail.Timeout = 30000; // Increase the timeout to 30 seconds
                    SmtpMail.UseDefaultCredentials = false;
                    SmtpMail.Host =  servername;
                    SmtpMail.Port =  Convert.ToInt32(portno);//Port for sending the mail  
                    SmtpMail.Credentials = new System.Net.NetworkCredential(senderemail, password);
                    SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpMail.EnableSsl = Convert.ToBoolean(enablessl);
                    SmtpMail.ServicePoint.MaxIdleTime = 10000;
                    SmtpMail.ServicePoint.SetTcpKeepAlive(true, 12000, 12000);
                    message.BodyEncoding = Encoding.Default;
                    
                    SmtpMail.Send(message); //Smtpclient to send the mail message  

                    // One second of delay while email processing in loop (BY SHIVAM)

                    if (spacing != 0) {
                        spacing = spacing * 1000;
                    }

                    Thread.Sleep(spacing);
                }

                Response.Write("Email has been sent");

                returnvalue = 1;
            }
            catch (Exception ex)
            {
                SendErrorToText(ex, ContactEmail);
                if(!ex.Message.StartsWith("Invalid Email"))
                {
                    throw ex;
                }
            }

            return returnvalue;
        }

        //[ValidateInput(false)]
        //public void sendemailcontact(string Name, string ContactEmail, string Subject, string EmailBody, string senderemail, string servername, string password, string portno, bool enablessl)
        //{
        //    string[] fName = Name.Split(' ');
        //    string Email = ContactEmail;
        //    string str = EmailBody;
        //    string s = "<p> Dear " + fName[0] + ", </p> <br/> ";
        //    str = s + str;
        //    string mailBody = Server.HtmlDecode(str);
        //    System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
        //    mailMessage.To.Clear();
        //    mailMessage.To.Add(Email);
        //    mailMessage.From = new System.Net.Mail.MailAddress(senderemail);
        //    mailMessage.Subject = Subject;\
        //    mailMessage.ReplyToList.Add(senderemail);
        //    mailMessage.Body = mailBody;
        //    mailMessage.IsBodyHtml = true;

        //    mailMessage.AlternateViews.Add(Mail_Body());
        //    mailMessage.BodyEncoding = Encoding.Default;
        //    mailMessage.Priority = MailPriority.High;

        //    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(servername);
        //    Add credentials if the SMTP server requires them.
        //    client.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
        //    System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(senderemail, password);
        //    client.UseDefaultCredentials = false;
        //    client.Credentials = SMTPUserInfo;
        //    client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
        //    client.Port = Convert.ToInt32(portno);
        //    if SSL check box is not select on Email setting on sender's Account. then use false otherwise make it true if you use SSL
        //    client.EnableSsl = Convert.ToBoolean(enablessl);//false;


        //    client.Send(mailMessage);
        //    intmailCount++;
        //}

        public JsonResult Getcount(string prospectListvalue, string BizzTYpe, string titleStandard)
        {
            int prospectList = 0;
            int intmailCount = 0;

            if (prospectListvalue != "")
            {
                prospectList = Convert.ToInt32(prospectListvalue);
            }

            if (BizzTYpe == "")
            {
                intmailCount = context.SPgetconformailwithoutbiztype(prospectList, titleStandard).Count();
            }
            else
            {
                intmailCount = context.SPgetcontactformailsend(prospectList, BizzTYpe, titleStandard).Count();
            }
            var EmailBatchSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EmailBatchSize"]);
            return Json(new
            {
                Count = intmailCount,
                BatchLimit = EmailBatchSize
            });
        }

        //    public void Sendmail()
        //    {
        //        try
        //        {
        //            MailMessage message = new MailMessage();
        //            message.To.Add("shweta.t@cisinlabs.com");// Email-ID of Receiver  
        //            message.Subject = "Test subject";// Subject of Email  
        //            message.From = new
        //System.Net.Mail.MailAddress("shweta.t@cisinlabs.com");// Email-ID of Sender  
        //            message.IsBodyHtml = true;
        //            message.AlternateViews.Add(Mail_Body());
        //            SmtpClient SmtpMail = new SmtpClient();
        //            SmtpMail.Host = "smtp.gmail.com";//name or IP-Address of Host used for SMTP transactions  
        //            SmtpMail.Port = 587;//Port for sending the mail  
        //            SmtpMail.Credentials = new
        //System.Net.NetworkCredential("shweta.t@cisinlabs.com", "chxBNsdcF3");//username/password of network, if apply  
        //            SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
        //            SmtpMail.EnableSsl = false;
        //            SmtpMail.ServicePoint.MaxIdleTime = 0;
        //            SmtpMail.ServicePoint.SetTcpKeepAlive(true, 2000, 2000);
        //            message.BodyEncoding = Encoding.Default;
        //            message.Priority = MailPriority.High;
        //            SmtpMail.Send(message); //Smtpclient to send the mail message  
        //            Response.Write("Email has been sent");
        //        }
        //        catch 
        //        { Response.Write("Failed"); }
        //    }


        //[HttpPost]
        //public JsonResult UploadFile()
        //{
        //    var fileName = Request.Files[0].FileName;
        //    var base64 = string.Empty;

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        Request.Files[0].InputStream.CopyTo(memoryStream);
        //        var fileContent = memoryStream.ToArray();
        //        base64 = Convert.ToBase64String(fileContent);
        //    }

        //    byte[] bytes = Convert.FromBase64String(base64);

        //    Image image;
        //    Random randm = new Random();
        //    String r = randm.Next(0, 999999).ToString("D6");

        //    string ImageName = "";
        //    using (MemoryStream ms = new MemoryStream(bytes))
        //    {
        //        image = Image.FromStream(ms);
        //        if (!Directory.Exists(Server.MapPath("/ImagesEmailOrignal")))
        //            Directory.CreateDirectory(Server.MapPath("/ImagesEmailOrignal"));
        //        image.Save(Server.MapPath("/ImagesEmailOrignal/" + r + ".jpg"));
        //        ImageName = r + ".jpg";
        //    }


        //    //string imgPath = Server.MapPath("/ImagesEmailOrignal/"); //"C:\\Users\\cis\\Pictures\\Saved Pictures\\";
        //    //string OutputName = r + ".jpg";
        //    //fileName = ImageName;
        //    //string CropImagePath = Server.MapPath("/ImagesEmail/");

        //    //CropImage.Imager.PerformImageResizeAndPutOnCanvas(imgPath, CropImagePath, fileName, Convert.ToInt16("250"), Convert.ToInt16("250"), OutputName);


        //    //fileName = "~/ImagesEmail/" + fileName + ".jpg";
        //    return Json(ImageName);
        //}

        [HttpPost]
        public JsonResult UploadFile()
        {
            var fileName = Request.Files[0].FileName;
            var base64 = string.Empty;

            using (var memoryStream = new MemoryStream())
            {
                Request.Files[0].InputStream.CopyTo(memoryStream);
                var fileContent = memoryStream.ToArray();
                base64 = Convert.ToBase64String(fileContent);
            }

            byte[] bytes = Convert.FromBase64String(base64);

            Image image;
            Random randm = new Random();
            String r = randm.Next(0, 999999).ToString("D6");

            string ImageName = "";

            //************ CODE FOR IMAGE CUTTER *********************

            //using (MemoryStream ms = new MemoryStream(bytes))
            //{
            //    image = Image.FromStream(ms);
            //    if (!Directory.Exists(Server.MapPath("/ImagesEmailOrignal")))
            //        Directory.CreateDirectory(Server.MapPath("/ImagesEmailOrignal"));
            //    image.Save(Server.MapPath("/ImagesEmailOrignal/" + r + ".jpg"));
            //    ImageName = r + ".jpg";
            //}

            //string path = Server.MapPath("/ImagesEmailOrignal/" + r + ".jpg");

            //ImageCutter(path, ImageName);

            // ********************* END ********************************

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
                if (!Directory.Exists(Server.MapPath("/ImagesEmail")))
                    Directory.CreateDirectory(Server.MapPath("/ImagesEmail"));
                image.Save(Server.MapPath("/ImagesEmail/" + r + ".jpg"));
                ImageName = r + ".jpg";
            }

            string path = Server.MapPath("/ImagesEmailOrignal/" + r + ".jpg");

            return Json(ImageName);
        }

        public void ImageCutter(string path, string imgname)
        {
            Image img = Image.FromFile(path);

            Bitmap newImage;
            int W = 0;
            int H = 0;
            decimal oldWidth = img.Width;
            decimal oldHeight = img.Height;
            int IdealHeightWhenWidth100px = 0;
            int IdealWidthWhenHeight100px = 0;
            string FolderNameId = string.Empty;
            int HD = 0;
            int WD = 0;

            if (oldHeight >= oldWidth)
            {
                decimal Ratio = oldWidth / oldHeight;
                decimal RatioResult = Math.Round(Ratio, 2);
                IdealWidthWhenHeight100px = Convert.ToInt32(RatioResult * 100);

                if (oldWidth == oldHeight)
                {
                    ThumbnailWidth = 50;
                }
                else
                {
                    if (IdealWidthWhenHeight100px % 2 != 0)
                    {
                        H = IdealWidthWhenHeight100px / 2;
                        ThumbnailWidth = H + 1;
                        HD = IdealWidthWhenHeight100px / 5;
                        HD = HD + 1;
                    }
                    else
                    {
                        ThumbnailWidth = IdealWidthWhenHeight100px / 2;
                        HD = IdealWidthWhenHeight100px / 5;
                    }
                }

                ThumbnailHeight = 50;

                if (oldWidth >= 320 || oldHeight >= 420)
                {
                    if (oldWidth == oldHeight)
                    {
                        ImageDetailWidth = 250;
                        ImageDetailHeight = 250;
                    }
                    else
                    {
                        ImageDetailWidth = (IdealWidthWhenHeight100px * 4) + HD;
                        ImageDetailHeight = 420;
                    }
                }
                else
                {
                    ImageDetailHeight = ProductSearchHeight;
                    ImageDetailWidth = ProductSearchWidth;
                }
            }
            else if (oldHeight < oldWidth)
            {
                decimal Ratio = oldHeight / oldWidth;
                decimal RatioResult = Math.Round(Ratio, 2);
                IdealHeightWhenWidth100px = Convert.ToInt32(RatioResult * 100);

                if (IdealHeightWhenWidth100px % 2 != 0)
                {
                    W = IdealHeightWhenWidth100px / 2;
                    ThumbnailHeight = W + 1;
                    WD = IdealHeightWhenWidth100px / 5;
                    WD = WD + 1;
                }
                else
                {
                    ThumbnailHeight = IdealHeightWhenWidth100px / 2;
                    WD = IdealHeightWhenWidth100px / 5;
                }

                ThumbnailWidth = 50;

                if (oldWidth > 320 || oldHeight > 420)
                {
                    ImageDetailWidth = 320;
                    ImageDetailHeight = (IdealHeightWhenWidth100px * 3) + WD;
                }
                else
                {
                    ImageDetailHeight = ProductSearchHeight;
                    ImageDetailWidth = ProductSearchWidth;
                }
            }
            //}
            //else {
            //    ImageDetailWidth = Convert.ToInt32(oldWidth);
            //    ImageDetailHeight = Convert.ToInt32(oldHeight);
            //}

            string pathhover = System.Web.HttpContext.Current.Server.MapPath("~");

            if (!System.IO.Directory.Exists(pathhover))
            {
                System.IO.Directory.CreateDirectory(pathhover);
            }

            SaveCutterimage(ImageDetailWidth, ImageDetailHeight, pathhover, "//ImagesEmail//", img, imgname);

            img.Dispose();
        }

        public void SaveCutterimage(int width, int height, string rootpath, string FolderName, Image Img, string Imgname)
        {
            using (Bitmap newImage = new Bitmap(width, height))
            {
                rootpath = rootpath + FolderName;

                if (!System.IO.Directory.Exists(rootpath))
                {
                    System.IO.Directory.CreateDirectory(rootpath);
                }

                Graphics g = Graphics.FromImage(newImage);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                Rectangle rect = new Rectangle(0, 0, width, height);
                g.DrawImage(Img, rect);
                newImage.Save(rootpath + Imgname, ImageFormat.Jpeg);
            }

        }

        private AlternateView Mail_Body(string RndmImageName, string mailBody)
        {

            AlternateView AV;
            try
            {
                string s = Request.Form["hiddenImageName"].ToString();

                if (RndmImageName != "")
                {
                    //string path = Server.MapPath(@"~/ImagesEmail/067047.jpg");
                    //string path = Server.MapPath(@"Images/photo.jpg");
                    string path = Server.MapPath(@"~/ImagesEmail/" + RndmImageName);

                    LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
                    Img.ContentId = "MyImage";
                    string str = @"  
                <table>  
                    <tr>  
                        <td> " + mailBody + @" 
                        </td>  
                    </tr>  
                    <tr>  
                        <td>  
                          <img src=cid:MyImage  id='img' alt=''/>   
                        </td>  
                    </tr></table>  
                ";
                    str = str.TrimEnd('\'').TrimStart('\'');
                    AV =
                   AlternateView.CreateAlternateViewFromString(str, null, MediaTypeNames.Text.Html);
                    AV.LinkedResources.Add(Img);
                    return AV;
                }
                else
                {
                    string str = @"  
                    <table>  
                        <tr>  
                            <td> " + mailBody + @" 
                            </td>  
                        </tr>  
                       </table>  
                    ";
                    str = str.TrimEnd('\'').TrimStart('\'');
                    AV = AlternateView.CreateAlternateViewFromString(str, null, MediaTypeNames.Text.Html);
                    //AV.LinkedResources.Add(Img);

                    return AV;
                }
            }
            catch (Exception ex)
            {
                string str = "";

                AV = AlternateView.CreateAlternateViewFromString(str, null, MediaTypeNames.Text.Html);
                SendErrorToText(ex, "");

                return AV;
            }
        }

        public void SendErrorToText(Exception ex, string emailAddress)
        {
            try
            {
                var line = Environment.NewLine + Environment.NewLine;

                ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
                Errormsg = ex.GetType().Name.ToString();
                extype = ex.GetType().ToString();
                exurl = System.Web.HttpContext.Current.Request.Url.ToString();
                ErrorLocation = ex.Message.ToString();

                string filepath = System.Web.HttpContext.Current.Server.MapPath("~/ExceptionDetailsFile/");  //Text File Path

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name

                if (!System.IO.File.Exists(filepath))
                {
                    System.IO.File.Create(filepath).Dispose();
                }

                using (StreamWriter sw = System.IO.File.AppendText(filepath))
                {
                    string error = "Email Address:" + " " + emailAddress + line 
                        + "Log Written Date:" + " " + DateTime.Now.ToString() + line
                        + "StackTrace:" + " " + ex.StackTrace + line 
                        + "Full Exception:" + " " + ex.ToString() + line 
                        + "Error Line No :" + " " + ErrorlineNo + line 
                        + "Error Message:" + " " + Errormsg + line 
                        + "Exception Type:" + " " + extype + line 
                        + "Error Location :" + " " + ErrorLocation + line 
                        + "Error Page Url:" + " " + exurl + line 
                        + "User Host IP:" + " " + hostIp + line;

                    sw.WriteLine("---Exception Details for Date:" + " " + DateTime.Now.ToString() + "---");
                    sw.WriteLine("---------------------------------------START---------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine(line);
                    sw.WriteLine("----------------------------------------END----------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        // Applied by omkar to validate the batch count
        public bool isInteger(double n)
        {
            return n - (Int64)n == 0;
        }

        // omkar
        public List<EmailBatch> BatchFunction(int intmailCount)
        {
            int PerbatchCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Emailbatchcount"]);//= 20;   // One batch count 
            double Totalbatch = (double)intmailCount / PerbatchCount;
            int TotalbatchCount = (int)Totalbatch;

            bool IsbatchInt = isInteger(Totalbatch);

            if (!IsbatchInt)
            {
                TotalbatchCount = (int)Totalbatch + 1;
            }

            for (int i = 0; i < TotalbatchCount; i++)
            {
                EmailBatch objEmailBatch = new EmailBatch();

                if (i == Convert.ToInt32(TotalbatchCount - 1))  // When last batch is selected
                {
                    if (!IsbatchInt)  // Case when total count was in decimal
                    {
                        objEmailBatch.BatchName = "Batch-" + (i + 1);
                        objEmailBatch.BatchStartingCount = PerbatchCount * (i);
                        objEmailBatch.BatchEndingCount = intmailCount;
                    }
                    else
                    {
                        objEmailBatch.BatchName = "Batch-" + (i + 1);
                        objEmailBatch.BatchStartingCount = PerbatchCount * (i);
                        objEmailBatch.BatchEndingCount = intmailCount;
                    }

                }
                else  // Any middle batch is selected not the last one
                {
                    objEmailBatch.BatchName = "Batch-" + (i + 1);
                    objEmailBatch.BatchStartingCount = PerbatchCount * i;
                    objEmailBatch.BatchEndingCount = objEmailBatch.BatchStartingCount + PerbatchCount;
                }

                BatchValuesList.Add(objEmailBatch);
            }

            return BatchValuesList;
        }
    }

    public class EmailBatch
    {
        public string BatchName { get; set; }
        public int BatchStartingCount { get; set; }
        public int BatchEndingCount { get; set; }
    }
}