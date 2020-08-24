using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoldCruise_DataPush
{
    public static class Tool
    {
        public static void WriteLog(string msg)
        {
            string dirpath = AppDomain.CurrentDomain.BaseDirectory + "Log";
            string filepath = $@"{dirpath}\{DateTime.Now:yyyyMMdd}.txt";
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            using (TextWriter tw = new StreamWriter(filepath, true, Encoding.UTF8))
            {
                tw.WriteLine(msg);
                tw.Close();
            }
        }

        public static string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("utf-8"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static string HttpPostData(string url, string param)
        {
            var result = string.Empty;
            // 设置提交的相关参数 
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            if (request == null)
            {
                throw new ApplicationException(
                string.Format("Invalid url string: {0}", url)
                );
            }
            //注意提交的编码 这边是需要改变的 这边默认的是Default：系统当前编码
            byte[] postData = Encoding.UTF8.GetBytes(param);
            
            Encoding myEncoding = Encoding.UTF8;
            request.Method = "POST";
            request.ContentType = "text/xml";
            request.ContentLength = postData.Length;
            // 提交请求数据 
            System.IO.Stream outputStream = request.GetRequestStream();
            outputStream.Write(postData, 0, postData.Length);
            outputStream.Close();

            HttpWebResponse response;
            Stream responseStream;
            StreamReader reader;
            string srcString;
            response = request.GetResponse() as HttpWebResponse;
            responseStream = response.GetResponseStream();
            reader = new System.IO.StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
            srcString = reader.ReadToEnd();
            result = srcString;   //返回值赋值
            reader.Close();

            return result;
        }

        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }
}
