using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ArduinoMonitor.Objects;
using ArduinoMonitor.DataAccess;

namespace ArduinoWeb.Controllers
{
    public class ArduinosController : ApiController
    {
        // GET api/arduinos
        public List<Arduino> Get()
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetArduinos();
        }

        // GET api/arduinos/60
        public List<Arduino> Get(int count)
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetArduinos();
        }
    }
}
