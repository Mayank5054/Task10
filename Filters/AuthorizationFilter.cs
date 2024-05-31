using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
namespace Task10.Filters
{
    public class AuthorizationFilter : FilterAttribute, IAuthorizationFilter
    {
        string[] roles;
        public AuthorizationFilter(string[] _members)
        {
            roles = _members;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Array.IndexOf(roles, filterContext.HttpContext.Session["Type"]) == -1)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {

                    filterContext.Result = new JsonResult
                    {
                        Data = new { status = "failure", message = "UnAuthorized" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Authentication/UnAuthorized");
                }
            }
        }
    }
}