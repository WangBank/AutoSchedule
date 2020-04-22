using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    [Table(Name = "RTWBACK_XH_JDWMS")]
    public class RTWBACK_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string RTWORDERCODE { get; set; }
        public string RTWORDERID { get; set; }
        public string WAREHOUSECODE { get; set; }
        public string ORDERTYPE { get; set; }
        public string BIZTYPE { get; set; }
        public string RTWSTATUS { get; set; }
        public string OWNERNO { get; set; }
        public string OUTBIZCODE { get; set; }
        public string ORDERCONFIRMTIME { get; set; }
        public string LOGISTICSCODE { get; set; }
        public string LOGISTICSNAME { get; set; }
        public string EXPRESSCODE { get; set; }
        public string RETURNREASON { get; set; }
        public string REMARK { get; set; }
        public string ERPSTATUE { get; set; }
    }

    [Table(Name = "RTWBACKDETAIL_XH_JDWMS")]
    public class RTWBACKDETAIL_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string MAINGUID { get; set; }
        public string ORDERLINENO { get; set; }
        public string ORDERSOURCECODE { get; set; }
        public string SUBSOURCECODE { get; set; }
        public string OWNERCODE { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMID { get; set; }
        public string INVENTORYTYPE { get; set; }
        public string PLANQTY { get; set; }
        public string ACTUALQTY { get; set; }
        public string PLANOUTQTY { get; set; }
        public string ACTUALOUTQTY { get; set; }
        public string BATCHCODE { get; set; }
        public string PRODUCTDATE { get; set; }
        public string EXPIREDATE { get; set; }
        public string PRODUCECODE { get; set; }
        public string QRCODE { get; set; }
    }
}
