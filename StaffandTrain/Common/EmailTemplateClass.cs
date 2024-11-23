using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace StaffandTrain.Utility
{
    public class EmailTemplateClass
    {
        public static string GetEmailBody(string TemplateType)
        {
            string body = "";

            switch (TemplateType)
            {
                case "Registration":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/Registration.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;

                case "NewConnectionRequest":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/NewConnectionRequest.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;

                case "InvitationReminder":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/InvitationReminder.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;

                case "EmailToIntrasoldCompanyLHS":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/EmailToIntrasoldCompanyLHS.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;

                case "EmailToIntrasoldCompanyOther":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/EmailToIntrasoldCompanyOther.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;
                case "EmailToIntrasoldCompanyRHS":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/EmailToIntrasoldCompanyRHS.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;


                case "PrivateMessage":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/PrivateMessage.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;

                case "MassMessage":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/MassMessage.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;

                case "Notification":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/Notification.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;

                case "ResetPassword":
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/ResetPassword.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    break;

            }

            return body;
        }
    }
}