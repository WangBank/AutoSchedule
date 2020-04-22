using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    public class OrderCancelFeedbackMsg
    {
        public string status { get; set; }
        public string message { get; set; }
        public string orderCode { get; set; }
        public string orderId { get; set; }
        public string orderType { get; set; }
        public string failureReason { get; set; }
    }
}
