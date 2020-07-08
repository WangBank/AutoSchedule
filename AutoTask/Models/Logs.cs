using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoTask.Models
{
    [Table(Name = "Logs")]
    public class Logs
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public string Id { get; set; }
        public string TimestampUtc { get; set; }
        public string Application { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public string EventId { get; set; }
    }
}
