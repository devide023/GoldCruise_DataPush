using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
namespace GoldCruise_DataPush
{
    public class DAO : IDisposable
    {
        private string cruiseconn = string.Empty;
        public DAO()
        {
            cruiseconn = ConfigurationManager.AppSettings["cruiseconn"];
        }
        public SqlConnection CruiseConn
        {
            get
            {
                return new SqlConnection(cruiseconn);
            }
        }

        public void Dispose()
        {

        }
    }
}
