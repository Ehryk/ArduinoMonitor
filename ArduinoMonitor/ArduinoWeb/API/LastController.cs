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
    public class LastController : ApiController
    {
        // GET api/last
        public List<SensorData> Get()
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetSensorDataLast();
        }

        // GET api/last/60
        public List<SensorData> Get(int count)
        {
            SQLServer dataAccess = new SQLServer();
            return dataAccess.GetSensorDataLast(count);
        }
    }
}
