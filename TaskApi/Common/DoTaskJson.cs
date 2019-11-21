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
        public List<Datas> Data { get; set; }

    }

    public class Datas
    {
        public DataTable DataMain { get; set; }
        public List<DataTable> DataDetail { get; set; }
    }

}
