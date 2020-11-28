using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldCruise_DataPush
{
    public static class ApiUrl
    {
        public static string baseurl
        {
            get
            {
                return "https://test.56rely.com/devapi/";
            }
        }
        /// <summary>
        /// token数据接口
        /// </summary>
        public static string tokenurl
        {
            get
            {
                return "spl/login/loginNoAuthCode.do";
            }
        }
        /// <summary>
        /// 航次信息数据接口
        /// </summary>
        public static string hcurl
        {
            get
            {
                return "spl/plan/add.do";
            }
        }

        /// <summary>
        /// 乘客信息数据接口
        /// </summary>
        public static string guseturl
        {
            get
            {
                return "spl/plan/passenger/add/cruiseplanid.do";
            }
        }
    }
}
