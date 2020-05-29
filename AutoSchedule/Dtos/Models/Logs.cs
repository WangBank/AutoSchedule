using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.Models
{
    [Table(Name = "Logs")]
    public class Logs
    {
        [Key]
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
