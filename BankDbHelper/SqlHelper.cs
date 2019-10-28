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

        public async Task<object> GetValueAsync(string sql)
        {
            return await this.helper.GetValueAsync(sql);
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
    }
}