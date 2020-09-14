using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldCruise_DataPush.Models;

namespace GoldCruise_DataPush
{
    public class sys_push_entity
    {
        /// <summary>
        /// 邮轮航次信息
        /// </summary>
        public sys_ship ship { get; set; }
        /// <summary>
        /// 航次游客信息
        /// </summary>
        public List<sys_guest> guests { get; set; }
    }
}
