using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskApi.Common
{
    public static class Httphelper
    {
        public static string GetRequestCharset(string ctype)
        {
            string charset = "utf-8";
            if (!string.IsNullOrEmpty(ctype))
            {
                string[] entires = ctype.Split(';');
                foreach (string entry in entires)
                {
                    string _entry = entry.Trim();
                    if (_entry.StartsWith("charset"))
                    {
                        string[] pair = _entry.Split('=');
                        if (pair.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(pair[1]))
                            {
                                charset = pair[1].Trim();
                            }
                        }
                        break;
                    }
                }
            }
            return charset;
        }
        public static async Task<string> GetStreamAsString(HttpRequest request, string charset)
        {
            request.EnableBuffering();
            try
            {
                string result = string.Empty;
                // 以字符流的方式读取HTTP请求体
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    result = await reader.ReadToEndAsync();
                }
                request.Body.Position = 0;
                return result;
                // 这里读取过body  Position是读取过几次  而此操作优于控制器先行 控制器只会读取Position为零次的
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
    }
}
