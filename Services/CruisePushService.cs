using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using GoldCruise_DataPush.Models;
using System.Data;

namespace GoldCruise_DataPush
{
    /// <summary>
    /// 邮轮数据推送到接口服务
    /// </summary>
    public class CruisePushService
    {
        /// <summary>
        /// 当天开航邮轮信息
        /// </summary>
        /// <returns></returns>
        public List<sys_ship> GetDayShipList()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT t1.名称 AS shipname, \n");
            sql.Append("       t2.* \n");
            sql.Append("FROM   邮轮表 t1, \n");
            sql.Append("       (SELECT 所用邮轮ID                   AS shipid, \n");
            sql.Append("               编号                       AS hcbh, \n");
            sql.Append("               上下水                     AS sxs, \n");
            sql.Append("               CONVERT(DATE, 开船时间, 120) sdate, \n");
            sql.Append("               CONVERT(DATE, 结束时间, 120) AS edate, \n");
            sql.Append("               上船码头                     AS scmt, \n");
            sql.Append("               下船码头                     AS xcmc, \n");
            sql.Append("               起始港                      AS sgk, \n");
            sql.Append("               到达港                      AS egk \n");
            sql.Append("        FROM   [cjhjyl].[dbo].[船期表] \n");
            sql.Append("        WHERE  开船时间 = CONVERT(DATE, '2019-10-01', 120)) t2 \n");
            sql.Append("WHERE  t2.shipid = t1.id");
            using (IDbConnection connection = new DAO().CruiseConn)
            {
                return connection.Query<sys_ship>(sql.ToString()).ToList();
            }
        }
        /// <summary>
        /// 获取航次客人信息
        /// </summary>
        /// <param name="hcbh"></param>
        /// <returns></returns>
        public IEnumerable<sys_guest> Get_GusetBy_Hc(string hcbh)
        {
            using (IDbConnection conn = new DAO().CruiseConn)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT t1.姓名 AS name, \n");
                sql.Append("       t1.性别 AS sex, \n");
                sql.Append("       t1.国籍 AS nation, \n");
                sql.Append("       t1.成小婴 as cxy,\n ");
                sql.Append("       t1.证件类型 AS certype, \n");
                sql.Append("       t1.证件编号 AS idcardno \n");
                sql.Append("FROM   乘客表 AS t1, \n");
                sql.Append("       (SELECT id \n");
                sql.Append("        FROM   [cjhjyl].[dbo].[订单表] \n");
                sql.Append("        WHERE  编号10位 = @hcbh \n");
                sql.Append("               AND 状态 = '订单确认') t2 \n");
                sql.Append("WHERE  t2.id = t1.订单id");
                return conn.Query<sys_guest>(sql.ToString(),new { hcbh=hcbh});
            }
        }
    }
}
