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
    public class EventLastController : ApiController
    {
        // GET api/eventlast
        public List<Event> Get()
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetEventsLast();
        }

        // GET api/eventlast/100
        public List<Event> Get(int count)
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetEventsLast(count);
        }
    }
}
