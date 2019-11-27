using BankDbHelper;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class ExecSqlHelper
    {
        public string ConString;
        public string DbType;
        private SqlHelper sqlHelper;

        public ExecSqlHelper(string _conString, string _DbType)
        {
            ConString = _conString;
            DbType = _DbType;
            sqlHelper = new SqlHelper(_DbType, _conString);
        }

        public async Task<bool> TestConnectionAsync()
        {
            return await this.sqlHelper.TestConnectionAsync();
        }

        public async Task<DataSet> GetDataSetAsync(string sql)
        {
            return await this.sqlHelper.GetDataSetAsync(sql);
        }

        public async Task<DataTable> GetDataTableAsync(string sql)
        {
            return await this.sqlHelper.GetDataTableAsync(sql);
        }

        public async Task<object> GetValueAsync(string sql)
        {
            return await this.sqlHelper.GetValueAsync(sql);
        }

        public async Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara)
        {
            return await this.sqlHelper.ExecProcAsync(procName, lstPara);
        }

        public async Task<string> ExecSqlAsync(List<ParamSql> lst)
        {
            return await this.sqlHelper.ExecSqlAsync(lst);
        }

        public async Task<string> ExecSqlAsync(ArrayList sqlList)
        {
            return await this.sqlHelper.ExecSqlAsync(sqlList);
        }

        public async Task<string> ExecSqlAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            return await this.sqlHelper.ExecSqlAsync(sql, lstPara);
        }

        public async Task<string> ExecSqlAsync(string sql)
        {
            return await this.sqlHelper.ExecSqlAsync(sql);
        }

        public async Task DisposeAsync()
        {
            await this.sqlHelper.DisposeAsync();
        }
    }
}