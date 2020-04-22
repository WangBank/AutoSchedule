using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    [Table(Name = "STOCKOUTCOMPLETE_XH_JDWMS")]
    public class STOCKOUTCOMPLETE_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string ERPSTATUE { get; set; }
        public string DELIVERYORDERCODE { get; set; }
        public string OPERATETIME { get; set; }
        public string OPERATEUSER { get; set; }
        public string DELIVERYORDERID { get; set; }
        public string WMSSTATUS { get; set; }
    }


    [Table(Name = "STOCKOUTPAYMETHOD_XH_JDWMS")]
    public class STOCKOUTPAYMETHOD_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string METHOD { get; set; }
        public string AMOUNT { get; set; }
        public string MAINGUID { get; set; }
    }


    [Table(Name = "STOCKOUTCOMDETAIL_XH_JDWMS")]
    public class STOCKOUTCOMDETAIL_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string MAINGUID { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMID { get; set; }
        public string ITEMNAME { get; set; }
        public string QUANTITY { get; set; }
        public string TYPE { get; set; }
    }

}
