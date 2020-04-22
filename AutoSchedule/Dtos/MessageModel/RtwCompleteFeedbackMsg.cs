using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    public class RtwCompleteFeedbackMsg
    {
        public RtwOrderModel rtwOrderModel { get; set; }

        public List<RtwOrderItem> rtwOrderItemList { get; set; }
    }

    public class RtwOrderModel
    {
        public string rtwOrderCode { get; set; }
        public string rtwOrderId { get; set; }
        public string warehouseCode { get; set; }
        public string orderType { get; set; }
        public string bizType { get; set; }
        public string rtwStatus { get; set; }
        public string ownerNo { get; set; }
        public string outBizCode { get; set; }
        public string orderConfirmTime { get; set; }
        public string logisticsCode { get; set; }

        public string logisticsName { get; set; }
        public string expressCode { get; set; }

        public string returnReason { get; set; }
        public string remark { get; set; }

        public  SenderInfo senderInfo { get; set; }

    }

    public class SenderInfo
    {
        public string company { get; set; }
       
        public string name { get; set; }
    
        public string zipCode { get; set; }
       
        public string tel { get; set; }
      
        public string mobile { get; set; }
    
        public string email { get; set; }
     
        public string countryCode { get; set; }
     
        public string province { get; set; }
     
        public string city { get; set; }
     
        public string area { get; set; }
       
        public string town { get; set; }
       
        public string detailAddress { get; set; }
    }


    public class RtwOrderItem
    {
        public string orderLineNo { get; set; }
        public string orderSourceCode { get; set; }
        public string subSourceCode { get; set; }
        public string ownerCode { get; set; }
        public string itemCode { get; set; }
        public string itemId { get; set; }
        public string inventoryType { get; set; }
        public string planQty { get; set; }
        public string actualQty { get; set; }
        public string planOutQty { get; set; }

        public string actualOutQty { get; set; }
        public string batchCode { get; set; }

        public string productDate { get; set; }
        public string expireDate { get; set; }

        public string produceCode { get; set; }
        public string qrCode { get; set; }

        public List<Batch> batchs { get; set; }


        public SenderInfo senderInfo { get; set; }

    }
    public class Batch
    {
        public string batchCode { get; set; }
        public string productDate { get; set; }
        public string expireDate { get; set; }
        public string produceCode { get; set; }
        public string inventoryType { get; set; }
        public string actualQty { get; set; }
        public string actualOutQty { get; set; }
    }
}
