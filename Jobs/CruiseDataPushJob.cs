using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using log4net;
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
                    log.Info(DateTime.Now.ToString("yyyy-MM-dd") +"数据推送开始");
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
