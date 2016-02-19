using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ArduinoMonitor.Objects;

namespace ArduinoMonitor.DataAccess
{
    public class SQLServer : BaseData, IDisposable
    {
        #region Connection

        protected override bool SetConnectionString()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ArduinoMonitor"].ConnectionString;
            return true;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(ConnectionString);
            }
        }

        #endregion

        #region Constructors/Destructor

        public static SQLServer Instance
        {
            get { return new SQLServer(); }
        }

        public void Dispose()
        {
            //connection.Dispose();
        }

        #endregion

        #region Procedure Names

        private const string ARDUINO_GET    = "ArduinoGet";
        private const string ARDUINOS_GET   = "ArduinosGet";
        private const string ARDUINO_INSERT = "ArduinoInsert";
        private const string ARDUINO_UPDATE = "ArduinoUpdate";
        private const string ARDUINO_DELETE = "ArduinoDelete";
        
        private const string EVENT_GET    = "EventGet";
        private const string EVENTS_GET   = "EventsGet";
        private const string EVENT_INSERT = "EventInsert";
        private const string EVENT_UPDATE = "EventUpdate";
        private const string EVENT_DELETE = "EventDelete";

        private const string EVENTS_LAST = "EventsLast";
        private const string EVENTS_RECENT = "EventsRecent";

        private const string SENSOR_DATA_GET    = "SensorDataGet";
        private const string SENSOR_DATA_INSERT = "SensorDataInsert";
        private const string SENSOR_DATA_UPDATE = "SensorDataUpdate";
        private const string SENSOR_DATA_DELETE = "SensorDataDelete";

        private const string SENSOR_DATA_LAST = "SensorDataLast";
        private const string SENSOR_DATA_RECENT = "SensorDataRecent";
        private const string SENSOR_DATA_CURRENT = "SensorDataCurrent";
        private const string SENSOR_DATA_GRAPH = "SensorDataGraph";

        #endregion

        #region Arduinos

        public Arduino GetArduino(int pID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(NewSqlParameter("@ID", pID, SqlDbType.Int));

            DataTable dt = RunDataTableProcedure(ARDUINO_GET, parameters.ToArray());

            if (dt.Rows.Count == 0)
                return null;

            return new Arduino(dt.Rows[0]);
        }

        public List<Arduino> GetArduinos()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            DataTable dt = RunDataTableProcedure(ARDUINOS_GET, parameters.ToArray());
            
            List<Arduino> data = new List<Arduino>();
            foreach (DataRow row in dt.Rows)
            {
                data.Add(new Arduino(row));
            }

            return data;
        }

        public int InsertArduino(IEnumerable<SqlParameter> parameters)
        {
            int newID = RunIntScalarProcedure(ARDUINO_INSERT, parameters.ToArray());

            return newID;
        }

        public bool UpdateArduino(IEnumerable<SqlParameter> parameters)
        {
            int affected = RunRowsProcedure(ARDUINO_UPDATE, parameters.ToArray());

            return affected == 1;
        }

        public bool DeleteArduino(int pID)
        {
            Arduino arduino = GetArduino(pID);

            if (arduino == null)
                return false;
            
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(NewSqlParameter("@ID", pID, SqlDbType.Int));
            int affected = RunRowsProcedure(ARDUINO_DELETE, parameters.ToArray());

            return affected != 0;
        }

        #endregion

        #region Event Log

        public List<Event> GetEventsLast(int? pCount = null)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (pCount != null)
                parameters.Add(new SqlParameter("@Events", pCount));

            DataTable dt = RunDataTableProcedure(EVENTS_LAST, parameters);

            List<Event> data = new List<Event>();
            foreach (DataRow row in dt.Rows)
            {
                data.Add(new Event(row));
            }

            return data;
        }

        public List<Event> GetEventsRecent(int? pMinutes = null)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (pMinutes != null)
                parameters.Add(new SqlParameter("@Minutes", pMinutes));

            DataTable dt = RunDataTableProcedure(EVENTS_RECENT, parameters);

            List<Event> data = new List<Event>();
            foreach (DataRow row in dt.Rows)
            {
                data.Add(new Event(row));
            }

            return data;
        }

        public int InsertEvent(int pArduinoID, string pMessage, EventType? pType = null, bool pIsException = false, string pException = null, string pStackTrace = null, DateTime? pDate = null, string pSource = "ArduinoMonitor")
        {
            string type = pType != null ? pType.Value.ToString() : null;
            List<SqlParameter> parameters = new List<SqlParameter>();
            
            parameters.Add(new SqlParameter("@Arduino", pArduinoID));

            parameters.Add(new SqlParameter("@Source", pSource));
            parameters.Add(new SqlParameter("@Message", pMessage));
            parameters.Add(new SqlParameter("@Type", type));
            
            parameters.Add(new SqlParameter("@IsException", pIsException));
            parameters.Add(new SqlParameter("@Exception", pException));
            parameters.Add(new SqlParameter("@StackTrace", pStackTrace));

            if (pDate.HasValue)
                parameters.Add(new SqlParameter("@Date", pDate));

            return InsertEvent(parameters);
        }


        public int InsertEvent(IEnumerable<SqlParameter> parameters)
        {
            int newID = RunIntScalarProcedure(EVENT_INSERT, parameters.ToArray());

            return newID;
        }

        #endregion

        #region Sensor Data

        public List<SensorData> GetSensorDataLast(int? pCount = null)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (pCount != null)
                parameters.Add(new SqlParameter("@Points", pCount));

            DataTable dt = RunDataTableProcedure(SENSOR_DATA_LAST, parameters);

            List<SensorData> data = new List<SensorData>();
            foreach (DataRow row in dt.Rows)
            {
                data.Add(new SensorData(row));
            }

            return data;
        }

        public List<SensorData> GetSensorDataRecent(int? pMinutes = null)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (pMinutes != null)
                parameters.Add(new SqlParameter("@Minutes", pMinutes));

            DataTable dt = RunDataTableProcedure(SENSOR_DATA_RECENT, parameters);

            List<SensorData> data = new List<SensorData>();
            foreach (DataRow row in dt.Rows)
            {
                data.Add(new SensorData(row));
            }

            return data;
        }

        public List<SensorData> GetSensorDataCurrent(int? pMinutes = null)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (pMinutes != null)
                parameters.Add(new SqlParameter("@Minutes", pMinutes));

            DataTable dt = RunDataTableProcedure(SENSOR_DATA_CURRENT, parameters);

            List<SensorData> data = new List<SensorData>();
            foreach (DataRow row in dt.Rows)
            {
                data.Add(new SensorData(row));
            }

            return data;
        }

        public int InsertSensorData(int pArduinoID, decimal? pTempCelsius, decimal? pTempFahrenheit, decimal? pHumidity, decimal? pLight = null, DateTime? pDate = null)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            
            parameters.Add(new SqlParameter("@Arduino", pArduinoID));
            parameters.Add(new SqlParameter("@TempCelsius", pTempCelsius));
            parameters.Add(new SqlParameter("@TempFahrenheit", pTempFahrenheit));
            parameters.Add(new SqlParameter("@Humidity", pHumidity));
            parameters.Add(new SqlParameter("@Light", pLight));

            if (pDate.HasValue)
                parameters.Add(new SqlParameter("@Date", pDate));

            return InsertSensorData(parameters);
        }

        public int InsertSensorData(IEnumerable<SqlParameter> parameters)
        {
            int newID = RunIntScalarProcedure(SENSOR_DATA_INSERT, parameters.ToArray());

            return newID;
        }

        #endregion

        #region Database Helpers

        protected object RunScalarProcedure(string name, IEnumerable<SqlParameter> parameters)
        {
            using (SqlConnection conn = Connection)
            {
                SqlCommand cmd = new SqlCommand(name, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddRange(parameters.ToArray());

                object result = cmd.ExecuteScalar();
                return result;
            }
        }

        protected int RunIntScalarProcedure(string name, IEnumerable<SqlParameter> parameters)
        {
            using (SqlConnection conn = Connection)
            {
                SqlCommand cmd = new SqlCommand(name, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddRange(parameters.ToArray());

                int result = (int)cmd.ExecuteScalar();
                return result;
            }
        }

        protected int RunReturnProcedure(string name, IEnumerable<SqlParameter> parameters)
        {
            using (SqlConnection conn = Connection)
            {
                SqlCommand cmd = new SqlCommand(name, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddRange(parameters.ToArray());

                SqlParameter returnParameter = new SqlParameter("@ReturnValue", SqlDbType.Int, 4);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(returnParameter);

                cmd.ExecuteNonQuery();

                int returnvalue = (int)cmd.Parameters["@ReturnValue"].Value;
                return returnvalue;
            }
        }

        protected int RunRowsProcedure(string name, IEnumerable<SqlParameter> parameters)
        {
            using (SqlConnection conn = Connection)
            {
                SqlCommand cmd = new SqlCommand(name, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddRange(parameters.ToArray());

                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
        }

        protected IDataReader RunDataReaderProcedure(string name, IEnumerable<SqlParameter> parameters)
        {
            SqlConnection conn = Connection;
            SqlCommand cmd = new SqlCommand(name, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddRange(parameters.ToArray());

            return cmd.ExecuteReader();
            //Caller should dispose of the reader!
        }

        protected DataTable RunDataTableProcedure(string name, IEnumerable<SqlParameter> parameters = null)
        {
            using (SqlConnection conn = Connection)
            {
                DataTable dt = new DataTable();

                SqlCommand cmd = new SqlCommand(name, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddRange(parameters.ToArray());
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);

                return dt;
            }
        }

        protected SqlParameter NewSqlParameter(string name, object value, DbType? type = null, ParameterDirection direction = ParameterDirection.Input)
        {
            SqlParameter parameter = new SqlParameter(name, value);
            if (type != null)
                parameter.DbType = (DbType)type;
            parameter.Direction = direction;
            return parameter;
        }

        protected SqlParameter NewSqlParameter(string name, object value, SqlDbType? type = null, ParameterDirection direction = ParameterDirection.Input)
        {
            SqlParameter parameter = new SqlParameter(name, value);
            if (type != null)
                parameter.SqlDbType = (SqlDbType)type;
            parameter.Direction = direction;
            return parameter;
        }

        #endregion
    }
}
