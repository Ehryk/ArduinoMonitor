using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ArduinoMonitor.Objects
{
    public class BaseObject
    {
        #region Properties

        public int ID { get; set; }

        #endregion

        #region Load BaseObject

        protected void Load(IDataReader pDataReader)
        {
            ID = ToInt(pDataReader["ID"]);
        }

        protected void Load(DataRow pRow)
        {
            ID = ToInt(pRow["ID"]);
        }

        #endregion

        #region Helper Methods

        //public static Nullable<T> ToNullable<T>(object o)
        //{
        //    if (String.IsNullOrWhiteSpace(text))
        //        return null;

        //    DateTime date;
        //    if (DateTime.TryParse(text, out date))
        //    {
        //        return date;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        protected static TNull? ToNullable<TNull>(object pValue) where TNull : struct
        {
            if (pValue != DBNull.Value)
            {
                try
                {
                    pValue = Convert.ChangeType(pValue, typeof(TNull));
                    return (TNull)pValue;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public static bool ColumnExists(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).ToUpper() == columnName.ToUpper())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ColumnExists(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName);
        }

        public static DateTime ToDateTime(object o)
        {
            return ToNullableDateTime(o) ?? DateTime.MinValue;
        }

        public static DateTime? ToNullableDateTime(object o)
        {
            if (o == null)
                return null;
            
            if (o is DateTime) return (DateTime)o;
            string text = o is string ? (string)o : o.ToString();

            if (String.IsNullOrWhiteSpace(text))
                return null;

            DateTime date;
            if (DateTime.TryParse(text, out date))
            {
                return date;
            }
            
            return null;
        }

        public static bool ToBool(object o)
        {
            return ToNullableBool(o) ?? false;
        }

        public static bool? ToNullableBool(object o)
        {
            if (o == null)
                return null;
            
            if (o is bool) return (bool)o;
            string text = o is string ? (string)o : o.ToString();

            if (String.IsNullOrWhiteSpace(text))
                return null;

            bool value;
            if (Boolean.TryParse(text, out value))
            {
                return value;
            }
            
            return null;
        }

        public static int ToInt(object o)
        {
            return ToNullableInt(o) ?? -1;
        }

        public static int? ToNullableInt(object o)
        {
            if (o == null)
                return null;
            
            if (o is int) return (int)o;
            string text = o is string ? (string)o : o.ToString();

            if (String.IsNullOrWhiteSpace(text))
                return null;

            int i;
            if (Int32.TryParse(text.Replace("?", "").Replace("$", ""), out i))
            {
                return i;
            }

            return null;
        }

        public static decimal ToDecimal(object o)
        {
            return ToNullableDecimal(o) ?? 0;
        }

        public static decimal? ToNullableDecimal(object o)
        {
            if (o == null)
                return null;

            if (o is decimal) return (decimal)o;
            string text = o is string ? (string)o : o.ToString();

            if (String.IsNullOrWhiteSpace(text))
                return null;

            decimal d;
            if (Decimal.TryParse(text.Replace("?", "").Replace("$", "").Replace("(", "").Replace(")", "").Replace("-", ""), out d))
            {
                //If negative
                if (text.Contains("(") || text.Contains("-"))
                    return 0 - d;

                return d;
            }

            return null;
        }

        public static double? ToNullableDouble(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return null;

            double d;
            if (Double.TryParse(text.Replace("?", "").Replace("$", ""), out d))
            {
                return d;
            }
            
            return null;
        }

        #endregion
    }
}
