using GoldCruise_DataPush.SM;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;
using log4net;
namespace GoldCruise_DataPush
{
    public class ApiService
    {
        private ILog logger;
        private string _token;
        public string GetSM3(string pwd)
        {
            byte[] msg = Encoding.Default.GetBytes(pwd);
            byte[] md = new byte[32];
            SM3Digest sm3 = new SM3Digest();
            sm3.BlockUpdate(msg, 0, msg.Length);
            sm3.DoFinal(md, 0);
            string PasswdDigest = new UTF8Encoding().GetString(Hex.Encode(md));
            return PasswdDigest;
        }
        public void InitToken()
        {
            string username = ConfigurationManager.AppSettings["username"];
            string userpwd = ConfigurationManager.AppSettings["userpwd"];
            string url = ApiUrl.baseurl + ApiUrl.tokenurl;
            string pwd = this.GetSM3(userpwd);
            string param = "{\"username\":\"" + username + "\",\"password\":\"" + pwd + "\"}";
            string resjson = Tool.HttpPostData(url, param);
            var resobj = JsonConvert.DeserializeObject<apitokenobj>(resjson);
            this._token = resobj.data;
        }
        public string GetToken
        {
            get {
                return this._token;
            }
        }

        public ApiService()
        {
            logger = LogManager.GetLogger(this.GetType());
            this._token = string.Empty;
            this.InitToken();
            logger.Info("------token:" + this._token + "------");
        }
        /// <summary>
        /// 提交航次信息
        /// </summary>
        /// <returns></returns>
        public string PostHCInfo(object postobj)
        {
            try
            {
                string res = string.Empty;
                string url = ApiUrl.baseurl + ApiUrl.hcurl;
                string postdata = JsonConvert.SerializeObject(postobj);
                res = Tool.HttpPostWithHeader(url, postdata, this._token);

                logger.Info("posturl:" + url);
                logger.Info("postdata:" + postdata);
                logger.Info("res:" + res);
                return res;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        public string PassengerInfo(api_passenger passenger)
        {
            try
            {
                string url = ApiUrl.baseurl + ApiUrl.guseturl;
                string post_json = JsonConvert.SerializeObject(passenger);
                string res_json = Tool.HttpPostWithHeader(url, post_json, _token);

                logger.Info("res:" + res_json);
                logger.Info("posturl:" + url);
                logger.Info("postdata:" + post_json);
                return res_json;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }
    }
}
