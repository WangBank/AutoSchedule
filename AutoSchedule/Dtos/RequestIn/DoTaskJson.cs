using System.Collections.Generic;
using System.Data;

namespace AutoSchedule.Dtos.RequestIn
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