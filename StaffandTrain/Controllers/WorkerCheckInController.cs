using StaffandTrain.Common;
using StaffandTrain.DataModel;
using StaffandTrain.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StaffandTrain.Controllers
{
    public class WorkerCheckInController : Controller
    {
        SATConn context = new SATConn();
        public ActionResult Index()
        {
            return HttpNotFound();
            if (TempData["Message"] != null)
            {
                ViewBag.message = TempData["Message"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult CheckInWorker(WorkerCheckInModel obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email) || string.IsNullOrEmpty(obj.Password)) throw new Exception("Invalid Data!");

                var worker = context.Workers.FirstOrDefault(e => e.Email == obj.Email);
                if (worker == null) throw new Exception("This worker not exist!");

                bool verifyPassword = CryptorEngine.VerifyPassword(obj.Password, worker.Password);
                if (verifyPassword == false) throw new Exception("Invalid Password!");

                var currentDate = DateTime.Now;
                var workerLog = context.WorkersLogs.FirstOrDefault(e => e.WorkerId == worker.Id && DbFunctions.TruncateTime(e.CreateDate) == currentDate.Date);
                if (workerLog != null) throw new Exception("You are already Checked In!");

                context.SPInsertOrUpdateWorkerLog(0, worker.Id, worker.Name + " Worker Logged in At " + DateTime.Now, "LogIn", DateTime.Now);
                context.SaveChanges();
                SendEmail mail = new SendEmail();
                mail.SendSMTPEmail(worker.Email, "Good Morning App - Logged In", "Hello " + worker.Name + ", you have logged in successfully At " + DateTime.Now.AddHours(getTimeZoneHours()).ToString("hh:mm:ss tt"));

                //TempData["Message"] = "Checked In Successfully!";
                //return RedirectToAction("Index");
                var url = "https://nearshore-staffing.com/worker-check-in/?s=1&message=" + Uri.EscapeDataString("Checked In Successfully!");
                return Redirect(url);
            }
            catch (Exception ex)
            {
                var url = "https://nearshore-staffing.com/worker-check-in/?s=0&message=" + Uri.EscapeDataString(ex.Message);
                return Redirect(url);
                //TempData["Message"] = ex.Message;
                //return RedirectToAction("Index");
            }
        }

        private int getTimeZoneHours()
        {
            var timeZoneHours = int.Parse(ConfigurationManager.AppSettings["TimeZoneHours"]);
            return timeZoneHours;
        }
    }

}