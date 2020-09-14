using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using log4net;
namespace GoldCruise_DataPush.Jobs
{
    public class NewCruiseDataPushJob : IJob
    {
        private ILog log;
        private Services.PushDataService pds;
        public NewCruiseDataPushJob()
        {
            log = LogManager.GetLogger(this.GetType());
            pds = new Services.PushDataService();
            pds.fromdb = FromDB.新系统;
        }
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    NewCruiseService service = new NewCruiseService();
                    var list = service.GetDayShipList();
                    foreach (var item in list)
                    {
                        var guests = service.Get_GusetBy_Hc(item.hcbh);
                        sys_push_entity entity = new sys_push_entity();
                        entity.ship = item;
                        entity.guests = guests.ToList();
                        pds.PushDataTo(entity);
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
