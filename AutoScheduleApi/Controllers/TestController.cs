using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoSchedule.Dtos.MessageModel;
using BankDbHelper;
using Jdwl.Api.Message.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

namespace AutoScheduleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost]
        public string StockInMessageInput(BankTest e)
        {
           // 入库完成回传,插入数据到临时表STOCKINFEEDBACK_XH_JDWMS中
             string billguid = Guid.NewGuid().ToString();
            ArrayList insertsqls = new ArrayList();
            StockInFeedbackMsg stock = JsonConvert.DeserializeObject<StockInFeedbackMsg>(e.message.msgPayload);
          string conn = "User Id=dbo;Password=romens;Data Source=192.168.100.9:1521/NewStddata;";
            BankDbHelper.SqlHelper sqlHelper = new BankDbHelper.SqlHelper(BankDbHelper.DBTypeEnum.Oracle.ToString(), conn);
            string insertMain = $"insert into STOCKINFEEDBACK_XH_JDWMS(GUID,CLPSORDERCODE,CREATEUSER,POORDERSTATUS,WAREHOUSECODE,BILLNO,CONFIRMTYPE,OPERATETIME,ORDERTYPE,OWNERCODE) values('{billguid}','{stock.entryOrder.clpsOrderCode}','{stock.entryOrder.createUser}','{stock.entryOrder.poOrderStatus}','{stock.entryOrder.warehouseCode}','{stock.entryOrder.entryOrderCode}','{stock.entryOrder.confirmType}','{stock.entryOrder.operateTime}','{ stock.entryOrder.orderType}','{stock.entryOrder.ownerCode}')";
            insertsqls.Add(insertMain);
            for (int i = 0; i <stock.orderLines.Count; i++)
            {
                insertsqls.Add($"INSERT INTO STOCKINFEEDBACKDETAIL_XH_JDWMS(EXPIREDQTY,GOODSSTATUS,SHORTFLOATQTY,DIFFQTY,ITEMNO,SHORTQTY,DAMAGEDQTY,GUID,OTHERDIFFQTY,OTHERDIFFFLOATQTY,MAINGUID,ORDERLINENO,EXPIREDFLOATQTY,PLANQTY,DAMAGEDFLOATQTY,EMPTYFLOATQTY,EMPTYQTY,DIFFFLOATQTY,REALINSTOREFLOATQTY,ITEMID,REMARK,PLANFLOATQTY,REALINSTOREQTY,ITEMNAME) VALUES ('{stock.orderLines[i].expiredQty}','{stock.orderLines[i].goodsStatus}','{stock.orderLines[i].shortFloatQty}','{stock.orderLines[i].diffQty}','{stock.orderLines[i].itemNo}','{stock.orderLines[i].shortQty}','{stock.orderLines[i].damagedQty}',sys_guid(),'{stock.orderLines[i].otherDiffQty}','{stock.orderLines[i].otherDiffFloatQty}','{billguid}','{stock.orderLines[i].orderLineNo}','{stock.orderLines[i].expiredFloatQty}','{stock.orderLines[i].planQty}','{stock.orderLines[i].damagedFloatQty}','{stock.orderLines[i].emptyFloatQty}','{stock.orderLines[i].emptyQty}','{stock.orderLines[i].diffFloatQty}','{stock.orderLines[i].realInstoreFloatQty}','{stock.orderLines[i].itemId}','{stock.orderLines[i].remark}','{stock.orderLines[i].planFloatQty}','{stock.orderLines[i].realInstoreQty}','{stock.orderLines[i].itemName}')");
            }
            var result = sqlHelper.ExecSql(insertsqls);
            if (string.IsNullOrEmpty(result))
            {
                //调用存储过程WMS_JDWMS_STOCKINRETURN
                List<SqlHelperParameter> lstPara = new List<SqlHelperParameter>();
                lstPara.Add(new SqlHelperParameter
                {
                    DataType = ParamsType.VARCHAR2,
                    Size = 50,
                    Direction = ParameterDirection.Input,
                    Name = "BILLGUID",
                    Value = billguid
                });
                lstPara.Add(new SqlHelperParameter
                {
                    DataType = ParamsType.Decimal,
                    Size = 50,
                    Direction = ParameterDirection.Output,
                    Name = "RETURNVALUE",
                    Value = ""
                });
                lstPara.Add(new SqlHelperParameter
                {
                    DataType = ParamsType.VARCHAR2,
                    Size = 50,
                    Direction = ParameterDirection.Output,
                    Name = "RETURNMSG",
                    Value = ""
                });
                Hashtable procResult = sqlHelper.ExecProc("WMS_JDWMS_STOCKINRETURN", lstPara); 
            }
            return "csb";
        }
    }
}