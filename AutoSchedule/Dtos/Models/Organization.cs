using System.ComponentModel.DataAnnotations;

namespace AutoSchedule.Dtos.Models
{
    public class Organization
    {
        [Key]
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string DBType { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DataBaseName { get; set; }
        public string ConnectingString { get; set; }
    }



}
