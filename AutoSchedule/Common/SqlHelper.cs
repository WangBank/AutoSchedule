using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System;

namespace AutoSchedule.Common
{
    public class SqlHelper
    {
        public OracleConnection conOracle = new OracleConnection();
        public MySqlConnection conMySql = new MySqlConnection();
        public SqlConnection conSql = new SqlConnection();
        public string conString;
        public SqlHelper(string _conString)
        {
            conString = _conString;
        }
        public  async Task<bool> ConnectOracleAsync()
        {
            conOracle.ConnectionString = conString;
            try
            {
                await conOracle.OpenAsync();
                await conOracle.CloseAsync();
                await conOracle.DisposeAsync();
                return true;
            }
            catch (Exception ex)
            {
                await conOracle.CloseAsync();
                await conOracle.DisposeAsync();
                return false;
            }
        }

        public  async Task<bool> ConnectMysqlAsync()
        {


            conMySql.ConnectionString = conString;
            try
            {
                await conMySql.OpenAsync();
                await conMySql.CloseAsync();
                await conMySql.DisposeAsync();
                return true;
            }
            catch (Exception ex)
            {
                await conMySql.CloseAsync();
                await conMySql.DisposeAsync();
                return false;
            }
        }

        public  async Task<bool> ConnectSqlServerAsync()
        {


            conSql.ConnectionString = conString;
            try
            {
                await conSql.OpenAsync();
                await conSql.CloseAsync();
                await conSql.DisposeAsync();
                return true;
            }
            catch (Exception ex)
            {
                await conSql.CloseAsync();
                await conSql.DisposeAsync();
                return false;
            }
        }

        public async Task<bool> GetValueAsyncOracle(string sql)
        {
            conOracle.ConnectionString = conString;
            try
            {
                await conOracle.OpenAsync();
                OracleCommand cmd = conOracle.CreateCommand();
                cmd.CommandText = "select * from emp";
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    Console.WriteLine(reader.GetInt32(0));
                reader.Dispose();
                cmd.Dispose();
                conOracle.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                await conOracle.CloseAsync();
                await conOracle.DisposeAsync();
                return false;
            }
        }
    }


}
