using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AutoSchedule.Common
{
    public static  class CommonHelper
    {
        public static async Task<string> HttpPostAsync(string strURL, string paramsStr, HttpClient client,string xaccesstoken = "" )
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(xaccesstoken))
                {
                    headers.Add("x-access-token", xaccesstoken);
                }
                int timeOut = 30;
                client.Timeout = new TimeSpan(0, 0, timeOut);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                using (HttpContent httpContent = new StringContent(paramsStr, Encoding.UTF8))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PostAsync(strURL, httpContent);
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return "错误：" + ex.Message;
            }
        }

        public static async Task<string> HttpGetAsync(string strURL, string xaccesstoken = "")
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Content-Type", "application/json");
                headers.Add("x-access-token", xaccesstoken);
                int timeOut = 30;

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, timeOut);
                    if (headers != null)
                    {
                        foreach (var header in headers)
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    string response = await client.GetStringAsync(strURL);
                    return response;
                }
            }
            catch (Exception)
            {
                return "错误";
            }
        }


        public static Dictionary<string, string> GetQueryMap(string queryString, string charset)
        {
            Dictionary<string, string> queryMap = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(queryString))
            {
                queryString = queryString.Substring(1); // 忽略?号
                string[] parameters = queryString.Split('&');
                foreach (string parameter in parameters)
                {
                    string[] kv = parameter.Split('=');
                    if (kv.Length == 2)
                    {
                        string key = HttpUtility.UrlDecode(kv[0], Encoding.GetEncoding(charset));
                        string value = HttpUtility.UrlDecode(kv[1], Encoding.GetEncoding(charset));
                        queryMap.Add(key, value);
                    }
                    else if (kv.Length == 1)
                    {
                        string key = HttpUtility.UrlDecode(kv[0], Encoding.GetEncoding(charset));
                        queryMap.Add(key, "");
                    }
                }
            }
            return queryMap;
        }

       
       
    }
}