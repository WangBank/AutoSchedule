using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jdwl.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewLife.Serialization;
using Newtonsoft.Json;

namespace AutoSchedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManualTranController : ControllerBase
    {
        public string JdWmsApi(string billno)
        {
            string result;
            try
            {
                #region  销售订单
                //IJdClient client;
                //client = new DefaultJdClient("https://api.jdwl.com/routerjson", "597f6671b2d34d448c7d10084f596fc7", "fa320743b62d4caca797eae731553065", "5b1c8cf159fd423087138f2f8d8286c1");

                ////dsData[i].DataDetail[0] the i is detail what developer defined

                //var OrderLines = new List<Jdwl.Api.Domain.Clps.ClpsOpenGwService.SoOrderLine>();

                //OrderLines.Add(new Jdwl.Api.Domain.Clps.ClpsOpenGwService.SoOrderLine
                //{
                //        OrderLineNo = "1",
                //        ItemCode = "00000001",
                //        PlanQty = 1
                //    });
                //OrderLines.Add(new Jdwl.Api.Domain.Clps.ClpsOpenGwService.SoOrderLine
                //{
                //    OrderLineNo = "2",
                //    ItemCode = "00000336",
                //    PlanQty = 1
                //});

                //var request = new Jdwl.Api.Request.Clps.ClpsTransportSoOrderLopRequest
                //{
                //    Pin = "romensfzl",
                //    Request = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.SoCreateRequest
                //    {
                //        DeliveryOrder = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.SoDeliveryOrder
                //        {
                //            DeliveryOrderCode = "202004270002",
                //            IsvSource = "ISV0020000000352",
                //            //京东那边的供应商编码
                //            SoType = "1",
                //            WarehouseCode = "800001573",
                //            OrderMark = "0",
                //            SourcePlatformCode = "1",
                //            OwnerNo = "CBU8816093026319",
                //            ShopNo = "CSP0020000045005",
                //            LogisticsCode = "CYF4418046511145",
                //            ReceiverInfo = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.ReceiverInfo
                //            {
                //                Name = "wangyinhang",
                //                Mobile = "17878978987",
                //                DetailAddress = "在山的那边"
                //            }

                //        },
                //        OrderLines = OrderLines
                //    }
                //};
                //var jsonrequest = JsonConvert.SerializeObject(request,new JsonSerializerSettings {
                //    NullValueHandling = NullValueHandling.Ignore
                //});
                ////{"response":{"content":{"code":"1","createTime":"2020-04-03 13:40:51","entryOrderCode":"CPL4418047893011","flag":"success","message":"成功"}, "code":0}}
                //Jdwl.Api.Response.Clps.ClpsTransportSoOrderResponse returnValue = client.Execute(request);

                //return returnValue.ToJson(); 
                #endregion


                #region 退货入库单
                IJdClient client;
                client = new DefaultJdClient("https://api.jdwl.com/routerjson", "597f6671b2d34d448c7d10084f596fc7", "fa320743b62d4caca797eae731553065", "5b1c8cf159fd423087138f2f8d8286c1");

                //dsData[i].DataDetail[0] the i is detail what developer defined

                var OrderLines = new List<Jdwl.Api.Domain.Clps.ClpsOpenGwService.RtwOrderItem>();

                OrderLines.Add(new Jdwl.Api.Domain.Clps.ClpsOpenGwService.RtwOrderItem
                {
                    ItemCode = "00000001",
                    ItemStatus = 1,
                    ItemQty = 1
                });
                var request = new Jdwl.Api.Request.Clps.ClpsAddRtwOrderLopRequest
                {
                    Pin = "romensfzl",
                    RtwOrderRequest = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.RtwOrderRequest
                    {
                        SenderInfo = new Jdwl.Api.Domain.Clps.ClpsOpenGwService.SenderInfo
                        {
                            Name = "wangyinhang",
                            Mobile = "17878978987",
                            DetailAddress = "在山的那边"
                        },
                        RtwType = 3,
                        RtwOrderType = 1,
                        WaybillCode = "yd202005060001",
                        OutRtwCode = "202004270002",
                        WarehouseCode = "800001573",
                        OwnerCode = "CBU8816093026319",
                        LogisticsCode = "CYF4418046511145",
                        RtwOrderItemList = OrderLines
                    }
                };
                var jsonrequest = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                //{"response":{"content":{"code":"1","createTime":"2020-04-03 13:40:51","entryOrderCode":"CPL4418047893011","flag":"success","message":"成功"}, "code":0}}
                Jdwl.Api.Response.Clps.ClpsAddRtwOrderResponse returnValue = client.Execute(request);

                return returnValue.ToJson(); 
                #endregion
            }
            catch (Exception ex)
            {

                result = "推送采购订单信息失败" + ex.Message;
                return result;
            }

        }
    }
}