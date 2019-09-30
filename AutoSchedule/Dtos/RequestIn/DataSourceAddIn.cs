using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.RequestIn
{
    public class DataSourceAddIn
    {
        //FType, string GUID, string Name, string IsStart, string MainKey, string GroupSqlString, string SqlString, string AfterSqlString, string AfterSqlstring2
        public string GUID { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 分组数据源 表头
        /// </summary>
        public string GroupSqlString { get; set; }

        /// <summary>
        /// 表体
        /// </summary>
        public string SqlString { get; set; }

        /// <summary>
        /// 成功后执行语句
        /// </summary>
        public string AfterSqlString { get; set; }


        /// <summary>
        /// 失败后执行语句
        /// </summary>
        public string AfterSqlstring2 { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public string IsStart { get; set; }


        /// <summary>
        /// 单据类型  0业务单据
        /// </summary>
        public string FType { get; set; }


        /// <summary>
        /// 主关键字段
        /// </summary>
        public string MainKey { get; set; }
    }
}
