using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;

namespace AutoSchedule.Dtos.MessageModel
{
    [Table(Name = "ORDERCANCELBACK_XH_JDWMS")]
    public class ORDERCANCELBACK_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string STATUS { get; set; }
        public string MESSAGE { get; set; }
        public string ORDERCODE { get; set; }
        public string ORDERID { get; set; }
        public string ORDERTYPE { get; set; }
        public string FAILUREREASON { get; set; }
        public string ERPSTATUE { get; set; }
    }
}
