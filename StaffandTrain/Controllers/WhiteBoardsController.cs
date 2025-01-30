using StaffandTrain.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StaffandTrain.Models;
using StaffandTrain.Common;
using System.Data.SqlClient;

namespace StaffandTrain.Controllers
{
    //[NoCache]
    //[Authorize(Roles = "Admin,Recruiter")]
    public class WhiteBoardsController : Controller
    {
        Common.Common cm = new Common.Common();
        SATConn context = new SATConn();
        // GET: WhiteBoards
        public ActionResult Index()
        {
            var WhiteboardList = new List<WhiteBoardName>();
            try
            {
                WhiteboardList = context.WhiteBoardNames.Where(p => p.AllowRecruiter != true).ToList();
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "WhiteBoards" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
            }
            return View(WhiteboardList);
        }

        public ActionResult RecruiterWB()
        {
            var WhiteboardList = new List<WhiteBoardName>();
            try
            {
                WhiteboardList = context.WhiteBoardNames.Where(p => p.AllowRecruiter == true).ToList();
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "WhiteBoards" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
            }
            return View(WhiteboardList);
        }

        public ActionResult SaveWhiteboard()
        {
            try
            {
                var Wbid = Request.Form["hdnwbid"];
                var Wbname = Request.Form["txtwbname"];
                context.SpupdatewhiteBoard(Wbname, Convert.ToInt32(Wbid));
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "WhiteBoards" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "SaveWhiteboard", "NA", "NA", "NA", "WEB");
            }
            return (RedirectToAction("Index"));

        }

        public ActionResult WhiteBoardDetails(string wbid)
        {
            var Jobdetails = new List<ManageJobs>();
            try
            {
                if (!string.IsNullOrEmpty(wbid))
                {
                    int wbiddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(wbid))));
                    string query = "[dbo].[SpGetJobdetailsbyWbid] @WhiteboardID = " + wbiddecrypt;
                    Jobdetails = context.Database.SqlQuery<ManageJobs>(query).ToList();
                    if (Jobdetails.Count() > 0)
                    {
                        ViewBag.wbname = "";
                    }
                    ViewBag.wbid = wbiddecrypt;

                    if (TempData["Message"] != null)
                    {
                        ViewBag.Message = TempData["Message"];
                    }
                }

            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "WhiteBoards" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "WhiteBoardDetails", "NA", "NA", "NA", "WEB");
            }
            return View(Jobdetails);

        }

        public ActionResult EditUpdateJobs(string jobid, string wbid)
        {
            ManageJobs objjobsdetails = new Models.ManageJobs();
            int wbiddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(wbid))));
            objjobsdetails.WhiteboardID = wbiddecrypt;
            if (!string.IsNullOrEmpty(jobid))
            {
                int jobiddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(jobid))));
                //var jobdetails = context.spgetjobdetailbyjobid(jobiddecrypt).FirstOrDefault();
                string query = "[dbo].[spgetjobdetailbyjobid] @jobid = " + jobiddecrypt;
                objjobsdetails = context.Database.SqlQuery<ManageJobs>(query).FirstOrDefault();
                objjobsdetails.jobiddecypt = objjobsdetails.jobid;
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];

            }
            return View(objjobsdetails);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveJobs(ManageJobs objjobsdetails)
        {
            try
            {
                SaveJobInDatabase(objjobsdetails);
                if (objjobsdetails.jobiddecypt == 0)
                {
                    TempData["Message"] = "Job Created";
                }
                else
                {
                    TempData["Message"] = "Job Updated";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Some error occured";
                cm.ErrorExceptionLogingByService(ex.ToString(), "WhiteBoards" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "WhiteBoardDetails", "NA", "NA", "NA", "WEB");
            }

            return RedirectToAction("WhiteBoardDetails", new 
            {
                @wbid = @cm.Code_Encrypt(CryptorEngine.Encrypt(objjobsdetails.WhiteboardID.ToString()))
                //@jobid = @cm.Code_Encrypt(CryptorEngine.Encrypt(objjobsdetails.jobiddecypt.ToString()))
            });
            //   return RedirectToAction("Index");

        }
        public ActionResult DeleteJob(string jobid, string wbid)
        {
            try
            {
                int wbiddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(wbid))));
                int jobiddecrypt = Convert.ToInt32(CryptorEngine.Decrypt(cm.Code_Decrypt(Convert.ToString(jobid))));
                context.spdeletejob(jobiddecrypt);
                context.SaveChanges();
                TempData["Message"] = "Record Deleted";
                return RedirectToAction("WhiteBoardDetails", new { @wbid = @cm.Code_Encrypt(CryptorEngine.Encrypt(wbiddecrypt.ToString())) });
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Some error occured";
                cm.ErrorExceptionLogingByService(ex.ToString(), "WhiteBoards" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "DeleteJob", "NA", "NA", "NA", "WEB");
            }


            return View();
        }

        private void SaveJobInDatabase(ManageJobs job)
        {
            var rowNumber = job.RowNumber.HasValue ? job.RowNumber.ToString() : "NULL";
            job.submittals = (job.submittals + "").Replace("'", "''");
            string query;
            if (job.jobiddecypt == 0)
            {
                query = string.Format("[dbo].[SpcreatenewJob] @jobtitle = NULL, @jobdescr = NULL, @WhiteboardID = {0}, @RowNumber = {1}, @submittals='{2}'", job.WhiteboardID, rowNumber, job.submittals);
            }
            else
            {
                query = string.Format("[dbo].[Spupdatejob] @jobtitle = NULL, @jobdescr = NULL, @jobid = {0}, @RowNumber = {1}, @submittals='{2}'", job.jobiddecypt, rowNumber, job.submittals);
            }
            context.Database.ExecuteSqlCommand(query);
            if (job.jobiddecypt == 0) 
            {
                var objjobsdetails = context.Database.SqlQuery<ManageJobs>("[dbo].[spgetlastjobdetail]").FirstOrDefault();
                job.jobiddecypt = objjobsdetails.jobid;
            }
                
        }
    }
}