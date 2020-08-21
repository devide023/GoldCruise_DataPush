using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    }
}
