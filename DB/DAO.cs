using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Configuration;
namespace GoldCruise_DataPush
{
    public class DAO : IDisposable
    {
        private string cruiseconn = string.Empty;
        private string mysqlconn = string.Empty;
        public DAO()
        {
            cruiseconn = ConfigurationManager.AppSettings["cruiseconn"];
            mysqlconn = ConfigurationManager.AppSettings["mysqlconn"];
        }
        public SqlConnection CruiseConn
        {
            get
            {
                return new SqlConnection(cruiseconn);
            }
        }

        public MySqlConnection MysqlConn
        {
            get
            {
                return new MySqlConnection(mysqlconn);
            }
        }

        public void Dispose()
        {

        }
    }
}
