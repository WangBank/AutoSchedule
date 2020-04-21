using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace AutoTask
{
    public static class CommonHelper
    {
        public static string SqlDataBankToString(this object bankData)
        {
            return bankData == DBNull.Value ? "" : bankData.ToString();
        }
        public static int SqlDataBankToInt(this object bankData)
        {
            return bankData == DBNull.Value ? 0 : Convert.ToInt32(bankData);
        }
       
        public static void Log(string content, string FileName)
        {
            try
            {
                string filename = FileName + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "Log/" + filename;
                FileInfo file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Log/" + filename);
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString());
                sb.Append(" ");
                sb.Append(content);
                FileMode fm = new FileMode();
                if (!file.Exists)
                {
                    fm = FileMode.Create;
                }
                else
                {
                    fm = FileMode.Append;
                }
                using (FileStream fs = new FileStream(filePath, fm, FileAccess.Write, FileShare.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("gb2312")))
                    {
                        sw.WriteLine(sb.ToString());
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log("生成Log失败" + ex.Message.ToString(), "Log异常");
            }
        }
    }
}
