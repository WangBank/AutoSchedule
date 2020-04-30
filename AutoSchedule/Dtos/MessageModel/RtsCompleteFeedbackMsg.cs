using Jdwl.Api.Domain.Clps.ClpsOpenGwService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    public class RtsCompleteFeedbackMsg
    {
        public RtsOrderModel rtsOrderModel { get; set; }

        public List<RtsItemModel> rtsItemModelList { get; set; }
    }

    public class RtsOrderModel
    {
        public string rtsOrderCode { get; set; }
        public string rtsOrderId { get; set; }
        public string rtsType { get; set; }
        public string ownerNo { get; set; }
        public string deliveryMode { get; set; }
        public string warehouseNo { get; set; }
        public string supplierNo { get; set; }
        public string supplierName { get; set; }
        public string orderCreateTime { get; set; }
        public string remark { get; set; }

        public ReceiverInfo receiverInfo { get; set; }

    }

    public class RtsItemModel
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

    }
}
