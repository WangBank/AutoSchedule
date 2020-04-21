using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    /// <summary>
    /// 出库完成回传
    /// </summary>
    public class StockOutFeedbackMsg
    {
        public StockOutDeliveryOrder entryOrder { get; set; }
        public List<StockOutOrderline> orderLines { get; set; }
    }

    public class StockOutDeliveryOrder
    {
        public string deliveryOrderCode { get; set; }
        public string deliveryOrderId { get; set; }
        public string salePlatformOrderCode { get; set; }
        public string salePlatformSourceCode { get; set; }
        public string warehouseCode { get; set; }
        public string orderType { get; set; }
        public string confirmType { get; set; }
        public string sellerNo { get; set; }
        public string deptNo { get; set; }
        public string shopNo { get; set; }
        public string deliveryDate { get; set; }
        public string operatorCode { get; set; }
        public string operatorName { get; set; }
        public string operateTime { get; set; }
    }

    public class StockOutOrderline
    {
        public string orderLineNo { get; set; }
        public string orderSourceCode { get; set; }
        public string ownerCode { get; set; }
        public string shopGoodsNo { get; set; }
        public string itemCode { get; set; }
        public string itemId { get; set; }
        public string inventoryType { get; set; }
        public string planQty { get; set; }
        public string actualQty { get; set; }
        public string actualOutQty { get; set; }    
    }
}
