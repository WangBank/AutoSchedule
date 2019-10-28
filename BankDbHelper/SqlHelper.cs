using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace BankDbHelper
{
    public class SqlHelper
    {
        public bool IsDebug;
        private ISqlHelper helper;

        // Token: 0x04000019 RID: 25
        private DBTypeEnum _dbType;
        // Token: 0x06000022 RID: 34 RVA: 0x00002FD8 File Offset: 0x000011D8
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

        // Token: 0x06000023 RID: 35 RVA: 0x0000306C File Offset: 0x0000126C
        public async Task<bool> TestConnectionAsync()
        {
            return await this.helper.TestConnectionAsync();
        }

        // Token: 0x06000024 RID: 36 RVA: 0x0000308C File Offset: 0x0000128C
        public async Task<DataSet> GetDataSetAsync(string sql)
        {
            return await this.helper.GetDataSetAsync(sql);
        }

        // Token: 0x06000025 RID: 37 RVA: 0x000030AC File Offset: 0x000012AC
        public async Task<DataTable> GetDataTableAsync(string sql)
        {
            return await this.helper.GetDataTableAsync(sql);
        }

        // Token: 0x06000026 RID: 38 RVA: 0x000030CC File Offset: 0x000012CC
        public async Task<object> GetValueAsync(string sql)
        {
            return await this.helper.GetValueAsync(sql);
        }

        // Token: 0x06000027 RID: 39 RVA: 0x000030EC File Offset: 0x000012EC
        public async Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara)
        {
            return await this.helper.ExecProcAsync(procName, lstPara);
        }

        // Token: 0x06000028 RID: 40 RVA: 0x0000310C File Offset: 0x0000130C
        public async Task<string> ExecSqlAsync(List<ParamSql> lst)
        {
            return await this.helper.ExecSqlAsync(lst);
        }

        // Token: 0x06000029 RID: 41 RVA: 0x0000312C File Offset: 0x0000132C
        public async Task<string> ExecSqlAsync(ArrayList sqlList)
        {
            return await this.helper.ExecSqlAsync(sqlList);
        }

        // Token: 0x0600002A RID: 42 RVA: 0x0000314C File Offset: 0x0000134C
        public async Task<string> ExecSqlAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            return await this.helper.ExecSqlAsync(sql, lstPara);
        }

        // Token: 0x0600002B RID: 43 RVA: 0x0000316C File Offset: 0x0000136C
        public  async Task<string> ExecSqlAsync(string sql)
        {
            return await this.helper.ExecSqlAsync(sql);
        }

        // Token: 0x0600002C RID: 44 RVA: 0x0000318A File Offset: 0x0000138A
        public async Task DisposeAsync()
        {
            await this.helper.DisposeAsync();
        }

        // Token: 0x04000018 RID: 24
       
    }
}
