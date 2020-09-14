using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using Dapper;
using GoldCruise_DataPush.Models;
using log4net;

namespace GoldCruise_DataPush.Services
{
    public class PushDataService : CheckPushService,IPushData
    {
        private ILog log;
        public PushDataService()
        {
            log = LogManager.GetLogger(this.GetType());
        }

        public FromDB fromdb { get; set; }

        /// <summary>
        /// 推送数据到水路客运联网售票系统接口
        /// </summary>
        /// <param name="push_entity"></param>
        /// <returns></returns>
        public bool PushDataTo(sys_push_entity push_entity)
        {
            try
            {
                using (IDbConnection conn = new DAO().LocalConn)
                {
                    List<pushdatainfo> list = new List<pushdatainfo>();
                    foreach (var item in push_entity.guests)
                    {
                        pushdatainfo info = new pushdatainfo();
                        info.hcbh = push_entity.ship.hcbh.Replace(" ", "");
                        info.name = item.name.Replace(" ", "");
                        info.certno = item.idcardno.Replace(" ", "");
                        info.fromdb = this.fromdb;
                        if (!IsPushed(info))
                        {
                            //推送数据到接口

                            //保存推送数据到本地
                            list.Add(info);
                        }
                    }
                    string savetoloal_sql = "insert into pushinfo (hcbh,name,certno,fromdb) values (@hcbh,@name,@certno,@fromdb) ";
                    int cnt = conn.Execute(savetoloal_sql, list);
                    list.Clear();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
