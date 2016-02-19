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
    public class EventRecentController : ApiController
    {
        // GET api/eventrecent
        public List<Event> Get()
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetEventsRecent();
        }

        // GET api/eventrecent/60
        public List<Event> Get(int count)
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetEventsRecent(count);
        }
    }
}
