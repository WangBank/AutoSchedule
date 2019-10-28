using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace BankDbHelper
{
    public interface ISqlHelper
    {
        // Token: 0x0600000B RID: 11
         Task<bool> TestConnectionAsync();

        // Token: 0x0600000C RID: 12
        Task<DataSet> GetDataSetAsync(string sql);

        // Token: 0x0600000D RID: 13
        Task<DataTable> GetDataTableAsync(string sql);

        // Token: 0x0600000E RID: 14
        Task<object> GetValueAsync(string sql);

        // Token: 0x0600000F RID: 15
        Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara);

        // Token: 0x06000010 RID: 16
        Task<string> ExecSqlAsync(ArrayList sqlList);

        // Token: 0x06000011 RID: 17
        Task<string> ExecSqlAsync(string sql);

        // Token: 0x06000012 RID: 18
        Task<string> ExecSqlAsync(string sql, List<SqlHelperParameter> lstPara);

        // Token: 0x06000013 RID: 19
        Task<string> ExecSqlAsync(List<ParamSql> lst);
        Task DisposeAsync();
    }
}
