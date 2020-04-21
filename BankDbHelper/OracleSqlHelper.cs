using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BankDbHelper
{
    internal class OracleSqlHelper : ISqlHelper
    {
        public OracleSqlHelper(string ConnectionString)
        {
            this._connectionString = ConnectionString;
        }

        private OracleParameter GetParameter(SqlHelperParameter sqlHelperParameter)
        {
            OracleParameter oracleParameter = new OracleParameter();
            OracleDbType oracleDbType = OracleDbType.Varchar2;
            switch (sqlHelperParameter.DataType)
            {
                case ParamsType.Varchar:
                    oracleDbType = OracleDbType.Varchar2;
                    break;

                case ParamsType.Int:
                    oracleDbType = OracleDbType.Int16;
                    break;

                case ParamsType.DateTime:
                    oracleDbType = OracleDbType.Date;
                    break;

                case ParamsType.Decimal:
                    oracleDbType = OracleDbType.Decimal;
                    break;

                case ParamsType.Blob:
                    oracleDbType = OracleDbType.Blob;
                    break;
            }
            oracleParameter.ParameterName = sqlHelperParameter.Name;
            oracleParameter.OracleDbType = oracleDbType;
            oracleParameter.Direction = sqlHelperParameter.Direction;
            oracleParameter.Value = sqlHelperParameter.Value;
            oracleParameter.Size = sqlHelperParameter.Size;
            return oracleParameter;
        }

#if NS21

        public async Task<bool> TestConnectionAsync()
        {
            OracleConnection oracleConnection = new OracleConnection(this._connectionString);
            bool result;
            try
            {
                await oracleConnection.OpenAsync();
                await oracleConnection.CloseAsync();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// exec sql,return error message
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<string> ExecSqlAsync(string sql)
        {
            string result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.cmd = new OracleCommand(sql, this.conn);
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    OracleTransaction oracleTransaction = this.conn.BeginTransaction();
                    try
                    {
                        this.cmd = new OracleCommand();
                        this.cmd.CommandType = CommandType.Text;
                        this.cmd.Connection = this.conn;
                        this.cmd.Transaction = oracleTransaction;
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
                        await oracleTransaction.CommitAsync();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        await oracleTransaction.RollbackAsync();
                        result = ex.Message;
                    }
                    finally
                    {
                        await oracleTransaction.DisposeAsync();
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.cmd = new OracleCommand(sql, this.conn);
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.ds = new DataSet();
                    this.dap = new OracleDataAdapter(sql, this.conn);
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.ds = new DataSet();
                    this.dap = new OracleDataAdapter(sql, this.conn);
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    result = await this.cmd.ExecuteScalarAsync();
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    Hashtable hashtable = new Hashtable();
                    this.cmd = new OracleCommand(procName, this.conn);
                    this.cmd.CommandType = CommandType.StoredProcedure;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    await this.cmd.ExecuteNonQueryAsync();
                    foreach (OracleParameter sqlHelperParameter2 in this.cmd.Parameters)
                    {
                        bool flag2 = sqlHelperParameter2.Direction == ParameterDirection.Output || sqlHelperParameter2.Direction == ParameterDirection.InputOutput;
                        if (flag2)
                        {
                            object obj = sqlHelperParameter2.Value;
                            bool flag3 = (obj == null || obj == DBNull.Value) && sqlHelperParameter2.OracleDbType == OracleDbType.Varchar2;
                            if (flag3)
                            {
                                obj = "";
                            }
                            hashtable.Add(sqlHelperParameter2.ParameterName, obj);
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
            using (this.conn = new OracleConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    await this.conn.OpenAsync();
                }
                OracleTransaction oracleTransaction = this.conn.BeginTransaction();
                try
                {
                    this.cmd = new OracleCommand();
                    this.cmd.CommandType = CommandType.Text;
                    this.cmd.Connection = this.conn;
                    foreach (object obj in sqlList)
                    {
                        string commandText = (string)obj;
                        this.cmd.CommandText = commandText;
                        await this.cmd.ExecuteNonQueryAsync();
                    }
                    await oracleTransaction.CommitAsync();
                    result = string.Empty;
                }
                catch (Exception ex)
                {
                    await oracleTransaction.RollbackAsync();
                    result = ex.Message;
                }
                finally
                {
                    await oracleTransaction.DisposeAsync();
                    await this.cmd.DisposeAsync();
                    await this.conn.CloseAsync();
                }
            }
            return result;
        }

        public async Task DisposeAsync()
        {
            await this.conn.CloseAsync();
            await this.cmd.DisposeAsync();
            await this.conn.DisposeAsync();
        }

        public async Task<DataTable> GetDataTableAsync(string sql, List<SqlHelperParameter> lstPara)
        {
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    DataTable dt = new DataTable();
                    this.ds = new DataSet();
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
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
            using (this.conn = new OracleConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    await this.conn.OpenAsync();
                }
                this.cmd = new OracleCommand(sql, this.conn);
                this.cmd.CommandType = CommandType.Text;
                foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                {
                    this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                }
                result = await this.cmd.ExecuteScalarAsync();
            }
            return result;
        }

        public bool TestConnection()
        {
            OracleConnection oracleConnection = new OracleConnection(this._connectionString);
            bool result;
            try
            {
                oracleConnection.Open();
                oracleConnection.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public string ExecSql(string sql)
        {
            string result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    this.cmd.ExecuteNonQuery();
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
                    this.cmd.Dispose();
                }
            }
            return result;
        }

        public string ExecSql(List<ParamSql> lst)
        {
            string result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    OracleTransaction oracleTransaction = this.conn.BeginTransaction();
                    try
                    {
                        this.cmd = new OracleCommand();
                        this.cmd.CommandType = CommandType.Text;
                        this.cmd.Connection = this.conn;
                        this.cmd.Transaction = oracleTransaction;
                        foreach (ParamSql paramSql in lst)
                        {
                            this.cmd.CommandText = paramSql.Sql;
                            foreach (SqlHelperParameter sqlHelperParameter in paramSql.Params)
                            {
                                this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                            }
                            this.cmd.ExecuteNonQuery();
                            this.cmd.Parameters.Clear();
                        }
                        oracleTransaction.Commit();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        oracleTransaction.Rollback();
                        result = ex.Message;
                    }
                    finally
                    {
                        oracleTransaction.Dispose();
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.ds = new DataSet();
                    this.dap = new OracleDataAdapter(sql, this.conn);
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

        public DataTable GetDataTable(string sql)
        {
            DataTable result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.ds = new DataSet();
                    this.dap = new OracleDataAdapter(sql, this.conn);
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

        public object GetValue(string sql)
        {
            object result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    result = this.cmd.ExecuteScalar();
                }
            }
            finally
            {
                bool flag2 = this.cmd != null;
                if (flag2)
                {
                    this.cmd.Dispose();
                }
            }
            return result;
        }

        public Hashtable ExecProc(string procName, List<SqlHelperParameter> lstPara)
        {
            Hashtable result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    Hashtable hashtable = new Hashtable();
                    this.cmd = new OracleCommand(procName, this.conn);
                    this.cmd.CommandType = CommandType.StoredProcedure;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    this.cmd.ExecuteNonQuery();
                    foreach (OracleParameter sqlHelperParameter2 in this.cmd.Parameters)
                    {
                        bool flag2 = sqlHelperParameter2.Direction == ParameterDirection.Output || sqlHelperParameter2.Direction == ParameterDirection.InputOutput;
                        if (flag2)
                        {
                            object obj = sqlHelperParameter2.Value;
                            bool flag3 = (obj == null || obj == DBNull.Value) && sqlHelperParameter2.OracleDbType == OracleDbType.Varchar2;
                            if (flag3)
                            {
                                obj = "";
                            }
                            hashtable.Add(sqlHelperParameter2.ParameterName, obj);
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
                    this.cmd.Dispose();
                }
            }
            return result;
        }

        public string ExecSql(ArrayList sqlList)
        {
            string result;
            using (this.conn = new OracleConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                OracleTransaction oracleTransaction = this.conn.BeginTransaction();
                try
                {
                    this.cmd = new OracleCommand();
                    this.cmd.CommandType = CommandType.Text;
                    this.cmd.Connection = this.conn;
                    foreach (object obj in sqlList)
                    {
                        string commandText = (string)obj;
                        this.cmd.CommandText = commandText;
                        this.cmd.ExecuteNonQuery();
                    }
                    oracleTransaction.Commit();
                    result = string.Empty;
                }
                catch (Exception ex)
                {
                    oracleTransaction.Rollback();
                    result = ex.Message;
                }
                finally
                {
                    oracleTransaction.Dispose();
                    this.cmd.Dispose();
                    this.conn.Close();
                }
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

        public DataTable GetDataTable(string sql, List<SqlHelperParameter> lstPara)
        {
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    DataTable dt = new DataTable();
                    this.ds = new DataSet();
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
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
            using (this.conn = new OracleConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new OracleCommand(sql, this.conn);
                this.cmd.CommandType = CommandType.Text;
                foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                {
                    this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                }
                result = this.cmd.ExecuteScalar();
            }
            return result;
        }

#else
        public bool TestConnection()
        {
            OracleConnection oracleConnection = new OracleConnection(this._connectionString);
            bool result;
            try
            {
                oracleConnection.Open();
                oracleConnection.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public string ExecSql(string sql)
        {
            string result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    this.cmd.ExecuteNonQuery();
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
                    this.cmd.Dispose();
                }
            }
            return result;
        }

        public string ExecSql(List<ParamSql> lst)
        {
            string result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    OracleTransaction oracleTransaction = this.conn.BeginTransaction();
                    try
                    {
                        this.cmd = new OracleCommand();
                        this.cmd.CommandType = CommandType.Text;
                        this.cmd.Connection = this.conn;
                        this.cmd.Transaction = oracleTransaction;
                        foreach (ParamSql paramSql in lst)
                        {
                            this.cmd.CommandText = paramSql.Sql;
                            foreach (SqlHelperParameter sqlHelperParameter in paramSql.Params)
                            {
                                this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                            }
                            this.cmd.ExecuteNonQuery();
                            this.cmd.Parameters.Clear();
                        }
                        oracleTransaction.Commit();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        oracleTransaction.Rollback();
                        result = ex.Message;
                    }
                    finally
                    {
                        oracleTransaction.Dispose();
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
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
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.ds = new DataSet();
                    this.dap = new OracleDataAdapter(sql, this.conn);
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

        public DataTable GetDataTable(string sql)
        {
            DataTable result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.ds = new DataSet();
                    this.dap = new OracleDataAdapter(sql, this.conn);
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

        public object GetValue(string sql)
        {
            object result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    result = this.cmd.ExecuteScalar();
                }
            }
            finally
            {
                bool flag2 = this.cmd != null;
                if (flag2)
                {
                    this.cmd.Dispose();
                }
            }
            return result;
        }

        public Hashtable ExecProc(string procName, List<SqlHelperParameter> lstPara)
        {
            Hashtable result;
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    Hashtable hashtable = new Hashtable();
                    this.cmd = new OracleCommand(procName, this.conn);
                    this.cmd.CommandType = CommandType.StoredProcedure;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    this.cmd.ExecuteNonQuery();
                    foreach (OracleParameter sqlHelperParameter2 in this.cmd.Parameters)
                    {
                        bool flag2 = sqlHelperParameter2.Direction == ParameterDirection.Output || sqlHelperParameter2.Direction == ParameterDirection.InputOutput;
                        if (flag2)
                        {
                            object obj = sqlHelperParameter2.Value;
                            bool flag3 = (obj == null || obj == DBNull.Value) && sqlHelperParameter2.OracleDbType == OracleDbType.Varchar2;
                            if (flag3)
                            {
                                obj = "";
                            }
                            hashtable.Add(sqlHelperParameter2.ParameterName, obj);
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
                    this.cmd.Dispose();
                }
            }
            return result;
        }

        public string ExecSql(ArrayList sqlList)
        {
            string result;
            using (this.conn = new OracleConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                OracleTransaction oracleTransaction = this.conn.BeginTransaction();
                try
                {
                    this.cmd = new OracleCommand();
                    this.cmd.CommandType = CommandType.Text;
                    this.cmd.Connection = this.conn;
                    foreach (object obj in sqlList)
                    {
                        string commandText = (string)obj;
                        this.cmd.CommandText = commandText;
                        this.cmd.ExecuteNonQuery();
                    }
                    oracleTransaction.Commit();
                    result = string.Empty;
                }
                catch (Exception ex)
                {
                    oracleTransaction.Rollback();
                    result = ex.Message;
                }
                finally
                {
                    oracleTransaction.Dispose();
                    this.cmd.Dispose();
                    this.conn.Close();
                }
            }
            return result;
        }

        public void Dispose()
        {
            this.conn.Close();
            this.cmd.Dispose();
            this.conn.Dispose();
        }

        public DataTable GetDataTable(string sql, List<SqlHelperParameter> lstPara)
        {
            try
            {
                using (this.conn = new OracleConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    DataTable dt = new DataTable();
                    this.ds = new DataSet();
                    this.cmd = new OracleCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
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
            using (this.conn = new OracleConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new OracleCommand(sql, this.conn);
                this.cmd.CommandType = CommandType.Text;
                foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                {
                    this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                }
                result = this.cmd.ExecuteScalar();
            }
            return result;
        }
#endif

        public string _connectionString;

        private OracleConnection conn = new OracleConnection();

        private OracleDataAdapter dap = new OracleDataAdapter();

        private DataSet ds = new DataSet();

        private OracleCommand cmd = new OracleCommand();

        public OracleParameter Parameter = null;
    }
}