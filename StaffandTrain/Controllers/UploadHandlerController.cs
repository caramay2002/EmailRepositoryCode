using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace StaffandTrain.Controllers
{
    public class UploadHandlerController : Controller
    {
        [HttpPost]
        public ActionResult UploadFile()
        {
            try
            {
                HttpPostedFileBase upload = Request.Files["upload"]; // Use "upload" as the input field name in your HTML form

                if (upload != null && upload.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(upload.FileName);
                    string uniqueFileName = GenerateUniqueFileName(fileName); // Generate a unique file name
                    string filePath = Path.Combine(Server.MapPath("~/UploadedFiles"), uniqueFileName);
                    upload.SaveAs(filePath);

                    string fileUrl = Url.Content("~/UploadedFiles/" + uniqueFileName);

                    Uri uri = new Uri(Request.Url.ToString());
                    string domainWithPort = uri.GetLeftPart(UriPartial.Authority);

                    // Return a JSON response object
                    return Json(new
                    {
                        uploaded = 1,
                        fileName = uniqueFileName,
                        url = ConfigurationManager.AppSettings["ImageProxyUrl"] + uniqueFileName
                    });
                }
                else
                {
                    return Json(new
                    {
                        uploaded = 0,
                        error = new { message = "No file uploaded." }
                    });
                }
            }
            catch (Exception ex)
            {
                var common = new Common.Common();
                common.ErrorExceptionLogingByService(ex.ToString(), "UploadFile", "UploadHandlerController", "NA", "NA", "NA", "WEB");
                return Json(new
                {
                    uploaded = 0,
                    error = new { message = ex.Message }
                });
            }
        }
        private string GenerateUniqueFileName(string fileName)
        {
            // Generate a unique file name based on the current timestamp and original file name
            string uniqueFileName = $"{DateTime.Now.Ticks}_{fileName}";
            return uniqueFileName;
        }
    }
}