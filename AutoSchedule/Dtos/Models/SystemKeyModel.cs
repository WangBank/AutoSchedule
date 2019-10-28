using System.Collections.Generic;

namespace AutoSchedule.Dtos.Models
{
    public class SystemKeyModel
    {
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
    }

    public class SystemKeyData
    {
        public int code { get; set; }
        public int count { get; set; }
        public string msg { get; set; }
        public List<SystemKeyModel> data { get; set; }
    }
}