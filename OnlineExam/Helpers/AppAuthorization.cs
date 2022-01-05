using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineExam.Helpers
{
    public class AppAuthorization : AuthorizeAttribute
    {
        bool IsSessionExist = false;

        #region UnauthorizedRequest
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (IsSessionExist == false)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.Write("<script>alert('Login time out! Please Loggin again!');window.location.href='../Account/Login';</script>");
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" }, { "ReturnUrl", filterContext.HttpContext.Request.FilePath } });
                }

            }
            else
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.Write("<script>UnAuthorized()</script>");
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "UnAuthorized" } });
                }

            }

        }
        #endregion

        #region AuthorizeCore
        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{

        //    if (AppSession.UserId > 0)
        //    {
        //        IsSessionExist = true;
        //        return true;
        //    }
        //    else
        //    {
        //        IsSessionExist = false;
        //        return false;
        //    }
        //}


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            if (httpContext.Request.IsAuthenticated)
            {


                IsSessionExist = true;
                string controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
                string action = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();
                //string roleName = httpContext.User.GETROLENAME();
                //if (roleName == "General User")
                //{
                //    var adminfeature = GetAllAdminFeatures();
                //    var isAdminFeature = adminfeature.Where(a => a.ControllerName.ToLower() == controller.ToLower()).Any();
                //    if (isAdminFeature)
                //    {
                //        return false;
                //    }
                //}
                return true;
            }
            else
            {
                IsSessionExist = false;
                return false;
            }
        }

        private List<Features> GetAllAdminFeatures()
        {
            List<Features> list = new List<Features>();
            list.Add(new Features
            {
                ID = 1,
                ControllerName = "Advices",
                Title = "Advices",
            });
            list.Add(new Features
            {
                ID = 2,
                ControllerName = "Brands",
                Title = "Brands",
            });
            list.Add(new Features
            {
                ID = 3,
                ControllerName = "DentalSchools",
                Title = "Dental Schools",
            });
            list.Add(new Features
            {
                ID = 4,
                ControllerName = "Designations",
                Title = "Designations",
            });
            list.Add(new Features
            {
                ID = 5,
                ControllerName = "Diagnostics",
                Title = "Diagnossis",
            });
            list.Add(new Features
            {
                ID = 6,
                ControllerName = "Diseases",
                Title = "Diseases",
            });
            list.Add(new Features
            {
                ID = 7,
                ControllerName = "Generics",
                Title = "Generics",
            });
            list.Add(new Features
            {
                ID = 8,
                ControllerName = "Medicines",
                Title = "Medicines",
            });
            list.Add(new Features
            {
                ID = 9,
                ControllerName = "Pathologies",
                Title = "Pathologies",
            });
            list.Add(new Features
            {
                ID = 10,
                ControllerName = "Subscriptions",
                Title = "Subscriptions",
            });
            list.Add(new Features
            {
                ID = 11,
                ControllerName = "Treatments",
                Title = "Treatments",
            });
            return list;
        }

        public class Features
        {
            public int ID { get; set; }
            public string ControllerName { get; set; }
            public string Title { get; set; }
            public bool IsView { get; set; }
            public bool IsAdd { get; set; }
            public bool IsEdit { get; set; }
            public bool IsDelete { get; set; }
        }
        //private bool CheckPermition(string roleName, string controller, string action)
        //{
        //    var res = GetAllFeatures();

        //    res.
        //    if (roleName == "Super Admin")
        //    {


        //        res.
        //    }

        //}
        #endregion
    }
}