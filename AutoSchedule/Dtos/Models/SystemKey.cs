using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace AutoSchedule.Dtos.Models
{
    [Table(Name = "SystemKeys")]
    public class SystemKey
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        [Key]
        public string KeyName { get; set; }

        public string KeyValue { get; set; }
    }
}