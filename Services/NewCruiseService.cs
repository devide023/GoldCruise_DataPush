using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using GoldCruise_DataPush.Models;

namespace GoldCruise_DataPush
{
    public class NewCruiseService
    {
        public NewCruiseService()
        {

        }
        /// <summary>
        /// 当天开航邮轮信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<sys_ship> GetDayShipList()
        {
            using (IDbConnection conn =  new DAO().MysqlConn)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("    select \n ");
                sql.Append("      t1.name as shipname, \n ");
                sql.Append("      t2.code as hcbh, \n ");
                sql.Append("      t2.title as linename, \n ");
                sql.Append("      t2.go_port as sgk, \n ");
                sql.Append("      t2.to_port as egk, \n ");
                sql.Append("      t2.go_port as scmt, \n ");
                sql.Append("      t2.to_port as xcmt, \n ");
                sql.Append("      t2.start_date as sdate, \n ");
                sql.Append("      t2.end_date as edate \n ");
                sql.Append("    from \n ");
                sql.Append("      sys_ship t1, \n ");
                sql.Append("      ship_line t2 \n ");
                sql.Append("    where t1.id = t2.ship_id \n ");
                sql.Append("      and t2.start_date = current_date(); ");
                return conn.Query<sys_ship>(sql.ToString());
            }
        }

        /// <summary>
        /// 获取航次客人信息
        /// </summary>
        /// <param name="hcbh"></param>
        /// <returns></returns>
        public IEnumerable<sys_guest> Get_GusetBy_Hc(string hcbh) {
            using (IDbConnection conn = new DAO().MysqlConn)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("    select \n ");
                sql.Append("      t1.name as name, \n ");
                sql.Append("      t1.cert_type as certype, \n ");
                sql.Append("      t1.cert_no as idcardno, \n ");
                sql.Append("      t1.sex as sex, \n ");
                sql.Append("      t1.nationality as nation \n ");
                sql.Append("    from \n ");
                sql.Append("      order_guest t1, \n ");
                sql.Append("      `order` t2, \n ");
                sql.Append("      ship_line t3 \n ");
                sql.Append("    where t1.order_id = t2.id \n ");
                sql.Append("      and t1.deleted_at is null \n ");
                sql.Append("      and t2.status in (1, 2, 3)  \n ");
                sql.Append("      and t3.id = t2.shipline_id \n ");
                sql.Append("      and t3.code = @hcbh \n");
                return conn.Query<sys_guest>(sql.ToString(), new { hcbh = hcbh });
            }
        }
    }
}
