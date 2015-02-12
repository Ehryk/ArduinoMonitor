using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoMonitor
{
    class AppSettings
    {
        private static string comPort = ConfigurationManager.ConnectionStrings["COM_Port"].ConnectionString;
        private static string connectionString = ConfigurationManager.ConnectionStrings["ArduinoMonitor"].ConnectionString;

        public static string ComPort
        {
            get { return comPort; }
        }

        public static string ConnectionString
        {
            get { return connectionString; }
        }
    }
}
