using ClosedXML.Excel;
using StaffandTrain.Common;
using StaffandTrain.DataModel;
using StaffandTrain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace StaffandTrain.Controllers
{
    [NoCache]
    [Authorize(Roles ="Client")]
    public class ProspectViewListClientController : Controller
    {
        SATConn context = new SATConn();
        Common.Common cm = new Common.Common();
        // GET: ProspectViewListClient
        public ActionResult Index(int listid, string lstname)
        {
            try
            {
                if (listid != 0)
                {
                    if (TempData["Message"] != null)
                    {
                        ViewBag.message = TempData["Message"];
                    }

                    ViewBag.listid = listid;
                    var listname = context.Prospecting_Lists.Where(x => x.listid == listid).FirstOrDefault().listname;
                    ViewBag.listname = listname;
                    var citycirclelist = context.SPgetCitycirclelist(listid).Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
                    var Biztypelist = context.SPgetbiztype(listid).Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
                    var Titlelist = context.SPgettitle().Select(xx => new SelectListItem { Value = xx.ToString(), Text = xx.ToString() }).ToList();
                    ViewData["citycirclelist"] = citycirclelist;
                    ViewData["Biztypelist"] = Biztypelist;
                    ViewData["Titlelist"] = Titlelist;
                }
                else
                {
                    return RedirectToAction("Index", "ProspectListsClient");
                }
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewListClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "Index", "NA", "NA", "NA", "WEB");
            }
            return View();
        }


        public ActionResult GetCompanyList(int listid, string citycircle, string biztype, string title, string name, string Notes)
        {
            var prospectviewlist = new List<SpGetprosviewlistclient_Result>();
            var complist = new List<CompanyModel>();
            try
            {
                List<string> titlelist = new List<string>();
                Notes = Notes.Replace(" ", "");
                if (!string.IsNullOrEmpty(title))
                {
                    if (title == "Manager")
                    {
                        titlelist.Add("CEO");
                        titlelist.Add("EXEC");
                        titlelist.Add("Manager");
                    }
                    else
                    {
                        titlelist.Add(title);
                    }
                }
                if (listid != 0 && string.IsNullOrEmpty(citycircle) && string.IsNullOrEmpty(biztype) && string.IsNullOrEmpty(title) && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(Notes))
                {
                    var data = context.SpGetprosviewlistclient(listid).ToList();
                    prospectviewlist = data;
                }
                else if (listid != 0 || !string.IsNullOrEmpty(citycircle) || !string.IsNullOrEmpty(biztype) || !string.IsNullOrEmpty(title))
                {
                    

                    var data = (from com in context.Companies
                                join con in context.Contacts on com.companyid equals con.companyid into gj
                                from x in gj.DefaultIfEmpty()
                                where
                                  ((citycircle == null || citycircle == "") || com.citycircle == citycircle) &&
                  ((biztype == null || biztype == "") || com.biztype == biztype) &&
                  ((title == null || title == "") || (titlelist.Contains(x.titlestandard) || x.titlestandard == null)) &&
                  ((name == null || name == "") || x.contactfullname.Contains(name) || com.name.Contains(name)) &&
                   ((Notes == null || Notes == "") || x.combinednotes.Trim().Replace("\r", "").Replace("\n", "").Replace(System.Environment.NewLine, "").Replace(" ", "").Contains(Notes))
                   && com.listid == listid
                                select new
                                {
                                    com.companyid,
                                    com.listid,
                                    com.citycircle,
                                    com.biztype,
                                    com.name,
                                    com.addr1,
                                    com.addr2,
                                    com.city,
                                    com.state,
                                    com.zip,
                                    com.weburl,
                                    com.phone,
                                    com.priority,
                                    com.target,
                                    x.combinednotes,
                                    com.Userid
                                }).Distinct().ToList();

                    foreach (var i in data)
                    {
                        CompanyModel objcomp = new CompanyModel();
                        objcomp.companyid = i.companyid;
                        objcomp.listid = i.listid;
                        objcomp.citycircle = i.citycircle;
                        objcomp.biztype = i.biztype;
                        objcomp.name = i.name;
                        objcomp.addr1 = i.addr1;
                        objcomp.addr2 = i.addr2;
                        objcomp.city = i.city;
                        objcomp.state = i.state;
                        objcomp.zip = i.zip;
                        objcomp.weburl = i.weburl;
                        objcomp.phone = i.phone;
                        objcomp.Userid = i.Userid;
                        if (i.priority == null) 
                        {
                            objcomp.priority = false;
                        }
                        else
                        {
                            objcomp.priority = Convert.ToBoolean(i.priority);

                        }
                        if (i.target == null)
                        {
                            objcomp.priority = false;
                        }
                        else
                        {
                            objcomp.target = Convert.ToBoolean(i.target);

                        }
                        objcomp.combinednotes = i.combinednotes;
                        if (complist.Where(x => x.companyid == objcomp.companyid).Count() == 0)
                        {
                            complist.Add(objcomp);
                        }
                    }
                    return View(complist);
                    //return RedirectToAction("Index", "ProspectListsAdmin");
                }


            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewListClient" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "GetCompanyList", "NA", "NA", "NA", "WEB");

            }
            return View(prospectviewlist);
        }

        public ActionResult SaveCompany(CompanyModel objcomp)
        {
            try
            {

                if (Request.QueryString["compid"] != null)
                {
                    objcomp.listid = Convert.ToInt32(Request.QueryString["compid"]);
                }
                if (Request.QueryString["companyidedit"] != null)
                {
                    objcomp.companyid = Convert.ToInt32(Request.QueryString["companyidedit"]);
                    var data = context.SPgetCompdetailsbycompid(objcomp.companyid).FirstOrDefault();
                    if (data != null)
                    {
                        objcomp.name = data.name;
                        objcomp.biztype = data.biztype;
                        objcomp.addr2 = data.addr2;
                        objcomp.addr1 = data.addr1;
                        objcomp.weburl = data.weburl;
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
                    }
                }
                var listname = context.Prospecting_Lists.Where(x => x.listid == objcomp.listid).FirstOrDefault().listname;
                objcomp.listname = listname;
                ViewData["city"] = context.Companies.Where(X => X.listid == objcomp.listid).Select(xx => new SelectListItem { Value = xx.citycircle, Text = xx.citycircle }).Distinct();
            }
            catch (Exception ex)
            {
                cm.ErrorExceptionLogingByService(ex.ToString(), "ProspectViewList" + ":" + new StackTrace().GetFrame(0).GetMethod().Name, "SaveCompany", "NA", "NA", "NA", "WEB");
            }
            return View(objcomp);
        }
        [HttpPost]
        public ActionResult EditUpdateCompany(CompanyModel objcomp)
        {
            try
            {
                if (objcomp.companyid == 0)
                {//SPinsertCompanyClient
                    context.SPinsertCompanyClient(objcomp.citycircle, objcomp.biztype, objcomp.name, objcomp.addr1, objcomp.addr2, objcomp.city, objcomp.state, objcomp.zip, objcomp.weburl, objcomp.phone, objcomp.priority, objcomp.target, objcomp.combinednotes, objcomp.adminnotes, objcomp.notes, objcomp.listid,User.Identity.Name);
                    context.SaveChanges();
                    TempData["Message"] = "Record Saved";
                }
                if (objcomp.companyid != 0)
                {
                    context.SPupdateCompanies(objcomp.citycircle, objcomp.biztype, objcomp.name, objcomp.addr1, objcomp.addr2, objcomp.city, objcomp.state, objcomp.zip, objcomp.weburl, objcomp.phone, objcomp.priority, objcomp.target, objcomp.combinednotes, objcomp.adminnotes, objcomp.notes, objcomp.companyid, objcomp.listid);
                    context.SaveChanges();
                    TempData["Message"] = "Record Updated";

                }
                var listname = context.Prospecting_Lists.Where(x => x.listid == objcomp.listid).FirstOrDefault().listname;
                objcomp.listname = listname;
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Some Error Occured";
            }
            return RedirectToAction("Index", new { @listid = objcomp.listid, @lstname = objcomp.listname });
        }

        public ActionResult exporttoexcel(int listid)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var datalist = context.SpGetdataforexcelexport(listid).ToList();
                var dt = ToDataTable(datalist);
                if (dt.Rows.Count > 0)
                {
                    wb.Worksheets.Add(dt);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    Random rdn = new Random();
                    string FileName = "";
                    if (datalist.FirstOrDefault().City_Circle != null && datalist.FirstOrDefault().City_Circle != "")
                    {
                        FileName = datalist.FirstOrDefault().City_Circle + ".xlsx";
                    }
                    else
                    {
                        FileName = "Company.xlsx";
                    }

                    //Response.AddHeader("content-disposition", "attachment;filename= MemberDetails.xlsx");
                    Response.AddHeader("content-disposition", "attachment;filename= " + FileName);

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
                else
                {
                    TempData["Message"] = "No Company Found to export";
                }
            }
            return RedirectToAction("Index", new { listid = listid, lstname = "" });
            


        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public ActionResult exporttocontactexcel(int listid, string biztype)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var datalist = context.Spcontexportclient(listid, biztype).ToList();
                var dt = ToDataTable(datalist);
                if (dt.Rows.Count > 0)
                {
                    wb.Worksheets.Add(dt);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Random rdn = new Random();
                    string FileName = "";
                    if (datalist.FirstOrDefault().City_Circle != null && datalist.FirstOrDefault().City_Circle != "")
                    {
                        FileName = "ContactList_" + datalist.FirstOrDefault().City_Circle + ".xlsx";
                    }
                    else
                    {
                        FileName = "ContactList_.xlsx";
                    } 
                    //Response.AddHeader("content-disposition", "attachment;filename= MemberDetails.xlsx");
                    Response.AddHeader("content-disposition", "attachment;filename= " + FileName);

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
                else
                {
                    TempData["Message"] = "No Contact Found to export";

                }
            }

          return  RedirectToAction("Index", new { listid = listid, lstname = "" });


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

    }
}