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
        public CruiseDataPushJob()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    LocalDbService service = new LocalDbService();
                    List<dynamic> list = new List<dynamic>();
                    list.Add(new { hcbh = "hc20200824", certno = "11" });
                    list.Add(new { hcbh = "hc20200824", certno = "12" });
                    list.Add(new { hcbh = "hc20200824", certno = "13" });
                    bool ret = service.add(list);
                    log.Info("---ok---");        
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
