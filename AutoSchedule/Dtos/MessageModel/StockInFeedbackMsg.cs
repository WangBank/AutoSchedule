using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{

    public class StockInFeedbackMsg
    {
        public StockInFeedEntryorder entryOrder { get; set; }
        public List<StockInFeedOrderline> orderLines { get; set; }
    }
    //{"entryOrder":{"entryOrderCode":"202004140001","ownerCode":"CBU8816093026319","warehouseCode":"800001573","clpsOrderCode":"CPL4418047933347","orderType":"CGRK","poOrderStatus":"70","operateTime":"2020-04-14 17:15:04","confirmType":0,"createUser":"romensfzl","outBizCode":"4976a6f3-3027-43ab-9d21-279d3ce20f63","ediRemark":""},"orderLines":[{"itemNo":"00000001","itemName":"耳聋左慈丸","itemId":"CMG4418287716460","planQty":2,"realInstoreQty":2,"diffQty":0,"shortQty":0,"damagedQty":0,"emptyQty":0,"expiredQty":0,"otherDiffQty":0,"remark":"","goodsStatus":"1","planFloatQty":2.0,"realInstoreFloatQty":2.0,"diffFloatQty":0.0,"shortFloatQty":0.0,"damagedFloatQty":0.0,"emptyFloatQty":0.0,"expiredFloatQty":0.0,"otherDiffFloatQty":0.0,"orderLineNo":"1"}]}
    public class StockInFeedEntryorder
    {
        public string entryOrderCode { get; set; }
        public string ownerCode { get; set; }
        public string warehouseCode { get; set; }
        public string clpsOrderCode { get; set; }
        public string orderType { get; set; }
        public string poOrderStatus { get; set; }
        public string operateTime { get; set; }
        public string confirmType { get; set; }
        public string createUser { get; set; }
        public string outBizCode { get; set; }
        public string ediRemark { get; set; }
    }

    public class StockInFeedOrderline
    {
        public string itemNo { get; set; }
        public string itemName { get; set; }
        public string itemId { get; set; }
        public string planQty { get; set; }
        public string realInstoreQty { get; set; }
        public string diffQty { get; set; }
        public string shortQty { get; set; }
        public string damagedQty { get; set; }
        public string emptyQty { get; set; }
        public string expiredQty { get; set; }
        public string otherDiffQty { get; set; }
        public string remark { get; set; }
        public string goodsStatus { get; set; }
        public string planFloatQty { get; set; }
        public string realInstoreFloatQty { get; set; }
        public string diffFloatQty { get; set; }
        public string shortFloatQty { get; set; }
        public string damagedFloatQty { get; set; }
        public string emptyFloatQty { get; set; }
        public string expiredFloatQty { get; set; }
        public string otherDiffFloatQty { get; set; }
        public string orderLineNo { get; set; }
    }

}
