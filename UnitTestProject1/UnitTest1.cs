using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoldCruise_DataPush;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ApiService service = new ApiService();
            api_passenger passenger = new api_passenger
            {
                certNo = "51162219900901252X",
                certType = (int)CertType.身份证,
                countryId = "中国",
                name = "罗旭宏",
                passengerType = "01",
                phone = "13272712304",
                Sex = (int)SEX.女,

            };

            api_hcinfo hcinfo = new api_hcinfo
            {
                activeStatus = "enable",
                debarkPort = "秭归港",
                departureDate = "2020-11-28 22:00:00",
                deptCode = "重庆港",
                destCode = "秭归港",
                embarkPort = "15码头",
                productId = "G10000001",
                shipRouteId = 1,
                total = 0,
                arriveDate = "2020-11-30 00:00:00",
                bookingPrice = 0,
                crossedPrice = 0,
                cruiseCode = "黄金9号"
            };
            string res = service.PostHCInfo(hcinfo);
            Console.WriteLine(res);
        }
    }
}
