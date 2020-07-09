using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CreditReversal.Controllers
{
    [RoutePrefix("error")]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
        [Route("access-denied")]
        public ActionResult NotFound()
        {
            return View();
        }
    }
}