using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Configuration;
namespace GoldCruise_DataPush
{
    public class DAO : IDisposable
    {
        private string cruiseconn = string.Empty;
        private string mysqlconn = string.Empty;
        private string localconn = string.Empty;
        private string accconn = string.Empty;
        public DAO()
        {
            cruiseconn = ConfigurationManager.AppSettings["cruiseconn"];
            mysqlconn = ConfigurationManager.AppSettings["mysqlconn"];
            localconn = ConfigurationManager.AppSettings["localconn"];
            accconn = ConfigurationManager.AppSettings["accconn"];
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
        public SqlConnection LocalConn
        {
            get
            {
                return new SqlConnection(localconn);
            }
        }

        public OleDbConnection AccConn
        {
            get
            {
                return new OleDbConnection(accconn);
            }
        }

        public void Dispose()
        {

        }
    }
}
