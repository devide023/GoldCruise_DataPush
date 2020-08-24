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
                    CruisePushService service = new CruisePushService();
                    var list = service.Get_GusetBy_Hc("G320191001");
                    var ser = new XmlSerializer(typeof(List<sys_guest>));
                    using (TextWriter tw = new StringWriter())
                    {
                        ser.Serialize(tw, list);
                        log.Info(tw.ToString());
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
