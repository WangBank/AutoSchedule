using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BankDbHelper
{
    internal class MySqlHelper : ISqlHelper
    {
        public MySqlHelper(string ConnectionString)
        {
            this._connectionString = ConnectionString;
        }

        public async Task<bool> TestConnectionAsync()
        {
            MySqlConnection mysqlConnection = new MySqlConnection(this._connectionString);
            bool result;
            try
            {
                await mysqlConnection.OpenAsync();
                await mysqlConnection.CloseAsync();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public async Task<string> ExecSqlAsync(string sql)
        {
            string result;
            try
            {
                using (this.conn = new MySqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.cmd = new MySqlCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    await this.cmd.ExecuteNonQueryAsync();
                }
                result = string.Empty;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                bool flag2 = this.cmd != null;
                if (flag2)
                {
                    await this.cmd.DisposeAsync();
                }
            }
            return result;
        }

        public async Task<string> ExecSqlAsync(List<ParamSql> lst)
        {
            string result;
            try
            {
                using (this.conn = new MySqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    MySqlTransaction mysqlTransaction = this.conn.BeginTransaction();
                    try
                    {
                        this.cmd = new MySqlCommand();
                        this.cmd.CommandType = CommandType.Text;
                        this.cmd.Connection = this.conn;
                        this.cmd.Transaction = mysqlTransaction;
                        foreach (ParamSql paramSql in lst)
                        {
                            this.cmd.CommandText = paramSql.Sql;
                            foreach (SqlHelperParameter sqlHelperParameter in paramSql.Params)
                            {
                                this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                            }
                            await this.cmd.ExecuteNonQueryAsync();
                            this.cmd.Parameters.Clear();
                        }
                        await mysqlTransaction.CommitAsync();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        await mysqlTransaction.RollbackAsync();
                        result = ex.Message;
                    }
                    finally
                    {
                        await mysqlTransaction.DisposeAsync();
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
                using (this.conn = new MySqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.cmd = new MySqlCommand(sql, this.conn);
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

        public async Task<DataSet> GetDataSetAsync(string sql)
        {
            DataSet result;
            try
            {
                using (this.conn = new MySqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.ds = new DataSet();
                    this.dap = new MySqlDataAdapter(sql, this.conn);
                    this.dap.Fill(this.ds);
                }
                result = this.ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bool flag2 = this.dap != null;
                if (flag2)
                {
                    this.dap.Dispose();
                }
            }
            return result;
        }

        public async Task<DataTable> GetDataTableAsync(string sql)
        {
            DataTable result;
            try
            {
                using (this.conn = new MySqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.ds = new DataSet();
                    this.dap = new MySqlDataAdapter(sql, this.conn);
                    this.dap.Fill(this.ds);
                    result = this.ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bool flag2 = this.dap != null;
                if (flag2)
                {
                    this.dap.Dispose();
                }
            }
            return result;
        }

        public async Task<object> GetValueAsync(string sql)
        {
            object result;
            try
            {
                using (this.conn = new MySqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.cmd = new MySqlCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    result = this.cmd.ExecuteScalarAsync();
                }
            }
            finally
            {
                bool flag2 = this.cmd != null;
                if (flag2)
                {
                    await this.cmd.DisposeAsync();
                }
            }
            return result;
        }

        public async Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara)
        {
            Hashtable result;
            try
            {
                using (this.conn = new MySqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    Hashtable hashtable = new Hashtable();
                    this.cmd = new MySqlCommand(procName, this.conn);
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
            }
            finally
            {
                bool flag4 = this.cmd != null;
                if (flag4)
                {
                    await this.cmd.DisposeAsync();
                }
            }
            return result;
        }

        public async Task<string> ExecSqlAsync(ArrayList sqlList)
        {
            string result;
            using (this.conn = new MySqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    await this.conn.OpenAsync();
                }
                MySqlTransaction mysqlTransaction = this.conn.BeginTransaction();
                try
                {
                    this.cmd = new MySqlCommand();
                    this.cmd.CommandType = CommandType.Text;
                    this.cmd.Connection = this.conn;
                    foreach (object obj in sqlList)
                    {
                        string commandText = (string)obj;
                        this.cmd.CommandText = commandText;
                        await this.cmd.ExecuteNonQueryAsync();
                    }
                    await mysqlTransaction.CommitAsync();
                    result = string.Empty;
                }
                catch (Exception ex)
                {
                    mysqlTransaction.Rollback();
                    result = ex.Message;
                }
                finally
                {
                    await mysqlTransaction.DisposeAsync();
                    await this.cmd.DisposeAsync();
                    await this.conn.CloseAsync();
                }
            }
            return result;
        }

        private MySqlParameter GetParameter(SqlHelperParameter sqlHelperParameter)
        {
            MySqlParameter mysqlParameter = new MySqlParameter();
            MySqlDbType mysqlDbType = MySqlDbType.VarChar;
            switch (sqlHelperParameter.DataType)
            {
                case ParamsType.Varchar:
                    mysqlDbType = MySqlDbType.VarChar;
                    break;

                case ParamsType.Int:
                    mysqlDbType = MySqlDbType.Int16;
                    break;

                case ParamsType.DateTime:
                    mysqlDbType = MySqlDbType.Date;
                    break;

                case ParamsType.Decimal:
                    mysqlDbType = MySqlDbType.Decimal;
                    break;

                case ParamsType.Blob:
                    mysqlDbType = MySqlDbType.Blob;
                    break;
            }
            mysqlParameter.ParameterName = sqlHelperParameter.Name;
            mysqlParameter.MySqlDbType = mysqlDbType;
            mysqlParameter.Direction = sqlHelperParameter.Direction;
            mysqlParameter.Value = sqlHelperParameter.Value;
            mysqlParameter.Size = sqlHelperParameter.Size;
            return mysqlParameter;
        }

        public async Task DisposeAsync()
        {
            await this.conn.CloseAsync();
            this.dap.Dispose();
            this.ds.Dispose();
            await this.cmd.DisposeAsync();
            await this.conn.DisposeAsync();
        }

        public string _connectionString;

        private MySqlConnection conn = new MySqlConnection();

        private MySqlDataAdapter dap = new MySqlDataAdapter();

        private DataSet ds = new DataSet();

        private MySqlCommand cmd = new MySqlCommand();

        public MySqlParameter Parameter = null;
    }
}