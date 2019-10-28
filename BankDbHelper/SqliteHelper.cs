using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;

namespace BankDbHelper
{
    internal class SqliteHelper : ISqlHelper
    {
        // Token: 0x06000039 RID: 57 RVA: 0x0000325C File Offset: 0x0000145C
        public SqliteHelper(string ConnectionString)
        {
            this._connectionString = ConnectionString;
        }

        // Token: 0x0600003A RID: 58 RVA: 0x000032AC File Offset: 0x000014AC
        public async Task<bool> TestConnectionAsync()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(this._connectionString);
            bool result;
            try
            {
                await sqliteConnection.OpenAsync();
                await sqliteConnection.CloseAsync();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600003B RID: 59 RVA: 0x000032F0 File Offset: 0x000014F0
        public async Task<DataSet> GetDataSetAsync(string sql)
        {
            DataSet result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.ds = new DataSet();
                    this.dap = new SQLiteDataAdapter(sql, this.conn);
                    this.dap.Fill(this.ds);
                }
                result = this.ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        // Token: 0x0600003C RID: 60 RVA: 0x000033A4 File Offset: 0x000015A4
        public async Task<DataTable> GetDataTableAsync(string sql)
        {
            DataTable result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.ds = new DataSet();
                    this.dap = new SQLiteDataAdapter(sql, this.conn);
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

        // Token: 0x0600003D RID: 61 RVA: 0x00003460 File Offset: 0x00001660
        public async Task<object> GetValueAsync(string sql)
        {
            object result;
            using (this.conn = new SQLiteConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    await this.conn.OpenAsync();
                }
                this.cmd = new SQLiteCommand(sql, this.conn);
                this.cmd.CommandType = CommandType.Text;
                result = this.cmd.ExecuteScalarAsync();
            }
            return result;
        }

        // Token: 0x0600003E RID: 62 RVA: 0x000034F0 File Offset: 0x000016F0
        public async Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara)
        {
            throw new NotImplementedException();
        }

        // Token: 0x0600003F RID: 63 RVA: 0x000034F8 File Offset: 0x000016F8
        public async Task<string> ExecSqlAsync(ArrayList sqlList)
        {
            string result;
            using (this.conn = new SQLiteConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    await this.conn.OpenAsync();
                }
                SQLiteTransaction sqliteTransaction = this.conn.BeginTransaction();
                try
                {
                    this.cmd = new SQLiteCommand();
                    this.cmd.CommandType = CommandType.Text;
                    this.cmd.Connection = this.conn;
                    foreach (object obj in sqlList)
                    {
                        string commandText = (string)obj;
                        this.cmd.CommandText = commandText;
                        await  this.cmd.ExecuteNonQueryAsync();
                    }
                    await sqliteTransaction.CommitAsync();
                    result = string.Empty;
                }
                catch (Exception ex)
                {
                    await sqliteTransaction.RollbackAsync();
                    result = ex.Message;
                }
                finally
                {
                    await sqliteTransaction.DisposeAsync();
                }
            }
            return result;
        }

        // Token: 0x06000040 RID: 64 RVA: 0x0000363C File Offset: 0x0000183C
        public async Task<string> ExecSqlAsync(string sql)
        {
            string result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                     this.cmd = new SQLiteCommand(sql, this.conn);
                     this.cmd.CommandType = CommandType.Text;
                    await this.cmd.ExecuteNonQueryAsync();
                    result = string.Empty;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        // Token: 0x06000041 RID: 65 RVA: 0x000036EC File Offset: 0x000018EC
        public async Task<string> ExecSqlAsync(List<ParamSql> lst)
        {
            string result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    SQLiteTransaction sqliteTransaction = this.conn.BeginTransaction();
                    try
                    {
                        this.cmd = new SQLiteCommand();
                        this.cmd.CommandType = CommandType.Text;
                        this.cmd.Connection = this.conn;
                        this.cmd.Transaction = sqliteTransaction;
                        foreach (ParamSql paramSql in lst)
                        {
                            this.cmd.CommandText = paramSql.Sql;
                            foreach (SqlHelperParameter sqlHelperParameter in paramSql.Params)
                            {
                                this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                            }
                            await this.cmd.ExecuteNonQueryAsync();
                        }
                        await sqliteTransaction.CommitAsync();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        await sqliteTransaction.RollbackAsync();
                        result = ex.Message;
                    }
                    finally
                    {
                        await sqliteTransaction.DisposeAsync();
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

        // Token: 0x06000042 RID: 66 RVA: 0x00003910 File Offset: 0x00001B10
        public async Task<string> ExecSqlAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            string result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.cmd = new SQLiteCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    await this.cmd.ExecuteNonQueryAsync();
                    result = string.Empty;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        // Token: 0x06000043 RID: 67 RVA: 0x00003A18 File Offset: 0x00001C18
        private SQLiteParameter GetParameter(SqlHelperParameter sqlHelperParameter)
        {
            SQLiteParameter result = new SQLiteParameter();
            switch (sqlHelperParameter.DataType)
            {
            }
            return result;
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00003A6D File Offset: 0x00001C6D
        public async Task DisposeAsync()
        {
            await this.conn.CloseAsync();
             this.dap.Dispose();
             this.ds.Dispose();
            await this.cmd.DisposeAsync();
            await this.conn.DisposeAsync();
        }

        // Token: 0x0400001F RID: 31
        public string _connectionString;

        // Token: 0x04000020 RID: 32
        private SQLiteConnection conn = new SQLiteConnection();

        // Token: 0x04000021 RID: 33
        private SQLiteCommand cmd = new SQLiteCommand();

        // Token: 0x04000022 RID: 34
        private SQLiteDataAdapter dap = new SQLiteDataAdapter();

        // Token: 0x04000023 RID: 35
        private DataSet ds = new DataSet();

        // Token: 0x04000024 RID: 36
        public SQLiteParameter Parameter = null;
    }
}
