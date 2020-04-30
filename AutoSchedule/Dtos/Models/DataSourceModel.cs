using System.Collections.Generic;

namespace AutoSchedule.Dtos.Models
{
    public class DataSourceModel
    {
        public string GUID { get; set; }
        public string Name { get; set; }
        public string GroupSqlString { get; set; }
        public string SqlString { get; set; }
        public string AfterSqlString { get; set; }
        public string AfterSqlstring2 { get; set; }
        public string IsStart { get; set; }
        public string MainKey { get; set; }
    }

    public class DataSourceData
    {
        public int code { get; set; }
        public int count { get; set; }
        public string msg { get; set; }
        public List<DataSourceModel> data { get; set; }
    }
}