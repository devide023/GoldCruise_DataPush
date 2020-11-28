using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldCruise_DataPush
{
    public class api_passenger
    {
        /// <summary>
        /// 乘客姓名,是否必要参数：是
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 国籍,是否必要参数：是
        /// </summary>
        public string countryId { get; set; }
        /// <summary>
        /// 联系电话,是否必要参数：是
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 证件号,是否必要参数：是
        /// </summary>
        public string certNo { get; set; }
        /// <summary>
        /// 证件类型,是否必要参数：是
        /// </summary>
        public int certType { get; set; }
        /// <summary>
        /// 游客类型,是否必要参数：是
        /// </summary>
        public string passengerType { get; set; }
        /// <summary>
        /// 性别,是否必要参数：是
        /// male=1，female=2
        /// </summary>
        public int Sex { get; set; }
    }
}
