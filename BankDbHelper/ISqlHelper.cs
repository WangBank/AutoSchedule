using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BankDbHelper
{
    public interface ISqlHelper: IDisposable
    {
#if NS21

        Task<bool> TestConnectionAsync();

        Task<DataSet> GetDataSetAsync(string sql);

        Task<DataTable> GetDataTableAsync(string sql);

        Task<DataTable> GetDataTableAsync(string sql, List<SqlHelperParameter> lstPara);

        Task<object> GetValueAsync(string sql);

        Task<object> GetValueAsync(string sql, List<SqlHelperParameter> lstPara);

        Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara);

        Task<string> ExecSqlAsync(ArrayList sqlList);

        Task<string> ExecSqlAsync(string sql);

        Task<string> ExecSqlAsync(string sql, List<SqlHelperParameter> lstPara);

        Task<string> ExecSqlAsync(List<ParamSql> lst);
        Task DisposeAsync();

#else
#endif

        bool TestConnection();

        DataSet GetDataSet(string sql);

        DataTable GetDataTable(string sql);

        DataTable GetDataTable(string sql, List<SqlHelperParameter> lstPara);

        object GetValue(string sql);

        object GetValue(string sql, List<SqlHelperParameter> lstPara);

        Hashtable ExecProc(string procName, List<SqlHelperParameter> lstPara);

        string ExecSql(ArrayList sqlList);

        string ExecSql(string sql);

        string ExecSql(string sql, List<SqlHelperParameter> lstPara);

        string ExecSql(List<ParamSql> lst);

    }
}