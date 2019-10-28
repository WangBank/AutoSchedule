using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BankDbHelper
{
    public interface ISqlHelper
    {
        Task<bool> TestConnectionAsync();

        Task<DataSet> GetDataSetAsync(string sql);

        Task<DataTable> GetDataTableAsync(string sql);

        Task<object> GetValueAsync(string sql);

        Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara);

        Task<string> ExecSqlAsync(ArrayList sqlList);

        Task<string> ExecSqlAsync(string sql);

        Task<string> ExecSqlAsync(string sql, List<SqlHelperParameter> lstPara);

        Task<string> ExecSqlAsync(List<ParamSql> lst);

        Task DisposeAsync();
    }
}