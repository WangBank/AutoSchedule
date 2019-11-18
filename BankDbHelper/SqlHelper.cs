using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BankDbHelper
{
    public class SqlHelper
    {
        public bool IsDebug;
        private ISqlHelper helper;

        private DBTypeEnum _dbType;

        public SqlHelper(string dbType, string connString)
        {
            this._dbType = (DBTypeEnum)Enum.Parse(typeof(DBTypeEnum), dbType);
            switch (this._dbType)
            {
                case DBTypeEnum.Oracle:
                    this.helper = new OracleSqlHelper(connString);
                    break;

                case DBTypeEnum.SqlServer:
                    this.helper = new SqlServerSqlHelper(connString);
                    break;

                case DBTypeEnum.Sqlite:
                    this.helper = new SqliteHelper(connString);
                    break;

                case DBTypeEnum.MySql:
                    this.helper = new MySqlHelper(connString);
                    break;

                default:
                    break;
            }
        }

#if NS21

        public async Task<bool> TestConnectionAsync()
        {
            return await this.helper.TestConnectionAsync();
        }

        public async Task<DataSet> GetDataSetAsync(string sql)
        {
            return await this.helper.GetDataSetAsync(sql);
        }

        public async Task<DataTable> GetDataTableAsync(string sql)
        {
            return await this.helper.GetDataTableAsync(sql);
        }

        public async Task<DataTable> GetDataTableAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            return await this.helper.GetDataTableAsync(sql, lstPara);
        }

        public async Task<object> GetValueAsync(string sql)
        {
            return await this.helper.GetValueAsync(sql);
        }

        public async Task<object> GetValueAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            return await this.helper.GetValueAsync(sql, lstPara);
        }

        public async Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara)
        {
            return await this.helper.ExecProcAsync(procName, lstPara);
        }

        public async Task<string> ExecSqlAsync(List<ParamSql> lst)
        {
            return await this.helper.ExecSqlAsync(lst);
        }

        public async Task<string> ExecSqlAsync(ArrayList sqlList)
        {
            return await this.helper.ExecSqlAsync(sqlList);
        }

        public async Task<string> ExecSqlAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            return await this.helper.ExecSqlAsync(sql, lstPara);
        }

        public async Task<string> ExecSqlAsync(string sql)
        {
            return await this.helper.ExecSqlAsync(sql);
        }

        public async Task DisposeAsync()
        {
            await this.helper.DisposeAsync();
        }

#else
#endif

        public bool TestConnection()
        {
            return this.helper.TestConnection();
        }

        public DataSet GetDataSet(string sql)
        {
            return this.helper.GetDataSet(sql);
        }

        public DataTable GetDataTable(string sql)
        {
            return this.helper.GetDataTable(sql);
        }

        public DataTable GetDataTable(string sql, List<SqlHelperParameter> lstPara)
        {
            return this.helper.GetDataTable(sql, lstPara);
        }

        public object GetValue(string sql)
        {
            return this.helper.GetValue(sql);
        }

        public object GetValue(string sql, List<SqlHelperParameter> lstPara)
        {
            return this.helper.GetValue(sql, lstPara);
        }

        public Hashtable ExecProc(string procName, List<SqlHelperParameter> lstPara)
        {
            return this.helper.ExecProc(procName, lstPara);
        }

        public string ExecSql(List<ParamSql> lst)
        {
            return this.helper.ExecSql(lst);
        }

        public string ExecSql(ArrayList sqlList)
        {
            return this.helper.ExecSql(sqlList);
        }

        public string ExecSql(string sql, List<SqlHelperParameter> lstPara)
        {
            return this.helper.ExecSql(sql, lstPara);
        }

        public string ExecSql(string sql)
        {
            return this.helper.ExecSql(sql);
        }

        public void Dispose()
        {
            this.helper.Dispose();
        }
    }
}