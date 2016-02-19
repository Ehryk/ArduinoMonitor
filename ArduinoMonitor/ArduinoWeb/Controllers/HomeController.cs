using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ArduinoMonitor.Objects;
using ArduinoMonitor.DataAccess;
using ArduinoMonitor.Extensions;
using ArduinoWeb.Models;

namespace ArduinoWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Dashboard(int? id)
        {
            //Default
            return Current(id);
        }

        public ActionResult Last(int? id)
        {
            ViewBag.ApiLink = "api/last";
            ViewBag.TimeBased = false;
            ViewBag.Title = "Dashboard (Last)";
            ViewBag.Count = id.HasValue ? id : 60;
            ViewBag.TimeZoneOffset = ConfigurationManager.AppSettings["TimeZoneOffset"].ToInt(0);

            return View("Dashboard");
        }

        public ActionResult Recent(int? id)
        {
            ViewBag.ApiLink = "api/recent";
            ViewBag.TimeBased = true;
            ViewBag.Title = "Dashboard (Recent)";
            ViewBag.Count = id.HasValue ? id : 60;
            ViewBag.TimeZoneOffset = ConfigurationManager.AppSettings["TimeZoneOffset"].ToInt(0);

            return View("Dashboard");
        }

        public ActionResult Current(int? id)
        {
            ViewBag.ApiLink = "api/current";
            ViewBag.TimeBased = true;
            ViewBag.Title = "Dashboard (Current)";
            ViewBag.Count = id.HasValue ? id : 60;
            ViewBag.TimeZoneOffset = ConfigurationManager.AppSettings["TimeZoneOffset"].ToInt(0);

            return View("Dashboard");
        }

        public ActionResult EventLast(int? id)
        {
            ViewBag.ApiLink = "api/eventlast";
            ViewBag.TimeBased = false;
            ViewBag.Title = "Events";
            ViewBag.Count = id.HasValue ? id : 100;
            ViewBag.TimeZoneOffset = ConfigurationManager.AppSettings["TimeZoneOffset"].ToInt(0);

            return View("Events");
        }

        public ActionResult EventRecent(int? id)
        {
            ViewBag.ApiLink = "api/eventrecent";
            ViewBag.TimeBased = true;
            ViewBag.Title = "Events";
            ViewBag.Count = id.HasValue ? id : 100;
            ViewBag.TimeZoneOffset = ConfigurationManager.AppSettings["TimeZoneOffset"].ToInt(0);

            return View("Events");
        }

        public ActionResult Arduinos(int? id)
        {
            ViewBag.Title = "Arduinos";
            ViewBag.TimeZoneOffset = ConfigurationManager.AppSettings["TimeZoneOffset"].ToInt(0);

            SQLServer dataAccess = new SQLServer();
            var model = new ArduinosViewModel(dataAccess.GetArduinos());

            return View("Arduinos", model);
        }

        public ActionResult API()
        {
            ViewBag.Title = "API";

            return View();
        }
    }
}
