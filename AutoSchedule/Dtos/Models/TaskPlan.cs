using System.ComponentModel.DataAnnotations;

namespace AutoSchedule.Dtos.Models
{
    public class TaskPlan
    {
        /// <summary>
        /// guid
        /// </summary>
        [Key]
        public string GUID { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public string CODE { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 时间数
        /// </summary>
        public string Frequency { get; set; }


        /// <summary>
        /// 时间类型 0 秒 1 分钟 2 小时
        /// </summary>
        public string FrequencyType { get; set; }

        /// <summary>
        /// 任务类型 0 上传 1下载
        /// </summary>
        public string TaskPlanType { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public string OrgCode { get; set; }
    }

    public class TaskPlanDetail
    {
        /// <summary>
        /// guid
        /// </summary>
        [Key]
        public string GUID { get; set; }


        public string TaskPlanGuid { get; set; }

        public string OpenSqlGuid { get; set; }

    }
}
