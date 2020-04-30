using AutoSchedule.Dtos.Data;
using AutoSchedule.Dtos.MessageModel;
using Jdwl.Api.Message;
using Jdwl.Api.Message.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewLife.Caching;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class QuartzStartup
    {

        //jd websocket config
        private string groupName;
        private string appKey;
        private string appSecret;
        private string wss_url;
        private string[] webSocketConfig = { "appKey", "appSecret", "groupName", "wss_url" };
        private List<Dtos.Models.SystemKey> SystemKeys;
        private static JdMessageClient client;
        private static readonly object locker = new object();
        private TaskFactory taskFactory = new TaskFactory();
        private static Dictionary<string, JobKey> jobKeys = new Dictionary<string, JobKey>();
        private readonly ILogger<QuartzStartup> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        public readonly IJobFactory _iocJobfactory;
        private IJobDetail jobDetail;
        private readonly SqlLiteContext _SqlLiteContext;
        public Redis rds;
        private IConfiguration _Configuration;
        private string RedisConnectstring;
        private readonly int RedisDb;
        protected readonly IServiceScope _scope;
        IFreeSql _fsql;
        public QuartzStartup(IJobFactory iocJobfactory, ILogger<QuartzStartup> logger, ISchedulerFactory schedulerFactory, IConfiguration configuration, IServiceProvider serviceProvider, IFreeSql fsql)
        {
            _fsql = fsql;
            this._logger = logger;
            //1、声明一个调度工厂
            this._schedulerFactory = schedulerFactory;
            _Configuration = configuration;
            this._iocJobfactory = iocJobfactory;
            RedisConnectstring = _Configuration.GetConnectionString("RedisConnectstring");
            RedisDb = int.Parse(_Configuration.GetConnectionString("RedisDb"));
            string redispwd = _Configuration.GetConnectionString("RedisPwd");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                rds = new FullRedis(RedisConnectstring, redispwd, RedisDb);
            }
            FullRedis.Register();
            _scope = serviceProvider.CreateScope();
            _SqlLiteContext = _scope.ServiceProvider.GetService(typeof(SqlLiteContext)) as SqlLiteContext;
            SystemKeys = _SqlLiteContext.SystemKeys.AsNoTracking().Where(o => webSocketConfig.Contains(o.KeyName)).ToList();
            groupName = SystemKeys.Where(o => o.KeyName == "groupName").FirstOrDefault().KeyValue;
            appKey = SystemKeys.Where(o => o.KeyName == "appKey").FirstOrDefault().KeyValue;
            appSecret = SystemKeys.Where(o => o.KeyName == "appSecret").FirstOrDefault().KeyValue;
            wss_url = SystemKeys.Where(o => o.KeyName == "wss_url").FirstOrDefault().KeyValue;
            try
            {
                if (client == null)
                {
                    lock (locker)
                    {
                        if (client == null)
                        {
                            if (_Configuration.GetConnectionString("WebSocketStatus") == "1")
                            {
                                client = new JdMessageClient(appKey, appSecret, groupName);
                                client.OnMessage += OnMessageHandler;
                                Console.WriteLine("监听京东Wms，WebSocket连接成功!");
                                client.Connect(wss_url);
                            } 
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PrintStackTrace(e);
            }
        }
        private void OnMessageHandler(object sender, JdEventArgs e)
        {
            try
            {

                //接收消息的业务方法
                ReceiveMsg(e);
            }
            catch (Exception ex)
            {
                //Error occur when do biz method,write log for exception
                PrintStackTrace(ex);

                //消息处理失败回滚，服务端需要重发。
                e.Fail();
            }
        }



        private void ReceiveMsg(JdEventArgs e)
        {
            string messageName = e.message.msgName;
            try
            {
                switch (messageName)
                {
                    case "stockInFeedbackMsg":
                        StockInFeedbackMsg(e);
                        break;
                    case "stockOutFeedbackMsg":
                        StockOutFeedbackMsg(e);
                        break;
                    case "stockOutStatusFeedbackMsg":
                        StockOutStatusFeedbackMsg(e);
                        break;
                    case "stockOutCompleteConfirmFeedbackMsg":
                        StockOutCompleteConfirmFeedbackMsg(e);
                        break;
                    case "checkStockFeedbackMsg":
                        CheckStockFeedbackMsg(e);
                        break;
                    case "rtwCompleteFeedbackMsg":
                        
                        RtwCompleteFeedbackMsg(e);
                        break;
                    case "orderCancelFeedbackMsg":
                        
                        OrderCancelFeedbackMsg(e);
                        break;
                    case "rtsCompleteFeedbackMsg":
                        
                        RtsCompleteFeedbackMsg(e);
                        break;

                    default:
                        _logger.LogInformation("{EventId}:\r\n{result}", "消息回传未定义", JsonConvert.SerializeObject(e));
                        break;
                }
            }
            catch (Exception ex)
            {
                e.Fail();
                _logger.LogInformation("{EventId}:\r\n{result}", "入库完成回传错误,消息", JsonConvert.SerializeObject(ex));
            }

        }
        /// <summary>
        /// 入库完成回传 √
        /// </summary>
        /// <param name="e"></param>
        private void StockInFeedbackMsg(JdEventArgs e)
        {
            _logger.LogInformation("{EventId}:\r\n{result}", "入库完成回传", JsonConvert.SerializeObject(e));
            string billguid = Guid.NewGuid().ToString("N");
            var stock = JsonConvert.DeserializeObject<StockInFeedbackMsg>(e.message.msgPayload);
            var insertMainCount = _fsql.Insert<STOCKINFEEDBACK_XH_JDWMS>().AppendData(new STOCKINFEEDBACK_XH_JDWMS
            {
                ERPSTATUE = "0",
                CLPSORDERCODE = stock.entryOrder.clpsOrderCode,
                BILLNO = stock.entryOrder.entryOrderCode,
                CONFIRMTYPE = stock.entryOrder.confirmType,
                CREATEUSER = stock.entryOrder.createUser,
                GUID = billguid,
                OPERATETIME = stock.entryOrder.operateTime,
                ORDERTYPE = stock.entryOrder.orderType,
                OWNERCODE = stock.entryOrder.ownerCode,
                POORDERSTATUS = stock.entryOrder.poOrderStatus,
                WAREHOUSECODE = stock.entryOrder.warehouseCode
            }).ExecuteAffrows();

            if (insertMainCount != 0)
            {
                List<STOCKINFEEDBACKDETAIL_XH_JDWMS> stockinfeedbackdetailsList = new List<STOCKINFEEDBACKDETAIL_XH_JDWMS>();
                for (int i = 0; i < stock.orderLines.Count; i++)
                {
                    stockinfeedbackdetailsList.Add(new STOCKINFEEDBACKDETAIL_XH_JDWMS
                    {
                        EXPIREDQTY = stock.orderLines[i].expiredQty,
                        GOODSSTATUS = stock.orderLines[i].goodsStatus,
                        SHORTFLOATQTY = stock.orderLines[i].shortFloatQty,
                        DIFFQTY = stock.orderLines[i].diffQty,
                        ITEMNO = stock.orderLines[i].itemNo,
                        SHORTQTY = stock.orderLines[i].shortQty,
                        DAMAGEDQTY = stock.orderLines[i].damagedQty,
                        GUID = Guid.NewGuid().ToString("N"),
                        OTHERDIFFQTY = stock.orderLines[i].otherDiffQty,
                        OTHERDIFFFLOATQTY = stock.orderLines[i].otherDiffFloatQty,
                        MAINGUID = billguid,
                        ORDERLINENO = stock.orderLines[i].orderLineNo,
                        EXPIREDFLOATQTY = stock.orderLines[i].expiredFloatQty,
                        PLANQTY = stock.orderLines[i].planQty,
                        DAMAGEDFLOATQTY = stock.orderLines[i].damagedFloatQty,
                        EMPTYFLOATQTY = stock.orderLines[i].emptyFloatQty,
                        EMPTYQTY = stock.orderLines[i].emptyQty,
                        DIFFFLOATQTY = stock.orderLines[i].diffFloatQty,
                        REALINSTOREFLOATQTY = stock.orderLines[i].realInstoreFloatQty,
                        ITEMID = stock.orderLines[i].itemId,
                        REMARK = stock.orderLines[i].remark,
                        PLANFLOATQTY = stock.orderLines[i].planFloatQty,
                        REALINSTOREQTY = stock.orderLines[i].realInstoreQty,
                        ITEMNAME = stock.orderLines[i].itemName
                    });

                }

                var insertDetailCount = _fsql.Insert(stockinfeedbackdetailsList).ExecuteAffrows();
                if (insertDetailCount == stock.orderLines.Count)
                {
                    //调用存储过程WMS_JDWMS_STOCKINRETURN

                    var BILLGUID = new OracleParameter
                    {
                        ParameterName = "BILLGUID",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Input,
                        Value = billguid,
                        Size = 50
                    };

                    var RETURNMSG = new OracleParameter
                    {
                        ParameterName = "RETURNMSG",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 50
                    };

                    var RETURNVALUE = new OracleParameter
                    {
                        ParameterName = "RETURNVALUE",
                        OracleDbType = OracleDbType.Decimal,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 10
                    };

                    _fsql.Ado.ExecuteNonQuery(CommandType.StoredProcedure, "WMS_JDWMS_STOCKINRETURN", BILLGUID, RETURNMSG, RETURNVALUE);
                    if (RETURNVALUE.Value.ToString() != "0")
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "入库完成回传保存数据,执行存储过程错误", RETURNMSG.Value);
                    }
                    else
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "入库完成回传保存数据,执行存储过程WMS_JDWMS_STOCKINRETURN,返回:", RETURNMSG.Value);
                    }
                }
                else
                {
                    _logger.LogInformation("{EventId}:\r\n{result}", "入库完成回传保存数据错误:", JsonConvert.SerializeObject(e));
                }
            }
            else
            {
                _logger.LogInformation("{EventId}:\r\n{result}", "入库完成回传保存主表错误", JsonConvert.SerializeObject(e));
            }
        }


        /// <summary>
        /// 退供应商订单完成信息回传  √
        /// </summary>
        /// <param name="e"></param>
        private void RtsCompleteFeedbackMsg(JdEventArgs e)
        {
            //{"rtsOrderModel":{"rtsOrderCode":"202004280001","rtsOrderId":"CBS4418046830359","rtsType":"1","ownerNo":"CBU8816093026319","deliveryMode":"1","warehouseNo":"800001573","supplierNo":"CMS4418046523757","supplierName":"宋志强测试供应商","receiverInfo":{"name":"宋志强测试供应商","mobile":"","email":"数据为null","detailAddress":"天津市东丽区机场北路丰树天津空港物流园"},"orderCreateTime":"2020-04-28 09:53:13","outBizCode":"617faed3-6f7e-4dd2-b33b-1e3f3fae717d","operateTime":"2020-04-28 10:39:44","orderConfirmTime":"2020-04-28 10:39:44"},"rtsItemModelList":[{"lotInfoList":[],"itemCode":"00000007","itemId":"CMG4418288906048","planQty":1,"actualQty":1,"planOutQty":1.0,"actualOutQty":1.0}]}
            _logger.LogInformation("{EventId}:\r\n{result}", "退供应商订单回传", JsonConvert.SerializeObject(e));
            string billguid = Guid.NewGuid().ToString("N");
            var stock = JsonConvert.DeserializeObject<RtsCompleteFeedbackMsg>(e.message.msgPayload);
            var insertMainCount = _fsql.Insert<RTSBACK_XH_JDWMS>().AppendData(new RTSBACK_XH_JDWMS
            {
                ERPSTATUE = "0",
                DELIVERYMODE = stock.rtsOrderModel.deliveryMode,
                ORDERCREATETIME = stock.rtsOrderModel.orderCreateTime,
                RTSORDERCODE = stock.rtsOrderModel.rtsOrderCode,
                RTSORDERID = stock.rtsOrderModel.rtsOrderId,
                RTSTYPE = stock.rtsOrderModel.rtsType,
                GUID = billguid,
                SUPPLIERNAME = stock.rtsOrderModel.supplierName,
                OWNERNO = stock.rtsOrderModel.ownerNo,
                REMARK = stock.rtsOrderModel.remark,
                SUPPLIERNO = stock.rtsOrderModel.supplierNo,
                WAREHOUSENO = stock.rtsOrderModel.warehouseNo
            }).ExecuteAffrows();

            if (insertMainCount != 0)
            {
                List<RTSBACKDETAIL_XH_JDWMS> rtwbacks = new List<RTSBACKDETAIL_XH_JDWMS>();

                foreach (var item in stock.rtsItemModelList)
                {
                    rtwbacks.Add(new RTSBACKDETAIL_XH_JDWMS
                    {
                        ACTUALOUTQTY = item.actualOutQty,
                        ACTUALQTY = item.actualQty,
                        EXPIREDATE = item.expireDate,
                        ITEMCODE = item.itemCode,
                        ORDERSOURCECODE = item.orderSourceCode,
                        OWNERCODE = item.ownerCode,
                        BATCHCODE = item.batchCode,
                        SUBSOURCECODE = item.subSourceCode,
                        INVENTORYTYPE = item.inventoryType,
                        PLANOUTQTY = item.planOutQty,
                        PRODUCECODE = item.produceCode,
                        PRODUCTDATE = item.productDate,
                        QRCODE = item.qrCode,
                        GUID = Guid.NewGuid().ToString("N"),
                        ITEMID = item.itemId,
                        MAINGUID = billguid,
                        ORDERLINENO = item.orderLineNo,
                        PLANQTY = item.planQty
                    });
                }

                var insertDetailCount = _fsql.Insert(rtwbacks).ExecuteAffrows();
                if (insertDetailCount == stock.rtsItemModelList.Count)
                {
                    //调用存储过程WMS_JDWMS_RTSBACK
                    var BILLGUID = new OracleParameter
                    {
                        ParameterName = "BILLGUID",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Input,
                        Value = billguid,
                        Size = 50
                    };

                    var RETURNMSG = new OracleParameter
                    {
                        ParameterName = "RETURNMSG",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 50
                    };

                    var RETURNVALUE = new OracleParameter
                    {
                        ParameterName = "RETURNVALUE",
                        OracleDbType = OracleDbType.Decimal,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 10
                    };

                    _fsql.Ado.ExecuteNonQuery(CommandType.StoredProcedure, "WMS_JDWMS_RTSBACK", BILLGUID, RETURNMSG, RETURNVALUE);
                    if (RETURNVALUE.Value.ToString() != "0")
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "退供应商订单保存数据,执行存储过程错误", RETURNMSG.Value);
                    }
                    else
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "退供应商订单保存数据, 执行存储过程WMS_JDWMS_RTSBACK,返回:", RETURNMSG.Value);
                    }
                }
                else
                {
                    _logger.LogInformation("{EventId}:\r\n{result}", "退供应商订单保存数据错误:", JsonConvert.SerializeObject(e));
                }
            }
            else
            {
                _logger.LogInformation("{EventId}:\r\n{result}", "退供应商订单回传保存主表错误", JsonConvert.SerializeObject(e));
            }
        }

        /// <summary>
        /// 单据取消异步回传 √
        /// </summary>
        /// <param name="e"></param>
        private void OrderCancelFeedbackMsg(JdEventArgs e)
        {
            //{"status":"1","message":"取消成功","orderCode":"202004280001","orderId":"CPL4418048123702","orderType":"CGRK"}
            _logger.LogInformation("{EventId}:\r\n{result}", "单据取消异步回传", JsonConvert.SerializeObject(e));
            string billguid = Guid.NewGuid().ToString("N");
            var stock = JsonConvert.DeserializeObject<OrderCancelFeedbackMsg>(e.message.msgPayload);
            var insertMainCount = _fsql.Insert<ORDERCANCELBACK_XH_JDWMS>().AppendData(new ORDERCANCELBACK_XH_JDWMS
            {
                ERPSTATUE = "0",
                ORDERTYPE = stock.orderType,
                FAILUREREASON = stock.failureReason,
                MESSAGE = stock.message,
                ORDERCODE = stock.orderCode,
                ORDERID = stock.orderId,
                STATUS = stock.status,
                GUID = billguid
            }).ExecuteAffrows();

        }

        /// <summary>
        /// 退货入库单完成回传
        /// </summary>
        /// <param name="e"></param>
        private void RtwCompleteFeedbackMsg(JdEventArgs e)
        {
            _logger.LogInformation("{EventId}:\r\n{result}", "退货入库回传", JsonConvert.SerializeObject(e));
            string billguid = Guid.NewGuid().ToString("N");
            var stock = JsonConvert.DeserializeObject<RtwCompleteFeedbackMsg>(e.message.msgPayload);
            var insertMainCount = _fsql.Insert<RTWBACK_XH_JDWMS>().AppendData(new RTWBACK_XH_JDWMS
            {
                ERPSTATUE = "0",
                BIZTYPE = stock.rtwOrderModel.bizType,
                EXPRESSCODE = stock.rtwOrderModel.expressCode,
                LOGISTICSCODE = stock.rtwOrderModel.logisticsCode,
                LOGISTICSNAME = stock.rtwOrderModel.logisticsName,
                ORDERCONFIRMTIME = stock.rtwOrderModel.orderConfirmTime,
                GUID = billguid,
                OUTBIZCODE = stock.rtwOrderModel.outBizCode,
                OWNERNO = stock.rtwOrderModel.ownerNo,
                REMARK = stock.rtwOrderModel.remark,
                ORDERTYPE = stock.rtwOrderModel.orderType,
                RETURNREASON = stock.rtwOrderModel.returnReason,
                RTWORDERCODE = stock.rtwOrderModel.rtwOrderCode,
                RTWORDERID = stock.rtwOrderModel.rtwOrderId,
                RTWSTATUS = stock.rtwOrderModel.rtwStatus,
                WAREHOUSECODE = stock.rtwOrderModel.warehouseCode
            }).ExecuteAffrows();

            if (insertMainCount != 0)
            {
                List<RTWBACKDETAIL_XH_JDWMS> rtwbacks = new List<RTWBACKDETAIL_XH_JDWMS>();

                foreach (var item in stock.rtwOrderItemList)
                {
                    rtwbacks.Add(new RTWBACKDETAIL_XH_JDWMS
                    {
                        ACTUALOUTQTY = item.actualOutQty,
                        ACTUALQTY = item.actualQty,
                        EXPIREDATE = item.expireDate,
                        ITEMCODE = item.itemCode,
                        ORDERSOURCECODE = item.orderSourceCode,
                        OWNERCODE = item.ownerCode,
                        BATCHCODE = item.batchCode,
                        SUBSOURCECODE = item.subSourceCode,
                        INVENTORYTYPE = item.inventoryType,
                        PLANOUTQTY = item.planOutQty,
                        PRODUCECODE = item.produceCode,
                        PRODUCTDATE = item.productDate,
                        QRCODE = item.qrCode,
                        GUID = Guid.NewGuid().ToString("N"),
                        ITEMID = item.itemId,
                        MAINGUID = billguid,
                        ORDERLINENO = item.orderLineNo,
                        PLANQTY = item.planQty
                    });
                }

                var insertDetailCount = _fsql.Insert(rtwbacks).ExecuteAffrows();
                if (insertDetailCount == stock.rtwOrderItemList.Count)
                {
                    //调用存储过程WMS_JDWMS_RTWBACK
                    var BILLGUID = new OracleParameter
                    {
                        ParameterName = "BILLGUID",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Input,
                        Value = billguid,
                        Size = 50
                    };

                    var RETURNMSG = new OracleParameter
                    {
                        ParameterName = "RETURNMSG",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 50
                    };

                    var RETURNVALUE = new OracleParameter
                    {
                        ParameterName = "RETURNVALUE",
                        OracleDbType = OracleDbType.Decimal,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 10
                    };

                    _fsql.Ado.ExecuteNonQuery(CommandType.StoredProcedure, "WMS_JDWMS_RTWBACK", BILLGUID, RETURNMSG, RETURNVALUE);
                    if (RETURNVALUE.Value.ToString() != "0")
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "退货入库回传保存数据,执行存储过程错误", RETURNMSG.Value);
                    }
                    else
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "退货入库回传保存数据,执行存储过程WMS_JDWMS_STOCKOUT,返回:", RETURNMSG.Value);
                    }
                }
                else
                {
                    _logger.LogInformation("{EventId}:\r\n{result}", "退货入库回传保存数据错误:", JsonConvert.SerializeObject(e));
                }
            }
            else
            {
                _logger.LogInformation("{EventId}:\r\n{result}", "退货入库回传保存主表错误", JsonConvert.SerializeObject(e));
            }
        }

        /// <summary>
        /// 盘点结果回传
        /// </summary>
        /// <param name="e"></param>
        private void CheckStockFeedbackMsg(JdEventArgs e)
        {
            _logger.LogInformation("{EventId}:\r\n{result}", "盘点结果回传", JsonConvert.SerializeObject(e));
            string billguid = Guid.NewGuid().ToString("N");
            var stockCheck = JsonConvert.DeserializeObject<CheckStockFeedbackMsg>(e.message.msgPayload);
            var insertMainCount = _fsql.Insert<CHECKSTOCKBACK_XH_JDWMS>().AppendData(new CHECKSTOCKBACK_XH_JDWMS
            {
                ERPSTATUE = "0",
                CHECKORDERCODE = stockCheck.checkOrderCode,
                CHECKORDERID = stockCheck.checkOrderId,
                CHECKTIME = stockCheck.checkTime,
                OUTBIZCODE = stockCheck.outBizCode,
                OWNERCODE = stockCheck.ownerCode,
                GUID = billguid,
                REMARK = stockCheck.remark,
                WAREHOUSECODE = stockCheck.warehouseCode
            }).ExecuteAffrows();

            if (insertMainCount != 0)
            {
                List<CHECKSTOCKBACKDETAIL_XH_JDWMS> checkStocks = new List<CHECKSTOCKBACKDETAIL_XH_JDWMS>();

                foreach (var item in stockCheck.checkStockItemList)
                {
                    checkStocks.Add(new CHECKSTOCKBACKDETAIL_XH_JDWMS
                    {
                        GUID = Guid.NewGuid().ToString("N"),
                        ITEMID = item.itemId,
                        INVENTORYTYPE = item.inventoryType,
                        ITEMCODE = item.itemCode,
                        MAINGUID = billguid,
                        QUANTITY = item.quantity,
                        QUANTITYVALUE = item.quantityValue
                    });
                }

                var insertDetailCount = _fsql.Insert(checkStocks).ExecuteAffrows();
                if (insertDetailCount == stockCheck.checkStockItemList.Count)
                {
                    //调用存储过程WMS_JDWMS_CHECKSTOCK
                    var BILLGUID = new OracleParameter
                    {
                        ParameterName = "BILLGUID",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Input,
                        Value = billguid,
                        Size = 50
                    };

                    var RETURNMSG = new OracleParameter
                    {
                        ParameterName = "RETURNMSG",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 50
                    };

                    var RETURNVALUE = new OracleParameter
                    {
                        ParameterName = "RETURNVALUE",
                        OracleDbType = OracleDbType.Decimal,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 10
                    };

                    _fsql.Ado.ExecuteNonQuery(CommandType.StoredProcedure, "WMS_JDWMS_CHECKSTOCK", BILLGUID, RETURNMSG, RETURNVALUE);
                    if (RETURNVALUE.Value.ToString() != "0")
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "盘点结果回传保存数据,执行存储过程错误", RETURNMSG.Value);
                    }
                    else
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "盘点结果回传保存数据,执行存储过程WMS_JDWMS_CHECKSTOCK,返回:", RETURNMSG.Value);
                    }
                }
                else
                {
                    _logger.LogInformation("{EventId}:\r\n{result}", "盘点结果回传保存数据错误:", JsonConvert.SerializeObject(e));
                }
            }
            else
            {
                _logger.LogInformation("{EventId}:\r\n{result}", "盘点结果回传保存主表错误", JsonConvert.SerializeObject(e));
            }
        }

        /// <summary>
        /// 出库单配送完成回传，在表中保存拒收信息和接受信息，调一下配送完成存储过程
        /// </summary>
        /// <param name="e"></param>
        private void StockOutCompleteConfirmFeedbackMsg(JdEventArgs e)
        {
            _logger.LogInformation("{EventId}:\r\n{result}", "出库单配送完成回传", JsonConvert.SerializeObject(e));
            string billguid = Guid.NewGuid().ToString("N");
            var stock = JsonConvert.DeserializeObject<stockOutCompleteConfirmBackMsg>(e.message.msgPayload);
            var insertMainCount = _fsql.Insert<STOCKOUTCOMPLETE_XH_JDWMS>().AppendData(new STOCKOUTCOMPLETE_XH_JDWMS
            {
                ERPSTATUE = stock.status,
                DELIVERYORDERCODE = stock.deliveryOrderCode,
                DELIVERYORDERID = stock.deliveryOrderId,
                OPERATEUSER = stock.operateUser,
                WMSSTATUS = "0",
                GUID = billguid,
                OPERATETIME = stock.operateTime
            }).ExecuteAffrows();

            if (insertMainCount != 0)
            {
                List<STOCKOUTPAYMETHOD_XH_JDWMS> paymethods = new List<STOCKOUTPAYMETHOD_XH_JDWMS>();
                List<STOCKOUTCOMDETAIL_XH_JDWMS> details = new List<STOCKOUTCOMDETAIL_XH_JDWMS>();

                foreach (var item in stock.payMethodList)
                {
                    paymethods.Add(new STOCKOUTPAYMETHOD_XH_JDWMS { 
                        AMOUNT = item.amount,
                        GUID = Guid.NewGuid().ToString("N"),
                        MAINGUID =billguid ,
                        METHOD = item.method
                    });
                }

                foreach (var item in stock.rejectGoodsItemList)
                {
                    details.Add(new STOCKOUTCOMDETAIL_XH_JDWMS
                    {
                        ITEMCODE = item.itemCode,
                        GUID = Guid.NewGuid().ToString("N"),
                        ITEMID = item.itemId,
                        ITEMNAME = item.itemName,
                        MAINGUID =billguid ,
                        QUANTITY = item.quantity,
                        TYPE = "1"
                    });
                }

                foreach (var item in stock.receiveGoodsItemList)
                {
                    details.Add(new STOCKOUTCOMDETAIL_XH_JDWMS
                    {
                        ITEMCODE = item.itemCode,
                        GUID = Guid.NewGuid().ToString("N"),
                        ITEMID = item.itemId,
                        ITEMNAME = item.itemName,
                        MAINGUID = billguid,
                        QUANTITY = item.quantity,
                        TYPE = "0"
                    });
                }


                var insertDetailCount = _fsql.Insert(details).ExecuteAffrows();
                var insertPMCount = _fsql.Insert(paymethods).ExecuteAffrows();
                if (insertDetailCount+ insertPMCount == stock.receiveGoodsItemList.Count+ stock.rejectGoodsItemList.Count + stock.payMethodList.Count)
                {
                    //调用存储过程WMS_JDWMS_STOCKINRETURN

                    var BILLGUID = new OracleParameter
                    {
                        ParameterName = "BILLGUID",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Input,
                        Value = billguid,
                        Size = 50
                    };

                    var RETURNMSG = new OracleParameter
                    {
                        ParameterName = "RETURNMSG",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 50
                    };

                    var RETURNVALUE = new OracleParameter
                    {
                        ParameterName = "RETURNVALUE",
                        OracleDbType = OracleDbType.Decimal,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 10
                    };

                    _fsql.Ado.ExecuteNonQuery(CommandType.StoredProcedure, "WMS_JDWMS_STOCKOUTCOM", BILLGUID, RETURNMSG, RETURNVALUE);
                    if (RETURNVALUE.Value.ToString() != "0")
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "出库单配送完成回传,执行存储过程错误", RETURNMSG.Value);
                    }
                    else
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "出库单配送完成回传,执行存储过程WMS_JDWMS_STOCKOUTCOM,返回:", RETURNMSG.Value);
                    }
                }
                else
                {
                    _logger.LogInformation("{EventId}:\r\n{result}", "出库单配送完成回传保存数据错误:", JsonConvert.SerializeObject(e));
                }
            }
            else
            {
                _logger.LogInformation("{EventId}:\r\n{result}", "出库单配送完成回传保存主表错误", JsonConvert.SerializeObject(e));
            }
        }

        /// <summary>
        /// 销售订单状态回传  √
        /// </summary>
        /// <param name="e"></param>
        private void StockOutStatusFeedbackMsg(JdEventArgs e)
        {
            //{"status":100153,"deliveryOrderCode":"202004240001","operateTime":"2020-04-26 11:12:04","operateUser":"orderAccept","deliveryOrderId":"CSL8816306781203","warehouseCode":"800001573","vmiType":false,"ownerCode":"CBU8816093026319"}
            _logger.LogInformation("{EventId}:\r\n{result}", "销售订单状态回传", JsonConvert.SerializeObject(e));
            var stockOutStatus = JsonConvert.DeserializeObject<StockOutStatusbackMsg>(e.message.msgPayload);

            var countUpdate = _fsql.Update<STOCKOUTFEEDBACK_XH_JDWMS>()
              .Set(a => a.OUTSTATUE, stockOutStatus.status)
              .Where(a => a.DELIVERYORDERID == stockOutStatus.deliveryOrderId)
              .ExecuteAffrows();
        }

        /// <summary>
        /// 出库完成回传   √
        /// </summary>
        /// <param name="e"></param>
        private void StockOutFeedbackMsg(JdEventArgs e)
        {
            _logger.LogInformation("{EventId}:\r\n{result}", "出库完成回传", JsonConvert.SerializeObject(e));
            string billguid = Guid.NewGuid().ToString("N");
            var stock = JsonConvert.DeserializeObject<StockOutFeedbackMsg>(e.message.msgPayload);
            var insertMainCount = _fsql.Insert<STOCKOUTFEEDBACK_XH_JDWMS>().AppendData(new STOCKOUTFEEDBACK_XH_JDWMS
            {
                ERPSTATUE = "0",
                DELIVERYORDERCODE = stock.deliveryOrder.deliveryOrderCode,
                DELIVERYDATE = stock.deliveryOrder.deliveryDate,
                CONFIRMTYPE = stock.deliveryOrder.confirmType,
                DELIVERYORDERID = stock.deliveryOrder.deliveryOrderId,
                DEPTNO = stock.deliveryOrder.deptNo,
                GUID = billguid,
                OPERATETIME = stock.deliveryOrder.operateTime,
                OPERATORCODE = stock.deliveryOrder.operatorCode,
                OPERATORNAME = stock.deliveryOrder.operatorName,
                ORDERTYPE = stock.deliveryOrder.orderType,
                RECEIVENAME = stock.deliveryOrder.receiverInfo.Name,
                RECEIVEMOBILE = stock.deliveryOrder.receiverInfo.Mobile,
                ORDERCONFIRMTIME = stock.deliveryOrder.orderConfirmTime,
                SELLERNO = stock.deliveryOrder.sellerNo,
                SHOPNO = stock.deliveryOrder.shopNo,
                WAREHOUSECODE = stock.deliveryOrder.warehouseCode,
                RECEIVEADDR = stock.deliveryOrder.receiverInfo.DetailAddress
            }).ExecuteAffrows();

            if (insertMainCount != 0)
            {
                List<STOCKOUTBACKDETAIL_XH_JDWMS> stockoutList = new List<STOCKOUTBACKDETAIL_XH_JDWMS>();

                foreach (var item in stock.orderLines)
                {
                    stockoutList.Add(new STOCKOUTBACKDETAIL_XH_JDWMS
                    {
                        ACTUALOUTQTY = item.actualOutQty,
                        ACTUALQTY = item.actualQty,
                        INVENTORYTYPE = item.inventoryType,
                        ITEMCODE = item.itemCode,
                        ORDERSOURCECODE = item.orderSourceCode,
                        OWNERCODE = item.ownerCode,
                        SHOPGOODSNO = item.shopGoodsNo,
                        GUID = Guid.NewGuid().ToString("N"),
                        ITEMID = item.itemId,
                        MAINGUID  =billguid ,
                        ORDERLINENO = item.orderLineNo,
                        PLANQTY = item.planQty,
                        PLANOUTQTY = item.planOutQty
                    });
                }
                List<STOCKOUTBACKLOT_XH_JDWMS> lotlist = new List<STOCKOUTBACKLOT_XH_JDWMS>();
                foreach (var item in stock.lotInfoList)
                {
                    lotlist.Add(new STOCKOUTBACKLOT_XH_JDWMS { 
                        GOODSNO = item.goodsNo,
                        BATCHQTY = item.batchQty,
                        GUID = Guid.NewGuid().ToString("N"), 
                        MAINGUID = billguid,
                        ORDERLINE = item.orderLine
                    });
                }

                var insertDetailCount = _fsql.Insert(stockoutList).ExecuteAffrows();
                insertDetailCount = _fsql.Insert(lotlist).ExecuteAffrows();
                    //调用存储过程WMS_JDWMS_STOCKINRETURN
                    var BILLGUID = new OracleParameter
                    {
                        ParameterName = "BILLGUID",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Input,
                        Value = billguid,
                        Size = 50
                    };

                    var RETURNMSG = new OracleParameter
                    {
                        ParameterName = "RETURNMSG",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 50
                    };

                    var RETURNVALUE = new OracleParameter
                    {
                        ParameterName = "RETURNVALUE",
                        OracleDbType = OracleDbType.Decimal,
                        Direction = ParameterDirection.Output,
                        Value = "",
                        Size = 10
                    };

                    _fsql.Ado.ExecuteNonQuery(CommandType.StoredProcedure, "WMS_JDWMS_STOCKOUT", BILLGUID, RETURNMSG, RETURNVALUE);
                    if (RETURNVALUE.Value.ToString() != "0")
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "出库完成回传保存数据,执行存储过程错误", RETURNMSG.Value);
                    }
                    else
                    {
                        _logger.LogInformation("{EventId}:\r\n{result}", "出库完成回传保存数据,执行存储过程WMS_JDWMS_STOCKOUT,返回:", RETURNMSG.Value);
                    }

            }
            else
            {
                _logger.LogInformation("{EventId}:\r\n{result}", "出库完成回传保存主表错误", JsonConvert.SerializeObject(e));
            }
        }

        internal static void PrintStackTrace(Exception e)
        {
            Console.WriteLine($"Exception Type {e.GetType().Name}, Message:{e.Message},Stack:{e.StackTrace}");
            if (e.InnerException != null)
            {
                Console.Write(" inner Exception: ");
                PrintStackTrace(e.InnerException);
            }
        }
        public async Task<string> Start(List<string> param)
        {
            try
            {
                int Second = 0;
                if (param.Count > 1 && _scheduler != null)
                {
                    await Stop();
                }
                for (int i = 0; i < param.Count; i++)
                {

                    //计算出事件的秒数
                    var ts = await _SqlLiteContext.TaskPlan.FirstOrDefaultAsync(o => o.GUID == param[i].ToString());
                    switch (ts.FrequencyType)
                    {
                        case "0":
                            Second = int.Parse(ts.Frequency);
                            break;

                        case "1":
                            Second = int.Parse(ts.Frequency) * 60;
                            break;

                        case "2":
                            Second = int.Parse(ts.Frequency) * 3600;
                            break;

                        default:
                            break;
                    }

                    //2、通过调度工厂获得调度器
                    _scheduler = await _schedulerFactory.GetScheduler();
                    _scheduler.JobFactory = this._iocJobfactory;
                    //  替换默认工厂
                    //3、开启调度器
                    _logger.LogInformation("定时任务({EventId})启动", ts.Name);
                    await _scheduler.Start();
                    //4、创建一个触发器
                    var trigger = TriggerBuilder.Create()
                                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(Second).RepeatForever())
                                    .Build();
                    //5、创建任务 0是dll 模式 1是api模式
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        if (rds.ContainsKey(param[i].ToString()))
                        {
                            return await Task.FromResult($"已经开启过任务{ts.Name}不允许重复开启！");
                        }

                        if (ts.WorkType == "0")
                        {
                            jobDetail = JobBuilder.Create<AutoTaskJobDll>()
                                       .WithIdentity(param[i].ToString(), "group")
                                       .UsingJobData("guid", param[i].ToString())
                                       .Build();

                        }
                        else
                        {
                            jobDetail = JobBuilder.Create<AutoTaskJob>()
                                       .WithIdentity(param[i].ToString(), "group")
                                       .UsingJobData("guid", param[i].ToString())
                                       .Build();
                        }

                        rds.Set(param[i].ToString(), jobDetail.Key);
                    }
                    else
                    {
                        if (ts.Status == "1" || jobKeys.ContainsKey(param[i].ToString()))
                        {
                            return await Task.FromResult($"已经开启过任务{ts.Name}不允许重复开启！");
                        }
                        else
                        {

                            if (ts.WorkType == "0")
                            {
                                jobDetail = JobBuilder.Create<AutoTaskJobDll>()
                                           .WithIdentity(param[i].ToString(), "group")
                                           .UsingJobData("guid", param[i].ToString())
                                           .Build();
                            }
                            else
                            {
                                jobDetail = JobBuilder.Create<AutoTaskJob>()
                                           .WithIdentity(param[i].ToString(), "group")
                                           .UsingJobData("guid", param[i].ToString())
                                           .Build();
                            }

                            ts.Status = "1";
                            _SqlLiteContext.TaskPlan.Update(ts);
                            await _SqlLiteContext.SaveChangesAsync();
                            jobKeys.Add(param[i].ToString(), jobDetail.Key);
                        }

                    }
                    //6、将触发器和任务器绑定到调度器中 

                    await _scheduler.ScheduleJob(jobDetail, trigger);

                }
                return await Task.FromResult("0");
            }
            catch (Exception ex)
            {
                _logger.LogError($"开启失败，失败原因:{ex.Message}");
                return await Task.FromResult($"开启失败，失败原因:{ex.Message}");
            }
        }

        public async Task<string> Stop(string param = "")
        {
            try
            {
                var taskPlands = await _SqlLiteContext.TaskPlan.ToListAsync();
                var tk = await _SqlLiteContext.TaskPlan.Where(o => o.GUID == param).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(param))
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        if (rds.ContainsKey(param))
                        {
                            if (await _scheduler.DeleteJob(rds.Get<JobKey>(param)))
                            {
                                rds.Remove(param);
                                return $"定时任务({ tk.Name})已结束";
                            }

                        }
                        else
                        {
                            return "还未开始，怎谈得上关闭呢？";
                        }
                    }
                    else
                    {
                        if (tk.Status == "1" && jobKeys.ContainsKey(param))
                        {
                            if (await _scheduler.DeleteJob(jobKeys.GetValueOrDefault(param)))
                            {
                                jobKeys.Remove(param);
                                tk.Status = "0";
                                _SqlLiteContext.TaskPlan.Update(tk);
                                await _SqlLiteContext.SaveChangesAsync();
                                return await Task.FromResult($"定时任务({ tk.Name})已结束");
                            }

                        }
                        else
                        {
                            tk.Status = "0";
                            _SqlLiteContext.TaskPlan.Update(tk);
                            await _SqlLiteContext.SaveChangesAsync();
                            return "还未开始，怎谈得上关闭呢？";
                        }

                    }




                }
                await _scheduler.Shutdown();

                for (int i = 0; i < taskPlands.Count; i++)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        rds.Remove(taskPlands[i].GUID);
                    }
                    else
                    {
                        jobKeys.Remove(taskPlands[i].GUID);
                        taskPlands[i].Status = "0";
                        _SqlLiteContext.TaskPlan.Update(taskPlands[i]);
                        await _SqlLiteContext.SaveChangesAsync();
                    }
                }

                return "定时任务已全部结束";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }

    }
}