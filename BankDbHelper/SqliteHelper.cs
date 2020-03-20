using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace BankDbHelper
{
    internal class SqliteHelper : ISqlHelper
    {
        public string _connectionString;

        public SQLiteParameter Parameter = null;

        private SQLiteCommand cmd = new SQLiteCommand();

        private SQLiteConnection conn = new SQLiteConnection();

        private SQLiteDataAdapter dap = new SQLiteDataAdapter();

        private DataSet ds = new DataSet();

        public SqliteHelper(string ConnectionString)
        {
            this._connectionString = ConnectionString;
        }

#if NS21

        public async Task<DataTable> GetDataTableAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    DataTable dt = new DataTable();
                    this.ds = new DataSet();
                    this.cmd = new SQLiteCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> GetValueAsync(string sql, List<SqlHelperParameter> lstPara)
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
                foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                {
                    this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                }
                result = await this.cmd.ExecuteScalarAsync();
            }
            return result;
        }

        public async Task DisposeAsync()
        {
            await this.conn.CloseAsync();
            await this.cmd.DisposeAsync();
            await this.conn.DisposeAsync();
        }

        public async Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara)
        {
           return await Task.FromResult<Hashtable>(new Hashtable());
        }

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
                    this.cmd = new SQLiteCommand
                    {
                        CommandType = CommandType.Text,
                        Connection = this.conn
                    };
                    foreach (object obj in sqlList)
                    {
                        string commandText = (string)obj;
                        this.cmd.CommandText = commandText;
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
                }
            }
            return result;
        }

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
                    this.cmd = new SQLiteCommand(sql, this.conn)
                    {
                        CommandType = CommandType.Text
                    };
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
                        this.cmd = new SQLiteCommand
                        {
                            CommandType = CommandType.Text,
                            Connection = this.conn,
                            Transaction = sqliteTransaction
                        };
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
                    this.cmd = new SQLiteCommand(sql, this.conn)
                    {
                        CommandText = sql
                    };
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
                this.cmd = new SQLiteCommand(sql, this.conn)
                {
                    CommandType = CommandType.Text
                };
                result = this.cmd.ExecuteScalarAsync();
            }
            return result;
        }

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

        public DataTable GetDataTable(string sql, List<SqlHelperParameter> lstPara)
        {
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    DataTable dt = new DataTable();
                    this.ds = new DataSet();
                    this.cmd = new SQLiteCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetValue(string sql, List<SqlHelperParameter> lstPara)
        {
            object result;
            using (this.conn = new SQLiteConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new SQLiteCommand(sql, this.conn);
                this.cmd.CommandType = CommandType.Text;
                foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                {
                    this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                }
                result = this.cmd.ExecuteScalar();
            }
            return result;
        }

        public void Dispose()
        {
            this.conn.Close();
            this.cmd.Dispose();
            this.conn.Dispose();
        }

        public Hashtable ExecProc(string procName, List<SqlHelperParameter> lstPara)
        {
            throw new NotImplementedException();
        }

        public string ExecSql(ArrayList sqlList)
        {
            string result;
            using (this.conn = new SQLiteConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                SQLiteTransaction sqliteTransaction = this.conn.BeginTransaction();
                try
                {
                    this.cmd = new SQLiteCommand
                    {
                        CommandType = CommandType.Text,
                        Connection = this.conn
                    };
                    foreach (object obj in sqlList)
                    {
                        string commandText = (string)obj;
                        this.cmd.CommandText = commandText;
                        this.cmd.ExecuteNonQuery();
                    }
                    sqliteTransaction.Commit();
                    result = string.Empty;
                }
                catch (Exception ex)
                {
                    sqliteTransaction.Rollback();
                    result = ex.Message;
                }
                finally
                {
                    sqliteTransaction.Dispose();
                }
            }
            return result;
        }

        public string ExecSql(string sql)
        {
            string result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new SQLiteCommand(sql, this.conn)
                    {
                        CommandType = CommandType.Text
                    };
                    this.cmd.ExecuteNonQuery();
                    result = string.Empty;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string ExecSql(List<ParamSql> lst)
        {
            string result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    SQLiteTransaction sqliteTransaction = this.conn.BeginTransaction();
                    try
                    {
                        this.cmd = new SQLiteCommand
                        {
                            CommandType = CommandType.Text,
                            Connection = this.conn,
                            Transaction = sqliteTransaction
                        };
                        foreach (ParamSql paramSql in lst)
                        {
                            this.cmd.CommandText = paramSql.Sql;
                            foreach (SqlHelperParameter sqlHelperParameter in paramSql.Params)
                            {
                                this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                            }
                            this.cmd.ExecuteNonQuery();
                        }
                        sqliteTransaction.Commit();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        sqliteTransaction.Rollback();
                        result = ex.Message;
                    }
                    finally
                    {
                        sqliteTransaction.Dispose();
                        this.cmd.Dispose();
                        this.conn.Close();
                    }
                }
            }
            catch (Exception ex2)
            {
                result = ex2.Message;
            }
            return result;
        }

        public string ExecSql(string sql, List<SqlHelperParameter> lstPara)
        {
            string result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new SQLiteCommand(sql, this.conn)
                    {
                        CommandText = sql
                    };
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    this.cmd.ExecuteNonQuery();
                    result = string.Empty;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public DataSet GetDataSet(string sql)
        {
            DataSet result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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

        public DataTable GetDataTable(string sql)
        {
            DataTable result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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

        public object GetValue(string sql)
        {
            object result;
            using (this.conn = new SQLiteConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new SQLiteCommand(sql, this.conn)
                {
                    CommandType = CommandType.Text
                };
                result = this.cmd.ExecuteScalar();
            }
            return result;
        }

        public bool TestConnection()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(this._connectionString);
            bool result;
            try
            {
                sqliteConnection.Open();
                sqliteConnection.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

#else
        public DataTable GetDataTable(string sql, List<SqlHelperParameter> lstPara)
        {
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    DataTable dt = new DataTable();
                    this.ds = new DataSet();
                    this.cmd = new SQLiteCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetValue(string sql, List<SqlHelperParameter> lstPara)
        {
            object result;
            using (this.conn = new SQLiteConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new SQLiteCommand(sql, this.conn);
                this.cmd.CommandType = CommandType.Text;
                foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                {
                    this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                }
                result = this.cmd.ExecuteScalar();
            }
            return result;
        }

        public void Dispose()
        {
            this.conn.Close();
            this.dap.Dispose();
            this.ds.Dispose();
            this.cmd.Dispose();
            this.conn.Dispose();
        }

        public Hashtable ExecProc(string procName, List<SqlHelperParameter> lstPara)
        {
            throw new NotImplementedException();
        }

        public string ExecSql(ArrayList sqlList)
        {
            string result;
            using (this.conn = new SQLiteConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                SQLiteTransaction sqliteTransaction = this.conn.BeginTransaction();
                try
                {
                    this.cmd = new SQLiteCommand
                    {
                        CommandType = CommandType.Text,
                        Connection = this.conn
                    };
                    foreach (object obj in sqlList)
                    {
                        string commandText = (string)obj;
                        this.cmd.CommandText = commandText;
                        this.cmd.ExecuteNonQuery();
                    }
                    sqliteTransaction.Commit();
                    result = string.Empty;
                }
                catch (Exception ex)
                {
                    sqliteTransaction.Rollback();
                    result = ex.Message;
                }
                finally
                {
                    sqliteTransaction.Dispose();
                }
            }
            return result;
        }

        public string ExecSql(string sql)
        {
            string result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new SQLiteCommand(sql, this.conn)
                    {
                        CommandType = CommandType.Text
                    };
                    this.cmd.ExecuteNonQuery();
                    result = string.Empty;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string ExecSql(List<ParamSql> lst)
        {
            string result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    SQLiteTransaction sqliteTransaction = this.conn.BeginTransaction();
                    try
                    {
                        this.cmd = new SQLiteCommand
                        {
                            CommandType = CommandType.Text,
                            Connection = this.conn,
                            Transaction = sqliteTransaction
                        };
                        foreach (ParamSql paramSql in lst)
                        {
                            this.cmd.CommandText = paramSql.Sql;
                            foreach (SqlHelperParameter sqlHelperParameter in paramSql.Params)
                            {
                                this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                            }
                            this.cmd.ExecuteNonQuery();
                        }
                        sqliteTransaction.Commit();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        sqliteTransaction.Rollback();
                        result = ex.Message;
                    }
                    finally
                    {
                        sqliteTransaction.Dispose();
                        this.cmd.Dispose();
                        this.conn.Close();
                    }
                }
            }
            catch (Exception ex2)
            {
                result = ex2.Message;
            }
            return result;
        }

        public string ExecSql(string sql, List<SqlHelperParameter> lstPara)
        {
            string result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new SQLiteCommand(sql, this.conn)
                    {
                        CommandText = sql
                    };
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    this.cmd.ExecuteNonQuery();
                    result = string.Empty;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public DataSet GetDataSet(string sql)
        {
            DataSet result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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

        public DataTable GetDataTable(string sql)
        {
            DataTable result;
            try
            {
                using (this.conn = new SQLiteConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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

        public object GetValue(string sql)
        {
            object result;
            using (this.conn = new SQLiteConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new SQLiteCommand(sql, this.conn)
                {
                    CommandType = CommandType.Text
                };
                result = this.cmd.ExecuteScalar();
            }
            return result;
        }

        public bool TestConnection()
        {
            SQLiteConnection sqliteConnection = new SQLiteConnection(this._connectionString);
            bool result;
            try
            {
                sqliteConnection.Open();
                sqliteConnection.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
#endif

        private SQLiteParameter GetParameter(SqlHelperParameter sqlHelperParameter)
        {
            SQLiteParameter result = new SQLiteParameter();
            return result;
        }
    }
}