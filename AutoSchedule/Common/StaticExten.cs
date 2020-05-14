using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public  static class StaticExten
    {
        public static string SqlDataBankToString(this object bankData)
        {
            return bankData == DBNull.Value ? "" : bankData.ToString();
        }

        public static string ToJsonCommon(this object commonPara)
        {
            return JsonConvert.SerializeObject(commonPara);
        }
    }
}
