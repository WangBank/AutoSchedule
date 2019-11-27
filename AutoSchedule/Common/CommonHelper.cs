using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class CommonHelper
    {
        public  async Task<string> HttpPostAsync(string strURL, string paramsStr, string xaccesstoken = "")
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(xaccesstoken))
                {
                    headers.Add("x-access-token", xaccesstoken);
                }
                int timeOut = 30;
                using (HttpClient client = new HttpClient())
                {
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
            }
            catch (Exception ex)
            {
                return "错误：" + ex.Message;
            }
        }

        public  async Task<string> HttpGetAsync(string strURL, string xaccesstoken = "")
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

        //public static string DataTableToJson(DataTable table)
        //{
        //    var JsonString = new StringBuilder();
        //    if (table.Rows.Count > 0)
        //    {
        //        JsonString.Append("[");
        //        for (int i = 0; i < table.Rows.Count; i++)
        //        {
        //            JsonString.Append("{");
        //            for (int j = 0; j < table.Columns.Count; j++)
        //            {
        //                if (j < table.Columns.Count - 1)
        //                {
        //                    JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
        //                }
        //                else if (j == table.Columns.Count - 1)
        //                {
        //                    JsonString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
        //                }
        //            }
        //            if (i == table.Rows.Count - 1)
        //            {
        //                JsonString.Append("}");
        //            }
        //            else
        //            {
        //                JsonString.Append("},");
        //            }
        //        }
        //        JsonString.Append("]");
        //    }
        //    return JsonString.ToString();
        //}

        //public static string ObjectToJson<T>(T tojson)
        //{
        //    //return Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes<T>(tojson
        //    //    , options: new System.Text.Json.JsonSerializerOptions
        //    //{
        //    //    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        //    //})
        //    //    ).Replace('\\',' ');

        //}
    }
}