using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net.Mail;

using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using StaffandTrain.DataModel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Data.Entity;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Text;
using System.Threading;
using System.Web.Services.Description;

/// <summary>
/// Summary description for SendEmail
/// </summary>
namespace StaffandTrain.Common
{
    public class SendEmail
    {
        SATConn context = new SATConn();

        private readonly SmtpClient _smtpClient;
        //---==== SendEmail Constructor ====----
        public SendEmail(string sectionName = "default")
        {
            SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("mailSettings/" + sectionName);

            _smtpClient = new SmtpClient();

            if (section != null)
            {
                if (section.Network != null)
                {
                    _smtpClient.Host = section.Network.Host;
                    _smtpClient.Port = section.Network.Port;
                    _smtpClient.UseDefaultCredentials = section.Network.DefaultCredentials;
                    _smtpClient.Credentials = new NetworkCredential(section.Network.UserName, section.Network.Password, section.Network.ClientDomain);
                    _smtpClient.EnableSsl = false;
                    if (section.Network.TargetName != null)
                        _smtpClient.TargetName = section.Network.TargetName;
                }

                _smtpClient.DeliveryMethod = section.DeliveryMethod;
                if (section.SpecifiedPickupDirectory != null && section.SpecifiedPickupDirectory.PickupDirectoryLocation != null)
                    _smtpClient.PickupDirectoryLocation = section.SpecifiedPickupDirectory.PickupDirectoryLocation;
            }
        }


        public bool SendToEmail(string Subject, string Message, string EmailTo)
        {
            bool emailResult = false;
            string emailSubject = Subject;

            string emailtemplate = Message;
            try
            {
                var HostName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                emailtemplate = emailtemplate.Replace("[HostName]", HostName);
                emailtemplate = emailtemplate.Replace("[TodayDate]", DateTime.Now.ToString("dd MMM yyyy"));
                emailtemplate = emailtemplate.Replace("[CurrentTime]", DateTime.Now.ToString("HH:mm:ss tt"));

                // Replace Token by Dictionary key and values

                MailMessage oEmail = new MailMessage();
                oEmail.From = new MailAddress("dj@nearshore-staffing.com");
                oEmail.Subject = emailSubject;
                oEmail.Body = emailtemplate;
                oEmail.IsBodyHtml = true;
                oEmail.To.Add(EmailTo); //-- add To email

    //            ServicePointManager.ServerCertificateValidationCallback =
    //delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
    //         X509Chain chain, SslPolicyErrors sslPolicyErrors)
    //{ return true; };

                _smtpClient.Send(oEmail);
                emailResult = true;
            }
            catch (Exception ex)
            {
               // Common cm = new Common();
               // cm.ErrorExceptionLogingByService(ex.ToString(), "ForgetPassword" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "ForgetPassword", "NA", "NA", "NA", "WEB");
                //CPPLException objExc = new CPPLException();
                //objExc.CPPLErrorExceptionLogingByService(ex.ToString(), GetType().Name + ":" + new StackTrace().GetFrame(0).GetMethod().Name, GetType().Name, "", emailType, emailtemplate);
                //emailResult = false;
            }
            return emailResult;
        }

        public void SendSMTPEmail(string recipientEmail, string subject, string body)
        {
            //recipientEmail = "dev@talkboxsolutions.com";
            // Email parameters
            if (!bool.Parse(ConfigurationManager.AppSettings["ActiveEmailSender"])) return;

            string senderEmail = ConfigurationManager.AppSettings["EmailSender"];
            string senderPassword = ConfigurationManager.AppSettings["EmailSenderPassword"];
            string senderDisplayName = ConfigurationManager.AppSettings["EmailSenderDisplayName"];

            // SMTP server details
            string smtpHost = ConfigurationManager.AppSettings["EmailSMTPHost"];
            int smtpPort = int.Parse(ConfigurationManager.AppSettings["EmailSMTPPort"]);
            bool enableSsl = bool.Parse(ConfigurationManager.AppSettings["EmailEnableSSL"]);

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true;

                MailMessage message = new MailMessage();

                //message.To.Add("info.usamaali@gmail.com");
                message.To.Add(recipientEmail);
                //message.To.Add("dev@talkboxsolutions.com");
                message.Subject = subject;// addfnamesubject;
                message.From = new System.Net.Mail.MailAddress(senderEmail, senderDisplayName);

                var plainText = AlternateView.CreateAlternateViewFromString("This is the plain text version of the email.", null, "text/plain");
                var htmlBody = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                message.AlternateViews.Add(plainText);
                message.AlternateViews.Add(htmlBody);

                message.IsBodyHtml = true;

                message.Headers.Add("Message-ID", $"<{Guid.NewGuid()}@nearshore-usa.com>");
                message.Headers.Add("X-Mailer", "Near Shore SMTP v1.0");
                using (SmtpClient SmtpMail = new SmtpClient())
                {
                    SmtpMail.Timeout = 30000; // Increase the timeout to 30 seconds
                    SmtpMail.UseDefaultCredentials = false;
                    SmtpMail.Host = smtpHost;
                    SmtpMail.Port = Convert.ToInt32(smtpPort);//Port for sending the mail  
                    SmtpMail.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                    SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                    SmtpMail.EnableSsl = true;// Convert.ToBoolean(enableSsl);
                    SmtpMail.ServicePoint.MaxIdleTime = 10000;
                    SmtpMail.ServicePoint.SetTcpKeepAlive(true, 12000, 12000);
                    message.BodyEncoding = Encoding.Default;

                    SmtpMail.Send(message); //Smtpclient to send the mail message  

                }


            }
            catch (Exception ex)
            {
               context.SPInsertOrUpdateLog(0, "Error", "SendEmail",  string.Format("Message: {0} \nStack Trace: {1}\nInner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException), null);
                Console.WriteLine("Error sending email: " + ex.Message);
            }
       
        }
    
        public void ScheduleWorkerEmail()
        {
            //context.SPInsertOrUpdateLog(0, "Success", "Email Scheduler", "Scheduler. run at: " + DateTime.Now, null);
            // Email those workers which 15 minutes are remaining to login
            int beforeMin = int.Parse(ConfigurationManager.AppSettings["EmailWorkerBeforeCheckIn"]);
            DateTime currentTime = DateTime.Now;
            DateTime endTime = currentTime.AddMinutes(beforeMin);
            
            var workers = context.Workers
                .Where(e => e.CheckIn.Hours == endTime.Hour && e.CheckIn.Minutes == endTime.Minute)
                .ToList();
            if (workers.Count() > 0)
            {
                var currentDate = DateTime.Now;
                foreach (var worker in workers)
                {
                    var workerLog = context.WorkersLogs.FirstOrDefault(e => e.WorkerId == worker.Id && DbFunctions.TruncateTime(e.CreateDate) == currentDate.Date);
                    if (workerLog == null)
                    {
                        var subject = "Good Morning App - Check In Reminder";
                        var body = "Hello " + worker.Name + ", \n\n" + beforeMin + " minutes left, please check in before your designated time: " + getCheckInTime(worker.CheckIn) + "\n\nNearshore Staffing ";
                        SendSMTPEmail(worker.Email, subject, body);
                        context.SPInsertOrUpdateLog(0, "Success", "Email Scheduler", "Worker inform to login scheduler. run at: " + DateTime.Now, null);
                    }
                }
            }



            // Email those workers which are not login
            string adminEmails = ConfigurationManager.AppSettings["LateNotificationAdminEmails"];
            int min = int.Parse(ConfigurationManager.AppSettings["EmailWorkerNotCheckIn"]);
            DateTime currentTime1= currentTime.AddMinutes(min * -1);

            var workers1 = context.Workers
                .Where(e => e.CheckIn.Hours == currentTime1.Hour && e.CheckIn.Minutes == currentTime1.Minute)
                .ToList();

            if (workers1.Count() > 0)
            {
                var currentDate = DateTime.Now;
                var workersName = "";
                foreach (var worker in workers1)
                {
                    if (workersName != "") workersName += ", ";
                    workersName += worker.Name;

                    var workerLog = context.WorkersLogs.FirstOrDefault(e => e.WorkerId == worker.Id && DbFunctions.TruncateTime(e.CreateDate) == currentDate.Date);
                    if (workerLog == null)
                    {
                       context.SPInsertOrUpdateLog(0, "Success", "Email Scheduler", "Worker inform to expired login. scheduler run at: " + DateTime.Now, null);

                        var subject = "Good Morning App - Late Reminder";
                        var body = "Hello " + worker.Name + ", \n\nYour designated check in time: " + getCheckInTime(worker.CheckIn) + " has passed and you were unable to check in" + "\n\nNearshore Staffing ";
                        SendSMTPEmail(worker.Email, subject, body);

                        var adminsubject = "Good Morning App - Late Worker Notification";
                        var adminbody = "Hello Admins,\n\n The worker " + worker.Name + " is not able to check in at designated time: " + getCheckInTime(worker.CheckIn) + "\n\nNearshore Staffing ";
                        SendSMTPEmail(adminEmails, adminsubject, adminbody);
                    }
                }
            }
        }

        private string getCheckInTime(TimeSpan checkInTime) 
        {
            var timeZoneHours = int.Parse(ConfigurationManager.AppSettings["TimeZoneHours"]);
            return DateTime.Today.Add(checkInTime).AddHours(timeZoneHours).ToString("hh:mm:ss tt");
        }
    }

}