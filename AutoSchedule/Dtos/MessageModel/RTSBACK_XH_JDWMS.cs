using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;

namespace AutoSchedule.Dtos.MessageModel
{
    [Table(Name = "RTSBACK_XH_JDWMS")]
    public class RTSBACK_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string RTSORDERCODE { get; set; }
        public string RTSORDERID { get; set; }
        public string RTSTYPE { get; set; }
        public string OWNERNO { get; set; }
        public string DELIVERYMODE { get; set; }
        public string WAREHOUSENO { get; set; }
        public string SUPPLIERNO { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string ORDERCREATETIME { get; set; }
        public string REMARK { get; set; }
        public string ERPSTATUE { get; set; }
    }



    [Table(Name = "RTSBACKDETAIL_XH_JDWMS")]
    public class RTSBACKDETAIL_XH_JDWMS
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
