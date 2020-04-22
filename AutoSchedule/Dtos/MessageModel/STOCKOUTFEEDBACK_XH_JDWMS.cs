using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    [Table(Name = "STOCKOUTFEEDBACK_XH_JDWMS")]
    public class STOCKOUTFEEDBACK_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string DELIVERYORDERCODE { get; set; }
        public string DELIVERYORDERID { get; set; }
        public string SALEPLATFORMORDERCODE { get; set; }
        public string SALEPLATFORMSOURCECODE { get; set; }
        public string WAREHOUSECODE { get; set; }
        public string ORDERTYPE { get; set; }
        public string CONFIRMTYPE { get; set; }
        public string SELLERNO { get; set; }
        public string DEPTNO { get; set; }
        public string SHOPNO { get; set; }
        public string DELIVERYDATE { get; set; }
        public string OPERATORCODE { get; set; }
        public string OPERATORNAME { get; set; }
        public string OPERATETIME { get; set; }
        public string ERPSTATUE { get; set; }
        public string OUTSTATUE { get; set; }
    }
}
