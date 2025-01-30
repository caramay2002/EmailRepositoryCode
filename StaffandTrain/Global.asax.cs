using StaffandTrain.Common;
using StaffandTrain.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace StaffandTrain
{
    public class MvcApplication : System.Web.HttpApplication
    {
        SATConn context = new SATConn();
        private Timer timer;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);            
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;

            
            StartTimer();
        }
        protected void Application_End(object sender, EventArgs e)
        {
            timer?.Dispose();
        }

        private void StartTimer()
        {
            int minute = int.Parse(ConfigurationManager.AppSettings["EmailWorkerSchedule"]);
            timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(minute));
        }

        private void TimerCallback(object state)
        {

            DateTime date = DateTime.Now;
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                SendEmail email = new SendEmail();
                email.ScheduleWorkerEmail();
            }

        }
    }
}
