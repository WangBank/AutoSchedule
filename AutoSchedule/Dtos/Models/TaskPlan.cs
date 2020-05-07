using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace AutoSchedule.Dtos.Models
{
    [Table(Name = "TaskPlan")]
    public class TaskPlan
    {
        /// <summary>
        /// guid
        /// </summary>
        [Key]
        [Column(IsIdentity = true, IsPrimary = true)]
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
        /// 所属机构
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// 任务Url
        /// </summary>
        public string DllOrUrl { get; set; }

        /// <summary>
        /// 数据接受类型 0：发送数据到dll   1 ：发送数据到api
        /// </summary>
        public string WorkType { get; set; }


        /// <summary>
        /// 运行状态
        /// </summary>
        public string Status { get; set; }


    }

    [Table(Name = "TaskPlanRelation")]
    public class TaskPlanDetail
    {
        /// <summary>
        /// guid
        /// </summary>
        [Key]
        [Column(IsIdentity = true, IsPrimary = true)]
        public string GUID { get; set; }

        public string TaskPlanGuid { get; set; }

        public string OpenSqlGuid { get; set; }
    }

    public class TaskPlanDetailExGuid
    {
        public string TaskPlanGuid { get; set; }

        public string OpenSqlGuid { get; set; }
    }

    public class TaskPlanExGuidCode
    {
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
        /// 所属机构
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// 任务Url
        /// </summary>
        public string DllOrUrl { get; set; }

        /// <summary>
        /// 数据接受类型 0：发送数据到dll   1 ：发送数据到api
        /// </summary>
        public string WorkType { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        public string Status { get; set; }
    }
}