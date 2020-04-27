using Jdwl.Api.Domain.Clps.ClpsOpenGwService;
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
        public StockOutDeliveryOrder deliveryOrder { get; set; }
        public List<StockOutOrderline> orderLines { get; set; }
        public List<SoPackage> packages { get; set; }
        public List<LotInfo> lotInfoList { get; set; }
    }

    public class StockOutDeliveryOrder
    {
        public string deliveryOrderCode { get; set; }
        public string deliveryOrderId { get; set; }
        public string orderConfirmTime { get; set; }
        public ReceiverInfo receiverInfo { get; set; }
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
        public string planOutQty { get; set; }
        public string actualQty { get; set; }
        public string actualOutQty { get; set; }    
    }


    public class LotInfo
    {
        public string batchQty { get; set; }
        public string goodsNo { get; set; }
        public string orderLine { get; set; }
    }
}
