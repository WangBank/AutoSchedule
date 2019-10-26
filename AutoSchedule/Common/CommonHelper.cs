using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class CommonHelper
    {
        public static async Task<string> HttpPostAsync(string strURL, string paramsStr, string xaccesstoken = "")
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Content-Type", "application/json");
                headers.Add("x-access-token", xaccesstoken);
                string contentType = null;
                int timeOut = 30;
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, timeOut);
                    if (headers != null)
                    {
                        foreach (var header in headers)
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    using (HttpContent httpContent = new StringContent(paramsStr, Encoding.UTF8))
                    {
                        if (contentType != null)
                            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                        
                        HttpResponseMessage response = await client.PostAsync(strURL, httpContent);
                        return await response.Content.ReadAsStringAsync();
                    }
                }

            }
            catch (Exception)
            {
                return "错误";
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
    }
}
