using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
namespace GoldCruise_DataPush
{
    public class NewCruiseService
    {
        public NewCruiseService()
        {

        }

        public IEnumerable<dynamic> MysqlTest()
        {
            using (IDbConnection conn =  new DAO().MysqlConn)
            {
                string sql = "select * from sys_user";
                return conn.Query(sql);
            }
        }
    }
}
