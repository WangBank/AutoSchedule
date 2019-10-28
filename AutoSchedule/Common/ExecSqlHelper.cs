using BankDbHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public class ExecSqlHelper
    {
        public string ConString;
        public string DbType;
        SqlHelper sqlHelper;
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

        // Token: 0x06000024 RID: 36 RVA: 0x0000308C File Offset: 0x0000128C
        public async Task<DataSet> GetDataSetAsync(string sql)
        {
            return await this.sqlHelper.GetDataSetAsync(sql);
        }

        // Token: 0x06000025 RID: 37 RVA: 0x000030AC File Offset: 0x000012AC
        public async Task<DataTable> GetDataTableAsync(string sql)
        {
            return await this.sqlHelper.GetDataTableAsync(sql);
        }

        // Token: 0x06000026 RID: 38 RVA: 0x000030CC File Offset: 0x000012CC
        public async Task<object> GetValueAsync(string sql)
        {
            return await this.sqlHelper.GetValueAsync(sql);
        }

        // Token: 0x06000027 RID: 39 RVA: 0x000030EC File Offset: 0x000012EC
        public async Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara)
        {
            return await this.sqlHelper.ExecProcAsync(procName, lstPara);
        }

        // Token: 0x06000028 RID: 40 RVA: 0x0000310C File Offset: 0x0000130C
        public async Task<string> ExecSqlAsync(List<ParamSql> lst)
        {
            return await this.sqlHelper.ExecSqlAsync(lst);
        }

        // Token: 0x06000029 RID: 41 RVA: 0x0000312C File Offset: 0x0000132C
        public async Task<string> ExecSqlAsync(ArrayList sqlList)
        {
            return await this.sqlHelper.ExecSqlAsync(sqlList);
        }

        // Token: 0x0600002A RID: 42 RVA: 0x0000314C File Offset: 0x0000134C
        public async Task<string> ExecSqlAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            return await this.sqlHelper.ExecSqlAsync(sql, lstPara);
        }

        // Token: 0x0600002B RID: 43 RVA: 0x0000316C File Offset: 0x0000136C
        public async Task<string> ExecSqlAsync(string sql)
        {
            return await this.sqlHelper.ExecSqlAsync(sql);
        }

        // Token: 0x0600002C RID: 44 RVA: 0x0000318A File Offset: 0x0000138A
        public async Task DisposeAsync()
        {
          await  this.sqlHelper.DisposeAsync();
        }

    }
}
