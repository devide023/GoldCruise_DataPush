using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldCruise_DataPush.Models
{
    public class sys_ship
    {
        public int id {get;set;}
        /// <summary>
        /// 邮轮名称成
        /// </summary>
        public string shipname{get;set; }
        /// <summary>
        /// 航次编号
        /// </summary>
        public string hcbh{get;set; }
        /// <summary>
        /// 开船时间
        /// </summary>
        public DateTime sdate{get;set; }
        /// <summary>
        /// 抵达时间
        /// </summary>
        public DateTime edate{get;set; }
        /// <summary>
        /// 上船码头
        /// </summary>
        public string scmt{get;set; }
        /// <summary>
        /// 下船码头
        /// </summary>
        public string xcmt{get;set; }
        /// <summary>
        /// 开始港口
        /// </summary>
        public string sgk{get;set; }
        /// <summary>
        /// 到达港口
        /// </summary>
        public string egk{get;set; }
        /// <summary>
        /// 上下水
        /// </summary>
        public string sxs{get;set; }
        /// <summary>
        /// 航线
        /// </summary>
        public string linename{get;set; }
    }
}
