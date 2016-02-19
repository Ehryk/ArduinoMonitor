using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ArduinoMonitor.Objects
{
    public class Event : BaseObject
    {
        #region Properties

        [DisplayName("Year")]
        public int Year { get; set; }

        [DisplayName("Date")]
        public DateTime Date { get; set; }

        [DisplayName("Arduino ID")]
        public int ArduinoID { get; set; }

        [DisplayName("Source")]
        public string Source { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }

        [DisplayName("Message")]
        public string Message { get; set; }

        [DisplayName("IsException")]
        public bool IsException { get; set; }

        [DisplayName("Exception")]
        public string Exception { get; set; }

        [DisplayName("StackTrace")]
        public string StackTrace { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        [DisplayName("Deleted")]
        public bool Deleted { get; set; }

        // Auxilary Properties
        [DisplayName("Elapsed Minutes")]
        public int? ElapsedMinutes { get; set; }

        #endregion

        #region Constructors

        public Event()
        {
        }

        public Event(IDataReader pDataReader)
        {
            base.Load(pDataReader);
            
            Year = ToInt(pDataReader["Year"]);
            Date = ToDateTime(pDataReader["Date"]);
            ArduinoID = ToInt(pDataReader["Arduino"]);

            Source = pDataReader["Source"].ToString();
            Type = pDataReader["Type"].ToString();
            Message = pDataReader["Message"].ToString();
            IsException = ToBool(pDataReader["IsException"]);
            Exception = pDataReader["Exception"].ToString();
            StackTrace = pDataReader["StackTrace"].ToString();
            Comment = pDataReader["Comment"].ToString();
            Deleted = ToBool(pDataReader["Deleted"]);

            if (ColumnExists(pDataReader, "ElapsedMinutes"))
                ElapsedMinutes = ToInt(pDataReader["ElapsedMinutes"]);
        }

        public Event(DataRow pRow)
        {
            base.Load(pRow);
            
            Year = ToInt(pRow["Year"]);
            Date = ToDateTime(pRow["Date"]);
            ArduinoID = ToInt(pRow["Arduino"]);

            Source = pRow["Source"].ToString();
            Type = pRow["Type"].ToString();
            Message = pRow["Message"].ToString();
            IsException = ToBool(pRow["IsException"]);
            Exception = pRow["Exception"].ToString();
            StackTrace = pRow["StackTrace"].ToString();
            Comment = pRow["Comment"].ToString();
            Deleted = ToBool(pRow["Deleted"]);

            if (ColumnExists(pRow, "ElapsedMinutes"))
                ElapsedMinutes = ToInt(pRow["ElapsedMinutes"]);
        }

        #endregion

        #region Parameters

        //public IEnumerable<SqlParameter> InsertParameters()
        //{
        //    List<SqlParameter> parameters = new List<SqlParameter>();
            
        //    parameters.Add(new SqlParameter("@Arduino", ArduinoID));
        //    parameters.Add(new SqlParameter("@Date", Date));

        //    parameters.Add(new SqlParameter("@TempCelsius", TempCelsius));
        //    parameters.Add(new SqlParameter("@TempFahrenheit", TempFahrenheit));
        //    parameters.Add(new SqlParameter("@Humidity", Humidity));
        //    parameters.Add(new SqlParameter("@Light", Light));

        //    return parameters;
        //}

        #endregion
    }
}
