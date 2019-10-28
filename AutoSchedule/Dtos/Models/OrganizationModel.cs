using System.Collections.Generic;

namespace AutoSchedule.Dtos.Models
{
    public class OrganizationModel
    {
        public string orgNum { get; set; }
        public string orgName { get; set; }
    }

    public class OrganizationData
    {
        public int code { get; set; }
        public int count { get; set; }
        public string msg { get; set; }
        public List<OrganizationModel> data { get; set; }
    }
}