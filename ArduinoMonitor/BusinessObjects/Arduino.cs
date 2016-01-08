using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ArduinoMonitor.BusinessObjects
{
    public class Arduino : BaseObject
    {
        #region Properties
        
        [DisplayName("Build Date")]
        public DateTime? BuildDate { get; set; }
        
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Location")]
        public string Location { get; set; }
        
        [DisplayName("Sensors")]
        public string Sensors { get; set; }
        
        [DisplayName("Builder")]
        public string Builder { get; set; }
        
        public string Comment { get; set; }
        public bool Deleted { get; set; }

        #endregion

        #region Constructors

        public Arduino()
        {
        }

        public Arduino(int pID = -1, string pName = "")
        {
            ID = pID;
            Name = pName;
        }

        public Arduino(IDataReader pDataReader)
        {
            LoadBase(pDataReader);
            
            Name = pDataReader["Name"].ToString();
            BuildDate = ToNullableDateTime(pDataReader["BuildDate"]);

            Location = pDataReader["Location"].ToString();
            Sensors = pDataReader["Sensors"].ToString();
            Builder = pDataReader["Builder"].ToString();

            Comment = pDataReader["Comment"].ToString();
            Deleted = ToBool(pDataReader["Deleted"]);
        }

        public Arduino(DataRow pRow)
        {
            Load(pRow);
            
            Name = pRow["Name"].ToString();
            BuildDate = ToNullableDateTime(pRow["BuildDate"]);

            Location = pRow["Location"].ToString();
            Sensors = pRow["Sensors"].ToString();
            Builder = pRow["Builder"].ToString();

            Comment = pRow["Comment"].ToString();
            Deleted = ToBool(pRow["Deleted"]);
        }

        #endregion

        #region Parameters

        public IEnumerable<SqlParameter> InsertParameters()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            
            parameters.Add(new SqlParameter("@Name", Name));
            parameters.Add(new SqlParameter("@BuildDate", BuildDate));

            parameters.Add(new SqlParameter("@Location", Location));
            parameters.Add(new SqlParameter("@Sensors", Sensors));
            parameters.Add(new SqlParameter("@Builder", Builder));
            
            parameters.Add(new SqlParameter("@Comment", Comment));

            return parameters;
        }

        public IEnumerable<SqlParameter> UpdateParameters()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            
            parameters.Add(new SqlParameter("@ID", ID));
            parameters.Add(new SqlParameter("@Name", Name));
            parameters.Add(new SqlParameter("@BuildDate", BuildDate));

            parameters.Add(new SqlParameter("@Location", Location));
            parameters.Add(new SqlParameter("@Sensors", Sensors));
            parameters.Add(new SqlParameter("@Builder", Builder));
            
            parameters.Add(new SqlParameter("@Comment", Comment));

            return parameters;
        }

        #endregion
    }
}
