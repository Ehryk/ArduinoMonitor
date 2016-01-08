using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoMonitor.Extensions
{
    static class Extensions
    {
        public static DateTime ToDateTime(this TimeSpan pTimeSpan)
        {
            return new DateTime(1900, 1, 1).Add(pTimeSpan);
        }

        public static DateTime? ToDateTime(this TimeSpan? pTimeSpan)
        {
            if (pTimeSpan == null)
                return null;

            return new DateTime(1900, 1, 1).Add((TimeSpan)pTimeSpan);
        }
    }
}
