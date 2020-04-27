using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    [Table(Name = "STOCKOUTBACKDETAIL_XH_JDWMS")]
    public class STOCKOUTBACKDETAIL_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string MAINGUID { get; set; }
        public string ORDERLINENO { get; set; }
        public string OWNERCODE { get; set; }
        public string SHOPGOODSNO { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMID { get; set; }
        public string INVENTORYTYPE { get; set; }
        public string PLANQTY { get; set; }
        public string PLANOUTQTY { get; set; }
        public string ACTUALQTY { get; set; }
        public string ACTUALOUTQTY { get; set; }
        public string ORDERSOURCECODE { get; set; }
    }
}
