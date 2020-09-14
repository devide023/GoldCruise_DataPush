using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using log4net;
using Newtonsoft.Json;
using System.Xml.Serialization;
using GoldCruise_DataPush.Models;
using System.IO;

namespace GoldCruise_DataPush
{
    public class CruiseDataPushJob : IJob
    {
        private ILog log;
        private Services.PushDataService pds;
        public CruiseDataPushJob()
        {
            log = LogManager.GetLogger(this.GetType());
            pds = new Services.PushDataService();
            pds.fromdb = FromDB.老系统;
        }
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                try
                {                    
                    CruisePushService services = new CruisePushService();
                    List<sys_ship> list = services.GetDayShipList();
                    foreach (var item in list)
                    {
                        string hcbh = item.hcbh.Replace("HC", "");
                        var guests = services.Get_GusetBy_Hc(hcbh);
                        sys_push_entity push_entity = new sys_push_entity();
                        push_entity.ship = item;
                        push_entity.guests = guests.ToList();
                        pds.PushDataTo(push_entity);
                    }     
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    throw;
                }
            });
        }
    }
}
