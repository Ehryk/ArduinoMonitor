using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ArduinoMonitor.BusinessObjects
{
    public class SensorData : BaseObject
    {
        #region Properties

        [DisplayName("Arduino ID")]
        public int ArduinoID { get; set; }
        
        [DisplayName("Date")]
        public DateTime Date { get; set; }

        [DisplayName("Year")]
        public int Year { get; set; }

        [DisplayName("Temperature (Celsius)")]
        public decimal? TempCelsius { get; set; }

        [DisplayName("Temperature (Fahrenheit)")]
        public decimal? TempFahrenheit { get; set; }

        [DisplayName("Humidity")]
        public decimal? Humidity { get; set; }
        
        [DisplayName("Light")]
        public decimal? Light { get; set; }

        // Auxilary Properties
        [DisplayName("Elapsed Minutes")]
        public int? ElapsedMinutes { get; set; }

        #endregion

        #region Constructors

        public SensorData()
        {
        }

        public SensorData(IDataReader pDataReader)
        {
            LoadBase(pDataReader, false);

            ArduinoID = ToInt(pDataReader["Arduino"]);

            Date = ToDateTime(pDataReader["Date"]);
            Year = ToInt(pDataReader["Year"]);

            TempCelsius = ToNullableDecimal(pDataReader["TempCelsius"]);
            TempFahrenheit = ToNullableDecimal(pDataReader["TempFahrenheit"]);
            Humidity = ToNullableDecimal(pDataReader["Humidity"]);
            Light = ToNullableDecimal(pDataReader["Light"]);

            if (ColumnExists(pDataReader, "ElapsedMinutes"))
                ElapsedMinutes = ToInt(pDataReader["ElapsedMinutes"]);
        }

        public SensorData(DataRow pRow)
        {
            Load(pRow, false);

            ArduinoID = ToInt(pRow["Arduino"]);

            Date = ToDateTime(pRow["Date"]);
            Year = ToInt(pRow["Year"]);

            TempCelsius = ToNullableDecimal(pRow["TempCelsius"]);
            TempFahrenheit = ToNullableDecimal(pRow["TempFahrenheit"]);
            Humidity = ToNullableDecimal(pRow["Humidity"]);
            Light = ToNullableDecimal(pRow["Light"]);

            if (ColumnExists(pRow, "ElapsedMinutes"))
                ElapsedMinutes = ToInt(pRow["ElapsedMinutes"]);
        }

        #endregion

        #region Parameters

        public IEnumerable<SqlParameter> InsertParameters()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            
            parameters.Add(new SqlParameter("@Arduino", ArduinoID));
            parameters.Add(new SqlParameter("@Date", Date));

            parameters.Add(new SqlParameter("@TempCelsius", TempCelsius));
            parameters.Add(new SqlParameter("@TempFahrenheit", TempFahrenheit));
            parameters.Add(new SqlParameter("@Humidity", Humidity));
            parameters.Add(new SqlParameter("@Light", Light));

            return parameters;
        }

        #endregion
    }
}
