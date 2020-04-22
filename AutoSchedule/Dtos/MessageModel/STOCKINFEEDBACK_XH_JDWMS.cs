using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.MessageModel
{
    [Table(Name = "STOCKINFEEDBACK_XH_JDWMS")]
    public class STOCKINFEEDBACK_XH_JDWMS
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }
        public string BILLNO { get; set; }
        public string OWNERCODE { get; set; }
        public string WAREHOUSECODE { get; set; }
        public string CLPSORDERCODE { get; set; }
        public string ORDERTYPE { get; set; }
        public string POORDERSTATUS { get; set; }
        public string OPERATETIME { get; set; }
        public string CONFIRMTYPE { get; set; }
        public string CREATEUSER { get; set; }
        public string ERPSTATUE { get; set; }
    }



    
}
