using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TaskApi.Common
{
    public class DoTaskJson
    {
        public string OpenSqlGuid { get; set; }
        public List<Datum> Data { get; set; }
    }

    
    
    public class Datum
    {
        public List<Datamain> DataMain { get; set; }
        public List<Datadetail> DataDetail { get; set; }
    }

    public class Datamain
    {
        public string employeename { get; set; }
        public string guid { get; set; }
    }

    public class Datadetail
    {
        public string employeename { get; set; }
    }

}
