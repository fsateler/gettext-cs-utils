using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gettext.Samples.Mvc.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = Strings.T("Welcome to internationalized ASP.NET MVC!");

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
