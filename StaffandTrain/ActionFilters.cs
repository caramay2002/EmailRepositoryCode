using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace StaffandTrain
{
    public class ActionFilters : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                //filterContext.HttpContext.Response.Redirect("/Login/Login", true);
                filterContext.HttpContext.Response.Redirect("/Login/Index", true);
            }

        }
        /// <summary>
        /// Logs the request vars.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="values">The values.</param>
     
    }
}