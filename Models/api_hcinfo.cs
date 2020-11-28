using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldCruise_DataPush
{
    public class api_hcinfo
    {
        /// <summary>
        /// 邮轮,是否必要参数：否
        /// </summary>
        public string cruiseCode { get; set; }
        /// <summary>
        /// 产品ID,是否必要参数：是
        /// </summary>
        public string productId { get; set; }
        /// <summary>
        /// 航次出发日期,是否必要参数：是
        /// </summary>
        public string departureDate { get; set; }
        /// <summary>
        /// 预计抵达时间,是否必要参数：否
        /// </summary>
        public string arriveDate { get; set; }
        /// <summary>
        /// 出发港口,是否必要参数：是
        /// </summary>
        public string deptCode { get; set; }
        /// <summary>
        /// 抵达港口,是否必要参数：是
        /// </summary>
        public string destCode { get; set; }
        /// <summary>
        /// 登船码头,是否必要参数：是
        /// </summary>
        public string embarkPort { get; set; }
        /// <summary>
        /// 下船码头,是否必要参数：是
        /// </summary>
        public string debarkPort { get; set; }
        /// <summary>
        /// 航线,是否必要参数：是
        /// </summary>
        public Int64 shipRouteId { get; set; }
        /// <summary>
        /// 航次总库存,是否必要参数：是
        /// </summary>
        public Int32 total { get; set; }
        /// <summary>
        /// 预订价格,是否必要参数：否
        /// </summary>
        public decimal bookingPrice { get; set; }
        /// <summary>
        /// 划线价,是否必要参数：否
        /// </summary>
        public decimal crossedPrice { get; set; }
        /// <summary>
        /// 状态,是否必要参数：是
        /// 可用值:enable,disable
        /// </summary>
        public string activeStatus { get; set; }
    }
}
