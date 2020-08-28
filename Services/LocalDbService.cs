using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using log4net;
namespace GoldCruise_DataPush
{
    public class LocalDbService
    {
        private ILog log;
        private string datadir = string.Empty;
        public LocalDbService()
        {
            log = LogManager.GetLogger(this.GetType());
            datadir = AppDomain.CurrentDomain.BaseDirectory+"DB";
            AppDomain.CurrentDomain.SetData("DataDirectory", datadir);
        }

        public bool add(List<dynamic> list)
        {
            using (IDbConnection conn = new DAO().AccConn)
            {
                string sql = "insert into pushinfo(hcbh,certno) values(@hcbh,@certno);";
                int cnt = conn.Execute(sql, list);
                return cnt>0?true:false;
            }
        }
    }
}
