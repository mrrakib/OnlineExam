using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;


namespace OnlineExam.Helpers
{
    public static class IdentityData
    {
        public static string GETUSERNAME(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (String.IsNullOrEmpty(identity.Name) && identity.FindFirst("username")==null)
            {
                return "";
            }
            string context = identity.FindFirst("username")?.Value;
            return context;
        }
        public static string GETUSEREMAIL(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (String.IsNullOrEmpty(identity.Name) && identity.FindFirst("userphone") == null)
            {
                return "";
            }
            string context = identity.FindFirst("useremail")?.Value;
            return context;
        }
        public static string GETUSERPHONENO(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (String.IsNullOrEmpty(identity.Name) && identity.FindFirst("userphone") == null)
            {
                return "";
            }
            string context = identity.FindFirst("userphone")?.Value;
            return context;
        }
        public static string GETSTUDENTID(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (String.IsNullOrEmpty(identity.Name) && identity.FindFirst("studentId") == null)
            {
                return "";
            }
            string context = identity.FindFirst("studentId")?.Value;
            return context;
        }
        public static string GETUSERFULLNAME(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (String.IsNullOrEmpty(identity.Name) && identity.FindFirst("fullname") == null)
            {
                return "";
            }
            string context = identity.FindFirst("fullname")?.Value;
            return context;
        }
        public static string GETROLENAME(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (String.IsNullOrEmpty(identity.Name) && identity.FindFirst("rolename") == null)
            {
                return "";
            }
            string context = identity.FindFirst("rolename")?.Value;
            return context;
        }
        public static string GETUSERID(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (String.IsNullOrEmpty(identity.Name) && identity.FindFirst("userid") == null)
            {
                return "";
            }
            string context = identity.FindFirst("userid")?.Value;
            return context;
        }

        public static bool GETISUSERACTIVE(this IPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (String.IsNullOrEmpty(identity.Name) && identity.FindFirst("isactive") == null)
            {
                return false;
            }
            bool context = Convert.ToBoolean(identity.FindFirst("isactive")?.Value);
            return context;
        }
    }
}