using System.ComponentModel.DataAnnotations;

namespace AutoSchedule.Dtos.Models
{
    public class SystemKey
    {
        [Key]
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
    }
}
