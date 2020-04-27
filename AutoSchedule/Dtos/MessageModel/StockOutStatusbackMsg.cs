using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    //{"status":100153,"deliveryOrderCode":"202004240001","operateTime":"2020-04-26 11:12:04","operateUser":"orderAccept","deliveryOrderId":"CSL8816306781203","warehouseCode":"800001573","vmiType":false,"ownerCode":"CBU8816093026319"}
    public class StockOutStatusbackMsg
    {
        public string status { get; set; }
        public string deliveryOrderCode { get; set; }
        public string deliveryOrderId { get; set; }
        public string operateTime { get; set; }
        public string operateUser { get; set; }
        public string remark { get; set; }
    }
}
