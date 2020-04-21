using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using System;
namespace Test
{
    [Table(Name = "STOCKINFEEDBACK_XH_JDWMS")]
    public class 入库完成回传类
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
        public string STATUE { get; set; }
    }
}
