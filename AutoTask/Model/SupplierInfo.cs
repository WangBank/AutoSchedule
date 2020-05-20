using Jdwl.Api.Domain.Clps.ClpsOpenGwService;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTask.Model
{

    #region 供应商response类
    public class SupplierResponseBody
    {
        public SupplierNoResponse response { get; set; }
    }

    public class SupplierNoResponse
    {
        public SupplierNoContent content { get; set; }
        public int code { get; set; }
    }

    public class SupplierNoContent
    {
        public string clpsSupplierNo { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
        public string isvSupplierNo { get; set; }
        public string message { get; set; }
    }
    #endregion

    #region 商品response类
    public class SingelResponseBody
    {
        public SingelResponse response { get; set; }
    }
    public class SingelResponse
    {
        public SingelContent content { get; set; }
        public int code { get; set; }
    }

    public class SingelContent
    {
        public string clpsGoodsCode { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
        public string itemCode { get; set; }
        public string message { get; set; }
    }

    #endregion

    #region 店铺response类
    public class ShopResponseBody
    {
        public ShopResponse response { get; set; }
    }
    public class ShopResponse
    {
        public ShopContent content { get; set; }
        public int code { get; set; }
    }

    public class ShopContent
    {
        public string shopNo { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
        public string isvShopNo { get; set; }
        public string message { get; set; }
    }

    #endregion

    #region 发送订单response类
    public class OrderResponseBody
    {
        public OrderResponse response { get; set; }
    }
    public class OrderResponse
    {
        public OrderContent content { get; set; }
        public int code { get; set; }
    }

    public class OrderContent
    {
        public string createTime { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
        public string entryOrderCode { get; set; }
        public string message { get; set; }
    }

    #endregion

    #region 查询订单状态response类
    public class OrderQueryResponseBody
    {
        public OrderQueryResponse response { get; set; }
    }
    public class OrderQueryResponse
    {
        public OrderQueryContent content { get; set; }
        public int code { get; set; }
    }

    public class OrderQueryContent
    {
        public string code { get; set; }
        public string flag { get; set; }
        public EntryOrder entryOrder { get; set; }
        public string message { get; set; }
    }

    public class EntryOrder
    {
        public string clpsOrderCode { get; set; }
        public int poOrderStatus { get; set; }
        public int storageStatus { get; set; }
    }

    #endregion

    # region ERP创建退供应商订单response类
    public class RtsOrderResponseBody
    {
        public RtsOrderResponse response { get; set; }
    }
    public class RtsOrderResponse
    {
        public RtsOrderContent content { get; set; }
        public int code { get; set; }
    }

    public class RtsOrderContent
    {
        public string clpsRtsNo { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
        public string message { get; set; }
    }

    #endregion

    # region ERP queryRtsOrder response类

    //{"response":{"content":{"code":"1","flag":"success","message":"成功","rtsResults":[{"deliveryMode":"1","isvRtsCode":"201909090001","operatorTime":"2020-04-07 17:05:31","operatorUser":"romensfzl","ownerNo":"CBU8816093026319","receiverInfo":{"email":"数据为null","mobile":"0578-5082404","name":"宋志强测试供应商"},"rtsCode":"CBS4418046753361","rtsDetailList":[{"goodsStatus":"1","itemId":"CMG4418288906048","itemName":"布洛伪麻胶囊(得尔)","itemNo":"00000007","planOutQty":5.0,"planQty":5}],"rtsOrderStatus":"100","serialNumberList":[],"source":"9","supplierNo":"CMS4418046523757","warehouseNo":"800001573"}],"totalLine":1}, "code":0}}
    public class RtsOrderQueryResponseBody
    {
        public RtsOrderQueryResponse response { get; set; }
    }
    public class RtsOrderQueryResponse
    {
        public RtsOrderQueryContent content { get; set; }
        public int code { get; set; }
    }

    public class RtsOrderQueryContent
    {
        public List<rtsResult> rtsResults { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
        public string message { get; set; }
    }

    public class rtsResult
    {
        public string rtsOrderStatus { get; set; }
    }
    #endregion

    # region ERP QueryStock 

    //{"response":{"content":{"code":"1","flag":"success","message":"成功","rtsResults":[{"deliveryMode":"1","isvRtsCode":"201909090001","operatorTime":"2020-04-07 17:05:31","operatorUser":"romensfzl","ownerNo":"CBU8816093026319","receiverInfo":{"email":"数据为null","mobile":"0578-5082404","name":"宋志强测试供应商"},"rtsCode":"CBS4418046753361","rtsDetailList":[{"goodsStatus":"1","itemId":"CMG4418288906048","itemName":"布洛伪麻胶囊(得尔)","itemNo":"00000007","planOutQty":5.0,"planQty":5}],"rtsOrderStatus":"100","serialNumberList":[],"source":"9","supplierNo":"CMS4418046523757","warehouseNo":"800001573"}],"totalLine":1}, "code":0}}
    public class QueryStockResponseBody
    {
        public QueryStockResponse response { get; set; }
    }
    public class QueryStockResponse
    {
        public QueryStockContent content { get; set; }
        public int code { get; set; }
    }

    public class QueryStockContent
    {
        public List<WarehouseStockModel> warehouseStockModelList { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
        public string message { get; set; }
    }

    #endregion

    #region 调拨出库单response类
    public class DBCKDResponseBody
    {
        public DBCKDResponse response { get; set; }
    }
    public class DBCKDResponse
    {
        public DBCKDContent content { get; set; }
        public int code { get; set; }
    }

    public class DBCKDContent
    {
        public string createTime { get; set; }
        public string code { get; set; }
        public string flag { get; set; }
        public string deliveryOrderCode { get; set; }
        public string message { get; set; }
    }

    #endregion
}
