using StaffandTrain.Common;
using StaffandTrain.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StaffandTrain.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.Identity;
using ClosedXML.Excel;
using System.Configuration;

namespace StaffandTrain.Controllers
{
    [NoCache]
    [Authorize(Roles = "Admin,Recruiter")]
    public class WorkerController : Controller
    {
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();

        public ActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.message = TempData["Message"];
            }
            var WorkerList = context.Workers.ToList();
            WorkerList.ForEach(p => p.CheckIn += TimeSpan.FromHours(getTimeZoneHours()));
            ViewBag.ServerTime = DateTime.Now.AddHours(getTimeZoneHours()).ToString("hh:mm:ss tt");
            return View(WorkerList);
        }
        public ActionResult SaveWorker(int? Id)
        {
            WorkerModel objuser = new WorkerModel();
            if (Id != null)
            {
                var worker = context.Workers.FirstOrDefault(e => e.Id == Id);

                if (worker != null)
                {
                    objuser.Id = worker.Id;
                    objuser.Name = worker.Name;
                    objuser.Email = worker.Email;
                    objuser.OldPassword = worker.Password;
                    objuser.CheckIn = worker.CheckIn;
                    objuser.CreateDate = worker.CreateDate;
                }
                objuser.CheckIn += TimeSpan.FromHours(getTimeZoneHours());
            }
            if (TempData["Message"] != null)
            {
                ViewBag.message = TempData["Message"];
            }
            ViewBag.ServerTime = DateTime.Now.AddHours(getTimeZoneHours()).ToString("hh:mm:ss tt");
            return View(objuser);
        }
        [HttpPost]
        public ActionResult SaveWorkerData(WorkerModel obj)
        {
            try
            {
                // Validate Email Duplication
                var worker = context.Workers.FirstOrDefault(e => e.Email == obj.Email);
                if (worker != null && worker.Id != obj.Id)
                {
                    throw new Exception("This worker email already registered!");
                }

                // Encrypt Password
                string password= string.Empty;
                DateTime createDate;
                DateTime modifyDate;
                if (obj.Id > 0)
                {
                    if (obj.Password != null)
                    {
                        password = CryptorEngine.HashPassword(obj.Password);
                    } else
                    {
                        password = obj.OldPassword;
                    }
                    createDate = obj.CreateDate;
                    modifyDate = DateTime.Now;
                    TempData["Message"] = "Worker Updated";

                } else
                {
                    password = CryptorEngine.HashPassword(obj.Password);
                    createDate = DateTime.Now;
                    modifyDate = DateTime.Now;
                    TempData["Message"] = "Worker Saved";
                    
                }
                obj.CheckIn -= TimeSpan.FromHours(getTimeZoneHours());
                context.SPInsertOrUpdateWorker(obj.Id, obj.Name, obj.Email, password, obj.CheckIn, createDate, modifyDate);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Some Error Occured: " + ex.Message;
                return RedirectToAction("SaveWorker", obj);
            }
        }
        public ActionResult DeleteWorker(int Id)
        {
            var worker = context.Workers.FirstOrDefault(e => e.Id == Id);

            if (worker != null)
            {
                context.SPDeleteWorker(Id);
                context.SaveChanges();
                TempData["Message"] = "Worker Deleted";
            }
            return RedirectToAction("index");
        }

        private int getTimeZoneHours()
        {
           var timeZoneHours =  int.Parse(ConfigurationManager.AppSettings["TimeZoneHours"]);
            return timeZoneHours;
        }
    }
}