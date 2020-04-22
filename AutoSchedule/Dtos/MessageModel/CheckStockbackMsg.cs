using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{

        public class CheckStockFeedbackMsg
    {
            public string warehouseCode { get; set; }
            public string checkOrderCode { get; set; }
            public string checkOrderId { get; set; }
            public string ownerCode { get; set; }
            public string checkTime { get; set; }
            public string outBizCode { get; set; }
            public string remark { get; set; }
            public List<CheckStockItem> checkStockItemList { get; set; }
          
        }
    public class CheckStockItem
    {
        public string itemCode { get; set; }
        public string itemId { get; set; }
        public string inventoryType { get; set; }
        public string quantity { get; set; }
        public string quantityValue { get; set; }
        public string remark { get; set; }

    }
}
