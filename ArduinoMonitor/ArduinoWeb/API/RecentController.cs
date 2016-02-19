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
    public class RecentController : ApiController
    {
        // GET api/recent
        public List<SensorData> Get()
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetSensorDataRecent();
        }

        // GET api/recent/60
        public List<SensorData> Get(int count)
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetSensorDataRecent(count);
        }
    }
}
