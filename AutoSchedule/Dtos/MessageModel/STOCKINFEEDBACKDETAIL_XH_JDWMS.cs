using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    [Table(Name = "STOCKINFEEDBACKDETAIL_XH_JDWMS")]
    public class STOCKINFEEDBACKDETAIL_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string EXPIREDQTY { get; set; }
        public string GOODSSTATUS { get; set; }
        public string SHORTFLOATQTY { get; set; }
        public string DIFFQTY { get; set; }
        public string ITEMNO { get; set; }
        public string SHORTQTY { get; set; }
        public string DAMAGEDQTY { get; set; }
        public string OTHERDIFFQTY { get; set; }
        public string OTHERDIFFFLOATQTY { get; set; }
        public string MAINGUID { get; set; }
        public string ORDERLINENO { get; set; }
        public string EXPIREDFLOATQTY { get; set; }
        public string PLANQTY { get; set; }
        public string DAMAGEDFLOATQTY { get; set; }
        public string EMPTYFLOATQTY { get; set; }
        public string EMPTYQTY { get; set; }
        public string DIFFFLOATQTY { get; set; }
        public string REALINSTOREFLOATQTY { get; set; }
        public string ITEMID { get; set; }
        public string REMARK { get; set; }
        public string PLANFLOATQTY { get; set; }
        public string REALINSTOREQTY { get; set; }
        public string ITEMNAME { get; set; }
    }
}
