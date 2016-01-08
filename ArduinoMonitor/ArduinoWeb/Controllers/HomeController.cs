using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArduinoWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Dashboard(int? id)
        {
            ViewBag.Title = "Dashboard";
            ViewBag.Count = id.HasValue ? id : 60;

            return View();
        }

        public ActionResult API()
        {
            ViewBag.Title = "API";

            return View();
        }
    }
}
