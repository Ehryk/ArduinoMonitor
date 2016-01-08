using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArduinoWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Dashboard";

            return View();
        }

        public ActionResult API()
        {
            ViewBag.Title = "API";

            return View();
        }
    }
}
