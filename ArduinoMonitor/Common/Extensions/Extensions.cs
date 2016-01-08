using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoMonitor.Extensions
{
    public static class Extensions
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

        public static int ToInt(this string s, int pDefault = -1)
        {
            return ToNullableInt(s) ?? pDefault;
        }

        public static int? ToNullableInt(this string s)
        {
            if (s == null)
                return null;

            if (String.IsNullOrWhiteSpace(s))
                return null;

            int i;
            if (Int32.TryParse(s.Replace("?", "").Replace("$", ""), out i))
            {
                return i;
            }

            return null;
        }
    }
}
