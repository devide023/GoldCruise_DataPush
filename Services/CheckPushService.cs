using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldCruise_DataPush.Models;
using System.Data;
using Dapper;
using log4net;
using Newtonsoft.Json;
namespace GoldCruise_DataPush
{
    public class CheckPushService : ICheckPushData
    {
        private ILog log;
        public CheckPushService()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        /// <summary>
        /// 数据是否提交过
        /// </summary>
        /// <param name="pushinfo"></param>
        /// <returns>true提交过，false未提交过</returns>
        public bool IsPushed(pushdatainfo pushinfo)
        {
            using (IDbConnection conn = new DAO().LocalConn)
            {
                String sql = "select count(*) as cnt from pushinfo where hcbh=@hcbh and name=@name and certno=@certno and fromdb=@fromdb";
                var list = conn.Query(sql, new { hcbh = pushinfo.hcbh.Replace(" ",""), name = pushinfo.name.Replace(" ", ""), certno = pushinfo.certno.Replace(" ", ""),fromdb = pushinfo.fromdb });
                int cnt = -1;
                int.TryParse(list.First().cnt.ToString(),out cnt);
                if (cnt==0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
