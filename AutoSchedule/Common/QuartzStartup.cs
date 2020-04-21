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
                            client = new JdMessageClient(appKey, appSecret, groupName);
                            client.OnMessage += OnMessageHandler;
                            Console.WriteLine("监听京东Wms，WebSocket连接成功!");
                            client.Connect(wss_url);
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
                _logger.LogInformation("{EventId}:\r\n{result}", "入库完成回传", JsonConvert.SerializeObject(e));
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
                        //出库单配送完成回传
                        StockOutCompleteConfirmFeedbackMsg(e);
                        break;
                    case "checkStockFeedbackMsg":
                        //盘点结果回传
                        CheckStockFeedbackMsg(e);
                        break;
                    case "rtwCompleteFeedbackMsg":
                        //退货入库单完成回传
                        RtwCompleteFeedbackMsg(e);
                        break;
                    case "orderCancelFeedbackMsg":
                        //单据取消异步回传
                        OrderCancelFeedbackMsg(e);
                        break;
                    case "rtsCompleteFeedbackMsg":
                        //退供应商订单完成信息回传
                        RtsCompleteFeedbackMsg(e);
                        break;

                    default:
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
        /// 入库完成回传
        /// </summary>
        /// <param name="e"></param>
        private void StockInFeedbackMsg(JdEventArgs e)
        {
            string billguid = Guid.NewGuid().ToString();
            var stock = JsonConvert.DeserializeObject<StockInFeedbackMsg>(e.message.msgPayload);
            var insertMainCount = _fsql.Insert<STOCKINFEEDBACK_XH_JDWMS>().AppendData(new STOCKINFEEDBACK_XH_JDWMS
            {
                STATUE = "0",
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
        /// 出库完成回传
        /// </summary>
        /// <param name="e"></param>
        private void RtsCompleteFeedbackMsg(JdEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OrderCancelFeedbackMsg(JdEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RtwCompleteFeedbackMsg(JdEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CheckStockFeedbackMsg(JdEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void StockOutCompleteConfirmFeedbackMsg(JdEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 销售订单状态回传
        /// </summary>
        /// <param name="e"></param>
        private void StockOutStatusFeedbackMsg(JdEventArgs e)
        {
            
            var stockOutStatus = JsonConvert.DeserializeObject<StockOutStatusbackMsg>(e.message.msgPayload);

            var countUpdate = _fsql.Update<STOCKOUTFEEDBACK_XH_JDWMS>()
              .Set(a => a.OUTSTATUE, stockOutStatus.soStatus)
              .Where(a => a.DELIVERYORDERID == stockOutStatus.deliveryOrderId)
              .ExecuteAffrows();
           _logger.LogInformation("{EventId}:\r\n{result}", "销售订单状态回传更新状态", JsonConvert.SerializeObject(e));
        }

        /// <summary>
        /// 出库完成回传 
        /// </summary>
        /// <param name="e"></param>
        private void StockOutFeedbackMsg(JdEventArgs e)
        {
            string billguid = Guid.NewGuid().ToString();
            var stock = JsonConvert.DeserializeObject<StockOutFeedbackMsg>(e.message.msgPayload);
            var insertMainCount = _fsql.Insert<STOCKOUTFEEDBACK_XH_JDWMS>().AppendData(new STOCKOUTFEEDBACK_XH_JDWMS
            {
                STATUE = "0",
                DELIVERYORDERCODE = stock.entryOrder.deliveryOrderCode,
                DELIVERYDATE = stock.entryOrder.deliveryDate,
                CONFIRMTYPE = stock.entryOrder.confirmType,
                DELIVERYORDERID = stock.entryOrder.deliveryOrderId,
                DEPTNO = stock.entryOrder.deptNo,
                GUID = billguid,
                OPERATETIME = stock.entryOrder.operateTime,
                OPERATORCODE = stock.entryOrder.operatorCode,
                OPERATORNAME = stock.entryOrder.operatorName,
                ORDERTYPE = stock.entryOrder.orderType,
                SALEPLATFORMORDERCODE = stock.entryOrder.salePlatformOrderCode,
                SALEPLATFORMSOURCECODE = stock.entryOrder.salePlatformOrderCode,
                SELLERNO = stock.entryOrder.sellerNo,
                SHOPNO = stock.entryOrder.shopNo,
                WAREHOUSECODE = stock.entryOrder.warehouseCode,
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
                        SHOPGOODSNO = item.shopGoodsNo
                    });
                }

                var insertDetailCount = _fsql.Insert(stockoutList).ExecuteAffrows();
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
                    _logger.LogInformation("{EventId}:\r\n{result}", "出库完成回传保存数据错误:", JsonConvert.SerializeObject(e));
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
                                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(Second).RepeatForever())//每两秒执行一次
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

                    //改变数据库中的状态
                }
                // _scheduler.ListenerManager.AddJobListener(commonJobListener, GroupMatcher<JobKey>.AnyGroup());
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
                            var closeResult = _scheduler.DeleteJob(rds.Get<JobKey>(param));
                            if (closeResult.Result)
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
                            var closeResult = _scheduler.DeleteJob(jobKeys.GetValueOrDefault(param));
                            if (closeResult.Result)
                            {
                                jobKeys.Remove(param);
                                tk.Status = "0";
                                _SqlLiteContext.TaskPlan.Update(tk);
                                await _SqlLiteContext.SaveChangesAsync();
                                return $"定时任务({ tk.Name})已结束";
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