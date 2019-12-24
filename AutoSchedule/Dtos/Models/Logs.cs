using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.Models
{
    public class Logs
    {
        [Key]
        public string Id { get; set; }
        public string TimestampUtc { get; set; }
        public string Application { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public string Exception { get; set; }
    }
}
