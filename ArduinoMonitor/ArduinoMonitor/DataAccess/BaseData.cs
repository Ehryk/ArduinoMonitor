using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoMonitor.DataAccess
{
    public class BaseData
    {
        #region Connection String

        protected string connectionString = "";

        public string ConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(connectionString))
                    SetConnectionString();
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }

        protected virtual bool SetConnectionString()
        {
            return false;
        }

        #endregion
    }
}
