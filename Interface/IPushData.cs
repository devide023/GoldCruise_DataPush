using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldCruise_DataPush
{
    /// <summary>
    /// 推送数据接口
    /// </summary>
    public interface IPushData
    {
        /// <summary>
        /// 推送数据到水路客运联网售票系统接口
        /// </summary>
        /// <param name="push_entity"></param>
        /// <returns></returns>
        bool PushDataTo(sys_push_entity push_entity);
    }
}
