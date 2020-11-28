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
            //老票务系统
            cruiseconn = ConfigurationManager.AppSettings["cruiseconn"];
            //新票务系统
            mysqlconn = ConfigurationManager.AppSettings["mysqlconn"];
            localconn = ConfigurationManager.AppSettings["localconn"];
            accconn = ConfigurationManager.AppSettings["accconn"];
        }
        /// <summary>
        /// 老票务系统
        /// </summary>
        public SqlConnection CruiseConn
        {
            get
            {
                return new SqlConnection(cruiseconn);
            }
        }
        /// <summary>
        /// 新票务系统
        /// </summary>
        public MySqlConnection MysqlConn
        {
            get
            {
                return new MySqlConnection(mysqlconn);
            }
        }
        /// <summary>
        /// 本地数据库
        /// </summary>
        public SqlConnection LocalConn
        {
            get
            {
                return new SqlConnection(localconn);
            }
        }
        /// <summary>
        /// 本地Access数据库
        /// </summary>
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
