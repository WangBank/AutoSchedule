using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    public class StockOutStatusbackMsg
    {
        public string soStatus { get; set; }
        public string deliveryOrderCode { get; set; }
        public string deliveryOrderId { get; set; }
        public string operateTime { get; set; }
        public string operateUser { get; set; }
        public string remark { get; set; }
    }
}
