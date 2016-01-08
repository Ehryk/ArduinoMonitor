using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ArduinoMonitor.Extensions;

namespace ArduinoWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Dashboard(int? id)
        {
            ViewBag.Title = "Dashboard";
            ViewBag.Count = id.HasValue ? id : 60;
            ViewBag.TimeZoneOffset = ConfigurationManager.AppSettings["TimeZoneOffset"].ToInt(0);

            return View();
        }

        public ActionResult API()
        {
            ViewBag.Title = "API";

            return View();
        }
    }
}
