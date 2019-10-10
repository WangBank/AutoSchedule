using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System;

namespace AutoSchedule.Common
{
    public static class TestConnect
    {
        public static async Task<bool> ConnectOracleAsync(string conString)
        {
            OracleConnection con = new OracleConnection();

            con.ConnectionString = conString;
            try
            {
                con.Open();
                await con.CloseAsync();
                await con.DisposeAsync();
                return true;
            }
            catch (Exception ex)
            {
                await con.CloseAsync();
                await con.DisposeAsync();
                return false;
            }
        }

        public static async Task<bool> ConnectMysqlAsync(string conString)
        {
            MySqlConnection con = new MySqlConnection();

            con.ConnectionString = conString;
            try
            {
                con.Open();
                await con.CloseAsync();
                await con.DisposeAsync();
                return true;
            }
            catch (Exception ex)
            {
                await con.CloseAsync();
                await con.DisposeAsync();
                return false;
            }
        }

        public static async Task<bool> ConnectSqlServerAsync(string conString)
        {
            SqlConnection con = new SqlConnection();

            con.ConnectionString = conString;
            try
            {
                con.Open();
                await con.CloseAsync();
                await con.DisposeAsync();
                return true;
            }
            catch (Exception ex)
            {
                await con.CloseAsync();
                await con.DisposeAsync();
                return false;
            }
        }
    }


}
