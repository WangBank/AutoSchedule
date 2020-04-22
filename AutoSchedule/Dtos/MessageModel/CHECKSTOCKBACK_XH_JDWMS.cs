using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AutoSchedule.Dtos.MessageModel
{
    [Table(Name = "CHECKSTOCKBACK_XH_JDWMS")]
    public class CHECKSTOCKBACK_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string WAREHOUSECODE { get; set; }
        public string CHECKORDERCODE { get; set; }
        public string CHECKORDERID { get; set; }
        public string OWNERCODE { get; set; }
        public string CHECKTIME { get; set; }
        public string OUTBIZCODE { get; set; }
        public string REMARK { get; set; }
        public string ERPSTATUE { get; set; }
    }


    [Table(Name = "CHECKSTOCKBACKDETAIL_XH_JDWMS")]
    public class CHECKSTOCKBACKDETAIL_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string MAINGUID { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMID { get; set; }
        public string INVENTORYTYPE { get; set; }
        public string QUANTITY { get; set; }
        public string QUANTITYVALUE { get; set; }
    }
}
