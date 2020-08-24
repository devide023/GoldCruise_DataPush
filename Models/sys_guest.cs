using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldCruise_DataPush.Models
{
    public class sys_guest
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string sex { get; set; }
        /// <summary>
        /// 国籍
        /// </summary>
        public string nation { get; set; }
        /// <summary>
        /// 成小婴
        /// </summary>
        public string cxy { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string certype { get; set; }
        /// <summary>
        /// 证件编号
        /// </summary>
        public string idcardno { get; set; }
    }
}
