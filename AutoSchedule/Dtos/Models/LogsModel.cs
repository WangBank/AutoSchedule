using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.Models
{
    public class LogsModel
    {
        public string Time { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string EventName { get; set; }
        public string Id { get; set; }
    }

    public class LogsModelData
    {
        public int code { get; set; }
        public int count { get; set; }
        public string msg { get; set; }
        public List<LogsModel> data { get; set; }
    }
}
