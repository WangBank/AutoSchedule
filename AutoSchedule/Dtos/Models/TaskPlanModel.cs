using System.Collections.Generic;

namespace AutoSchedule.Dtos.Models
{
    public class TaskPlanModel
    {
        /// <summary>
        /// guid
        /// </summary>
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
        /// 任务类型 0 上传 1下载
        /// </summary>
        public string TaskPlanType { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// 任务Url
        /// </summary>
        public string TaskUrl { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public string State { get; set; }
    }

    public class TaskPlanData
    {
        public int code { get; set; }
        public int count { get; set; }
        public string msg { get; set; }
        public List<TaskPlanModel> data { get; set; }
    }

    public class TaskPlanDetailModel
    {
        public string dsGuid { get; set; }
        public string dsName { get; set; }
        public string dsState { get; set; }
        public string tkDetailGuid { get; set; }
    }

    public class TaskPlanDetailData
    {
        public int code { get; set; }
        public int count { get; set; }
        public string msg { get; set; }
        public List<TaskPlanDetailModel> data { get; set; }
    }
}