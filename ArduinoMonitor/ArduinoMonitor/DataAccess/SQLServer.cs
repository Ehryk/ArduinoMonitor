using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ArduinoMonitor.BusinessObjects;

namespace ArduinoMonitor.DataAccess
{
    public class SQLServer : BaseData, IDisposable
    {
        #region Connection

        protected override bool SetConnectionString()
        {
#if DEBUG
            connectionString = "Data Source=localhost;Initial Catalog=ArduinoMonitor;Persist Security Info=False;User ID=ArduinoMonitor;Password=password;";
            return true;
#endif
            return false;
        }

        private SqlConnection connection;

        private SqlConnection Connection
        {
            get
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                    return connection;

                connection = new SqlConnection(ConnectionString);
                connection.Open();

                return connection;
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
            connection.Dispose();
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
        
        private const string SENSOR_DATA_GET    = "SensorDataGet";
        private const string SENSOR_DATA_INSERT = "SensorDataInsert";
        private const string SENSOR_DATA_UPDATE = "SensorDataUpdate";
        private const string SENSOR_DATA_DELETE = "SensorDataDelete";

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

        public int InsertArduino(IEnumerable<SqlParameter> parameters)
        {
            int newID = RunIntProcedure(ARDUINO_INSERT, parameters.ToArray());

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
            int newID = RunIntProcedure(EVENT_INSERT, parameters.ToArray());

            return newID;
        }

        #endregion

        #region Sensor Data

        public int InsertSensorData(int pArduinoID, float? pTempCelsius, float? pTempFahrenheit, float? pHumidity, float? pLight = null, DateTime? pDate = null)
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
            int newID = RunIntProcedure(SENSOR_DATA_INSERT, parameters.ToArray());

            return newID;
        }

        #endregion

        #region Helper Functions

        #region Database Methods

        /// <summary>
        /// Calls the given stored procedure with the given parameters.  One or more rows are expected back.
        /// </summary>
        /// <param name="procName">stored procedure name</param>
        /// <param name="parameters">A dictionary of string key/value pairs.  Can be null.</param>
        /// <returns>A DataTable containing the results of the query.</returns>
        private DataTable CallStoredProc(string procName, Dictionary<string, string> parameters)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);

            SqlCommand cmd = new SqlCommand(procName, conn) { CommandType = CommandType.StoredProcedure };

            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> kvp in parameters)
                {
                    if (kvp.Key[0] == '@')
                    {
                        if (kvp.Value == null || kvp.Value == "NULL")
                        {
                            cmd.Parameters.AddWithValue(kvp.Key, DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                        }
                    }
                    else
                    {
                        if (kvp.Value == null || kvp.Value == "NULL")
                        {
                            cmd.Parameters.AddWithValue("@" + kvp.Key, DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
                        }
                    }
                }
            }

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }


        private int CallStoredProc(string pProcName, List<SqlParameter> parameters)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);

            SqlCommand cmd = new SqlCommand(pProcName, conn) { CommandType = CommandType.StoredProcedure };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        private int CallStoredProcWithReturnValue(string procName, Dictionary<string, string> parameters, string returnValueName)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);

            SqlCommand cmd = new SqlCommand(procName, conn) { CommandType = CommandType.StoredProcedure };

            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> kvp in parameters)
                {
                    if (kvp.Key[0] == '@')
                    {
                        if (kvp.Value == null || kvp.Value == "NULL")
                        {
                            cmd.Parameters.AddWithValue(kvp.Key, DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(kvp.Key, kvp.Value);
                        }
                    }
                    else
                    {
                        if (kvp.Value == null || kvp.Value == "NULL")
                        {
                            cmd.Parameters.AddWithValue("@" + kvp.Key, DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(returnValueName))
            {
                SqlParameter returnValue = new SqlParameter(returnValueName, DbType.Int32) { Direction = ParameterDirection.ReturnValue };

                cmd.Parameters.Add(returnValue);
            }

            conn.Open();
            //cmd.Connection = conn;
            cmd.ExecuteNonQuery();

            int vValue = 0;
            if (!string.IsNullOrEmpty(returnValueName)) vValue = Int32.Parse(cmd.Parameters[returnValueName].Value.ToString());

            conn.Close();

            return vValue;
        }
        private DataTable CallSQL(string pSQLName, List<SqlParameter> parameters)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);

            SqlCommand cmd = new SqlCommand(pSQLName, conn) { CommandType = CommandType.Text };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }


        #endregion

        #region Database Helpers

        protected object RunProcedure(string name, SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(name, Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddRange(parameters);

            object result = cmd.ExecuteScalar();
            return result;
        }

        protected int RunIntScalarProcedure(string name, SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(name, Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddRange(parameters);

            int result = (int)cmd.ExecuteScalar();
            return result;
        }

        protected int RunIntProcedure(string name, SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(name, Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddRange(parameters);
            
            SqlParameter returnParameter = new SqlParameter("@ReturnValue", SqlDbType.Int, 4);
            returnParameter.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(returnParameter);

            cmd.ExecuteNonQuery();

            int returnvalue = (int)cmd.Parameters["@ReturnValue"].Value;
            return returnvalue;
        }

        protected int RunRowsProcedure(string name, SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(name, Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddRange(parameters);

            int rows = cmd.ExecuteNonQuery();
            return rows;
        }

        protected IDataReader RunDataReaderProcedure(string name, SqlParameter[] parameters)
        {
            SqlCommand cmd = new SqlCommand(name, Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddRange(parameters);

            return cmd.ExecuteReader();
        }

        protected DataTable RunDataTableProcedure(string name, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();

            SqlCommand cmd = new SqlCommand(name, Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddRange(parameters);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(dt);

            return dt;
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

        #region Generic Helpers

        public static DateTime ToDateTime(TimeSpan pTimeSpan)
        {
            return new DateTime(1900, 1, 1).Add(pTimeSpan);
        }

        public static DateTime? ToDateTime(TimeSpan? pTimeSpan)
        {
            if (pTimeSpan == null)
                return null;

            return new DateTime(1900, 1, 1).Add((TimeSpan)pTimeSpan);
        }

        #endregion

        #endregion
    }
}
