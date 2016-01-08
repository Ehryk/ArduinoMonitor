using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ArduinoMonitor.Objects
{
    public class BaseObjectAudit : BaseObject
    {
        #region Properties
        
        [JsonIgnore]
        [XmlIgnore]
        public int? CreateUser { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public DateTime? CreateDate { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public int? UpdateUser { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public DateTime? UpdateDate { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public int? DeleteUser { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        public DateTime? DeleteDate { get; set; }

        #endregion

        #region Load BaseObject

        protected void Load(IDataReader pDataReader, bool pLoadAudit = true)
        {
            base.Load(pDataReader);

            if (pLoadAudit)
            {
                if (ColumnExists(pDataReader, "CreateUser"))
                    CreateUser = ToNullableInt(pDataReader["CreateUser"]);
                if (ColumnExists(pDataReader, "CreateDate"))
                    CreateDate = ToNullableDateTime(pDataReader["CreateDate"]);

                if (ColumnExists(pDataReader, "UpdateUser"))
                    UpdateUser = ToNullableInt(pDataReader["UpdateUser"]);
                if (ColumnExists(pDataReader, "UpdateDate"))
                    UpdateDate = ToNullableDateTime(pDataReader["UpdateDate"]);

                if (ColumnExists(pDataReader, "DeleteUser"))
                    DeleteUser = ToNullableInt(pDataReader["DeleteUser"]);
                if (ColumnExists(pDataReader, "DeleteDate"))
                    DeleteDate = ToNullableDateTime(pDataReader["DeleteDate"]);
            }
        }

        protected void Load(DataRow pRow, bool pLoadAudit = true)
        {
            base.Load(pRow);

            if (pLoadAudit)
            {
                if (ColumnExists(pRow, "CreateUser"))
                    CreateUser = ToNullableInt(pRow["CreateUser"]);
                if (ColumnExists(pRow, "CreateDate"))
                    CreateDate = ToNullableDateTime(pRow["CreateDate"]);

                if (ColumnExists(pRow, "UpdateUser"))
                    UpdateUser = ToNullableInt(pRow["UpdateUser"]);
                if (ColumnExists(pRow, "UpdateDate"))
                    UpdateDate = ToNullableDateTime(pRow["UpdateDate"]);

                if (ColumnExists(pRow, "DeleteUser"))
                    DeleteUser = ToNullableInt(pRow["DeleteUser"]);
                if (ColumnExists(pRow, "DeleteDate"))
                    DeleteDate = ToNullableDateTime(pRow["DeleteDate"]);
            }
        }

        #endregion

        #region Helper Methods

        #endregion
    }
}
