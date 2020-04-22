using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    public class stockOutCompleteConfirmBackMsg
    {
        public List<RejectGoodsItem> rejectGoodsItemList { get; set; }
        public List<PayMethod> payMethodList { get; set; }

        public List<ReceiveGoodsItem> receiveGoodsItemList { get; set; }
        public string status { get; set; }
        public string deliveryOrderCode { get; set; }
        public string operateTime { get; set; }
        public string operateUser { get; set; }
        public string deliveryOrderId { get; set; }
    }

    public class PayMethod
    {
        public string method { get; set; }
        public string amount { get; set; }
    }

    public class RejectGoodsItem
    {
        public string itemCode { get; set; }
        public string itemId { get; set; }
        public string itemName { get; set; }
        public string quantity { get; set; }
    }

    public class ReceiveGoodsItem
    {
        public string itemCode { get; set; }
        public string itemId { get; set; }
        public string itemName { get; set; }
        public string quantity { get; set; }
    }
}
