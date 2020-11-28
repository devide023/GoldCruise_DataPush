using GoldCruise_DataPush.SM;
using log4net;
using Org.BouncyCastle.Utilities.Encoders;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace GoldCruise_DataPush
{
    public partial class DataPushService : ServiceBase
    {
        private ILog logger;
        private IScheduler scheduler;
        public DataPushService()
        {
            InitializeComponent();
            logger = LogManager.GetLogger(this.GetType());
            Init().GetAwaiter().GetResult();
        }

        private async Task Init()
        {
            ISchedulerFactory factory = new StdSchedulerFactory();
            scheduler = await factory.GetScheduler();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (!scheduler.IsStarted)
                {
                    //启动调度器
                    scheduler.Start();
                    logger.Info("--------服务开启---------");
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        protected override void OnStop()
        {
            if (!scheduler.IsShutdown)
            {
                scheduler.Shutdown();
            }
            base.OnStop();
            logger.Info("--------服务停止---------");
        }

        protected override void OnPause()
        {
            scheduler.PauseAll();
            base.OnPause();
            logger.Info("--------服务暂停---------");
        }

        protected override void OnContinue()
        {
            scheduler.ResumeAll();
            base.OnContinue();
            logger.Info("--------服务继续---------");
        }
    }
}
