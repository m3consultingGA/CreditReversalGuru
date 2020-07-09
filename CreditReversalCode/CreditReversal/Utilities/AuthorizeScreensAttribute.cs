using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace CreditReversal.Utilities
{
    public class AuthorizeScreensAttribute
    {
    }
    public class Authorization : AuthorizeAttribute
    {
        private bool _authorize;
        private bool loginTokenFailure;
        public Authorization()
        {
            this._authorize = false;
            this.loginTokenFailure = false;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                string name = HttpContext.Current.User.Identity.Name;
                loginTokenFailure = false;
                //string name = HttpContext.Current.User.Identity.Name;
                if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["UserId"] != null)
                {
                    _authorize = true;
                    return _authorize;
                }
                _authorize = false;
                return _authorize;
            }
            catch (Exception)
            {
                return false;
            }
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest()) // If the request is from an Ajax call returning 401 status code
            {
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.HttpContext.Response.Headers.Add("isauthenticated", "False");
                //filterContext.HttpContext.Response.End();
            }
            if (loginTokenFailure)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Home", //TODO Changed Home page
                            action = "Index",
                            tokenError = "Yes"
                        })
                    );
            }
            if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["UserId"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "home",
                            action = "index"
                        })
                    );
            }
        }
    }
    public class CustomBaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //SessionValues getSessionValues = new AccountController().GetSessionValues();
            //if (getSessionValues != null)
            //{
            //    ViewBag.LastLogin = !string.IsNullOrEmpty(getSessionValues.LastLogin) ? getSessionValues.LastLogin : string.Empty;
            //    ViewBag.UserName = !string.IsNullOrEmpty(getSessionValues.UserName) ? getSessionValues.UserName : string.Empty;
            //}
        }
    }
    public class LoginAuthorize : AuthorizeAttribute
    {
        private bool _authorize;
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                string name = HttpContext.Current.User.Identity.Name;
                _authorize = false;
                if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["UserId"] != null)
                {
                        _authorize = true;
                }
                return _authorize;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public class ScreensAuthorize : AuthorizeAttribute
    {
        private bool _authorize;
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                string name = HttpContext.Current.User.Identity.Name;
                _authorize = false;
                if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["UserRole"] != null)
                {
                    string role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
                    if(role.ToUpper() == "ADMIN")
                    {
                        _authorize = true;
                    }
                }
                return _authorize;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Result = new HttpUnauthorizedResult(); // Try this but i'm not sure
            filterContext.Result = new RedirectResult("~/Error/NotFound");
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }

    }
}