using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankDbHelper
{
    internal class SqlServerSqlHelper : ISqlHelper
    {
        // Token: 0x06000045 RID: 69 RVA: 0x00003AAC File Offset: 0x00001CAC
        public SqlServerSqlHelper(string ConnectionString)
        {
            this._connectionString = ConnectionString;
        }

        // Token: 0x06000046 RID: 70 RVA: 0x00003AEC File Offset: 0x00001CEC
        public async Task<bool> TestConnectionAsync()
        {
            SqlConnection sqlConnection = new SqlConnection(this._connectionString);
            bool result;
            try
            {
                await sqlConnection.OpenAsync();
                await sqlConnection.CloseAsync();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00003B30 File Offset: 0x00001D30
        public async Task<string> ExecSqlAsync(string sql)
        {
            string result;
            try
            {
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                       await this.conn.OpenAsync();
                    }
                    this.cmd = new SqlCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    int resultInt = await this.cmd.ExecuteNonQueryAsync();
                    result = resultInt.ToString();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00003BE0 File Offset: 0x00001DE0
        public async Task<string> ExecSqlAsync(List<ParamSql> lst)
        {
            string result;
            try
            {
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    SqlTransaction sqlTransaction = this.conn.BeginTransaction();
                    try
                    {
                        this.cmd = new SqlCommand();
                        this.cmd.CommandType = CommandType.Text;
                        this.cmd.Connection = this.conn;
                        this.cmd.Transaction = sqlTransaction;
                        foreach (ParamSql paramSql in lst)
                        {
                            this.cmd.CommandText = paramSql.Sql;
                            foreach (SqlHelperParameter sqlHelperParameter in paramSql.Params)
                            {
                                this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                            }
                           await this.cmd.ExecuteNonQueryAsync();
                        }
                        await sqlTransaction.CommitAsync();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        await sqlTransaction.RollbackAsync();
                        result = ex.Message;
                    }
                    finally
                    {
                        await sqlTransaction.DisposeAsync();
                        await this.cmd.DisposeAsync();
                        await this.conn.CloseAsync();
                    }
                }
            }
            catch (Exception ex2)
            {
                result = ex2.Message;
            }
            return result;
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00003E04 File Offset: 0x00002004
        public async Task<string> ExecSqlAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            string result;
            try
            {
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.cmd = new SqlCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    int resultint = await this.cmd.ExecuteNonQueryAsync();
                    result = resultint.ToString();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00003F0C File Offset: 0x0000210C
        public async Task<DataSet> GetDataSetAsync(string sql)
        {
            DataSet result;
            try
            {
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.ds = new DataSet();
                    this.dap = new SqlDataAdapter(sql, this.conn);
                    this.dap.Fill(this.ds);
                    result = this.ds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00003FBC File Offset: 0x000021BC
        public async Task<DataTable> GetDataTableAsync(string sql)
        {
            DataTable result;
            try
            {
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.ds = new DataSet();
                    this.dap = new SqlDataAdapter(sql, this.conn);
                    this.dap.Fill(this.ds);
                    result = this.ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        // Token: 0x0600004C RID: 76 RVA: 0x00004078 File Offset: 0x00002278
        public async Task<object> GetValueAsync(string sql)
        {
            object result;
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    await this.conn.OpenAsync();
                }
                this.cmd = new SqlCommand(sql, this.conn);
                this.cmd.CommandType = CommandType.Text;
                result = await this.cmd.ExecuteScalarAsync();
            }
            return result;
        }

        // Token: 0x0600004D RID: 77 RVA: 0x00004108 File Offset: 0x00002308
        public async Task<Hashtable>  ExecProcAsync(string procName, List<SqlHelperParameter> lstPara)
        {
            Hashtable result;
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    await this.conn.OpenAsync();
                }
                Hashtable hashtable = new Hashtable();
                this.cmd = new SqlCommand(procName, this.conn);
                this.cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                {
                    this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                }
                await this.cmd.ExecuteNonQueryAsync();
                foreach (SqlHelperParameter sqlHelperParameter2 in lstPara)
                {
                    bool flag2 = sqlHelperParameter2.Direction == ParameterDirection.Output || sqlHelperParameter2.Direction == ParameterDirection.InputOutput;
                    if (flag2)
                    {
                        object obj = sqlHelperParameter2.Value;
                        bool flag3 = (obj == null || obj == DBNull.Value) && sqlHelperParameter2.DataType == ParamsType.Varchar;
                        if (flag3)
                        {
                            obj = "";
                        }
                        hashtable.Add(sqlHelperParameter2.Name, obj);
                    }
                }
                result = hashtable;
            }
            return result;
        }

        // Token: 0x0600004E RID: 78 RVA: 0x000042B8 File Offset: 0x000024B8
        public async Task<string> ExecSqlAsync(ArrayList sqlList)
        {
            string result;
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    await this.conn.OpenAsync();
                }
                bool flag2 = sqlList.Count == 1;
                if (flag2)
                {
                    result = await this.ExecSqlAsync(sqlList[0].ToString());
                }
                else
                {
                    SqlTransaction sqlTransaction =  this.conn.BeginTransaction();
                    try
                    {
                        this.cmd = new SqlCommand();
                        this.cmd.CommandType = CommandType.Text;
                        this.cmd.Connection = this.conn;
                        this.cmd.Transaction = sqlTransaction;
                        foreach (object obj in sqlList)
                        {
                            string commandText = (string)obj;
                            this.cmd.CommandText = commandText;
                            await this.cmd.ExecuteNonQueryAsync();
                        }
                        await sqlTransaction.CommitAsync();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        await sqlTransaction.RollbackAsync();
                        result = ex.Message;
                    }
                    finally
                    {
                        await sqlTransaction.DisposeAsync();
                    }
                }
            }
            return result;
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00004468 File Offset: 0x00002668
        private  SqlParameter GetParameter(SqlHelperParameter sqlHelperParameter)
        {
            SqlParameter sqlParameter = new SqlParameter();
            DbType dbType = DbType.String;
            switch (sqlHelperParameter.DataType)
            {
                case ParamsType.Varchar:
                    dbType = DbType.String;
                    break;
                case ParamsType.Int:
                    dbType = DbType.Int16;
                    break;
                case ParamsType.DateTime:
                    dbType = DbType.DateTime;
                    break;
                case ParamsType.Decimal:
                    dbType = DbType.Decimal;
                    break;
                case ParamsType.Blob:
                    dbType = DbType.Binary;
                    break;
            }
            sqlParameter.ParameterName = sqlHelperParameter.Name;
            sqlParameter.DbType = dbType;
            sqlParameter.Direction = sqlHelperParameter.Direction;
            sqlParameter.Value = sqlHelperParameter.Value;
            sqlParameter.Size = sqlHelperParameter.Size;
            return sqlParameter;
        }

        // Token: 0x06000050 RID: 80 RVA: 0x000044F9 File Offset: 0x000026F9
        public async Task DisposeAsync()
        {
            await this.conn.CloseAsync();
            this.dap.Dispose();
            this.ds.Dispose();
            await this.cmd.DisposeAsync();
            await this.conn.DisposeAsync();
        }

        // Token: 0x04000025 RID: 37
        public string _connectionString;

        // Token: 0x04000026 RID: 38
        private SqlConnection conn = new SqlConnection();

        // Token: 0x04000027 RID: 39
        private SqlDataAdapter dap = new SqlDataAdapter();

        // Token: 0x04000028 RID: 40
        private DataSet ds = new DataSet();

        // Token: 0x04000029 RID: 41
        private SqlCommand cmd = new SqlCommand();
    }
}
