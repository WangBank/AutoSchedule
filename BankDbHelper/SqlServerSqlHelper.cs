using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BankDbHelper
{
    internal class SqlServerSqlHelper : ISqlHelper
    {
        public string _connectionString;

        private SqlConnection conn = new SqlConnection();

        private SqlDataAdapter dap = new SqlDataAdapter();

        private DataSet ds = new DataSet();

        private SqlCommand cmd = new SqlCommand();

        public SqlServerSqlHelper(string ConnectionString)
        {
            this._connectionString = ConnectionString;
        }

        private SqlParameter GetParameter(SqlHelperParameter sqlHelperParameter)
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

#if NS21

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

        public async Task<Hashtable> ExecProcAsync(string procName, List<SqlHelperParameter> lstPara)
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
                foreach (SqlParameter sqlHelperParameter2 in this.cmd.Parameters)
                {
                    bool flag2 = sqlHelperParameter2.Direction == ParameterDirection.Output || sqlHelperParameter2.Direction == ParameterDirection.InputOutput;
                    if (flag2)
                    {
                        object obj = sqlHelperParameter2.Value;
                        bool flag3 = (obj == null || obj == DBNull.Value) && sqlHelperParameter2.DbType == DbType.String;
                        if (flag3)
                        {
                            obj = "";
                        }
                        hashtable.Add(sqlHelperParameter2.ParameterName, obj);
                    }
                }
                result = hashtable;
            }
            return result;
        }

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
                    SqlTransaction sqlTransaction = this.conn.BeginTransaction();
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        await this.conn.OpenAsync();
                    }
                    DataTable dt = new DataTable();
                    this.ds = new DataSet();
                    this.cmd = new SqlCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
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
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    await this.conn.OpenAsync();
                }
                this.cmd = new SqlCommand(sql, this.conn);
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
            SqlConnection sqlConnection = new SqlConnection(this._connectionString);
            bool result;
            try
            {
                sqlConnection.Open();
                sqlConnection.Close();
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new SqlCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    int resultInt = this.cmd.ExecuteNonQuery();
                    result = resultInt.ToString();
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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
                            this.cmd.ExecuteNonQuery();
                        }
                        sqlTransaction.Commit();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        result = ex.Message;
                    }
                    finally
                    {
                        sqlTransaction.Dispose();
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new SqlCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    int resultint = this.cmd.ExecuteNonQuery();
                    result = resultint.ToString();
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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

        public DataTable GetDataTable(string sql)
        {
            DataTable result;
            try
            {
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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

        public object GetValue(string sql)
        {
            object result;
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new SqlCommand(sql, this.conn);
                this.cmd.CommandType = CommandType.Text;
                result = this.cmd.ExecuteScalar();
            }
            return result;
        }

        public Hashtable ExecProc(string procName, List<SqlHelperParameter> lstPara)
        {
            Hashtable result;
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                Hashtable hashtable = new Hashtable();
                this.cmd = new SqlCommand(procName, this.conn);
                this.cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                {
                    this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                }
                this.cmd.ExecuteNonQuery();
                foreach (SqlParameter sqlHelperParameter2 in this.cmd.Parameters)
                {
                    bool flag2 = sqlHelperParameter2.Direction == ParameterDirection.Output || sqlHelperParameter2.Direction == ParameterDirection.InputOutput;
                    if (flag2)
                    {
                        object obj = sqlHelperParameter2.Value;
                        bool flag3 = (obj == null || obj == DBNull.Value) && sqlHelperParameter2.DbType == DbType.String;
                        if (flag3)
                        {
                            obj = "";
                        }
                        hashtable.Add(sqlHelperParameter2.ParameterName, obj);
                    }
                }
                result = hashtable;
            }
            return result;
        }

        public string ExecSql(ArrayList sqlList)
        {
            string result;
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                bool flag2 = sqlList.Count == 1;
                if (flag2)
                {
                    result = this.ExecSql(sqlList[0].ToString());
                }
                else
                {
                    SqlTransaction sqlTransaction = this.conn.BeginTransaction();
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
                            this.cmd.ExecuteNonQuery();
                        }
                        sqlTransaction.Commit();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        result = ex.Message;
                    }
                    finally
                    {
                        sqlTransaction.Dispose();
                    }
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    DataTable dt = new DataTable();
                    this.ds = new DataSet();
                    this.cmd = new SqlCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
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
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new SqlCommand(sql, this.conn);
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
            SqlConnection sqlConnection = new SqlConnection(this._connectionString);
            bool result;
            try
            {
                sqlConnection.Open();
                sqlConnection.Close();
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new SqlCommand(sql, this.conn);
                    this.cmd.CommandType = CommandType.Text;
                    int resultInt = this.cmd.ExecuteNonQuery();
                    result = resultInt.ToString();
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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
                            this.cmd.ExecuteNonQuery();
                        }
                        sqlTransaction.Commit();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        result = ex.Message;
                    }
                    finally
                    {
                        sqlTransaction.Dispose();
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    this.cmd = new SqlCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    int resultint = this.cmd.ExecuteNonQuery();
                    result = resultint.ToString();
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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

        public DataTable GetDataTable(string sql)
        {
            DataTable result;
            try
            {
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
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

        public object GetValue(string sql)
        {
            object result;
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new SqlCommand(sql, this.conn);
                this.cmd.CommandType = CommandType.Text;
                result = this.cmd.ExecuteScalar();
            }
            return result;
        }

        public Hashtable ExecProc(string procName, List<SqlHelperParameter> lstPara)
        {
            Hashtable result;
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                Hashtable hashtable = new Hashtable();
                this.cmd = new SqlCommand(procName, this.conn);
                this.cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                {
                    this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                }
                this.cmd.ExecuteNonQuery();
                foreach (SqlParameter sqlHelperParameter2 in this.cmd.Parameters)
                {
                    bool flag2 = sqlHelperParameter2.Direction == ParameterDirection.Output || sqlHelperParameter2.Direction == ParameterDirection.InputOutput;
                    if (flag2)
                    {
                        object obj = sqlHelperParameter2.Value;
                        bool flag3 = (obj == null || obj == DBNull.Value) && sqlHelperParameter2.DbType == DbType.String;
                        if (flag3)
                        {
                            obj = "";
                        }
                        hashtable.Add(sqlHelperParameter2.ParameterName, obj);
                    }
                }
                result = hashtable;
            }
            return result;
        }

        public string ExecSql(ArrayList sqlList)
        {
            string result;
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                bool flag2 = sqlList.Count == 1;
                if (flag2)
                {
                    result = this.ExecSql(sqlList[0].ToString());
                }
                else
                {
                    SqlTransaction sqlTransaction = this.conn.BeginTransaction();
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
                            this.cmd.ExecuteNonQuery();
                        }
                        sqlTransaction.Commit();
                        result = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        result = ex.Message;
                    }
                    finally
                    {
                        sqlTransaction.Dispose();
                    }
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
                using (this.conn = new SqlConnection(this._connectionString))
                {
                    bool flag = this.conn.State != ConnectionState.Open;
                    if (flag)
                    {
                        this.conn.Open();
                    }
                    DataTable dt = new DataTable();
                    this.ds = new DataSet();
                    this.cmd = new SqlCommand(sql, this.conn);
                    this.cmd.CommandText = sql;
                    foreach (SqlHelperParameter sqlHelperParameter in lstPara)
                    {
                        this.cmd.Parameters.Add(this.GetParameter(sqlHelperParameter));
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
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
            using (this.conn = new SqlConnection(this._connectionString))
            {
                bool flag = this.conn.State != ConnectionState.Open;
                if (flag)
                {
                    this.conn.Open();
                }
                this.cmd = new SqlCommand(sql, this.conn);
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
    }
}