using System;
using System.Data;
using Npgsql;

namespace quanlycitieu.DAL
{
    public static class DbConnection
    {
        public static string ConnectionString = "Host=localhost;Database=quan_ly_chi_tieu;Username=postgres;Password=manhdz123;Client Encoding=UTF8;";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        public static DataTable ExecuteQuery(string query, NpgsqlParameter[]? parameters = null)
        {
            DataTable dt = new DataTable();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static int ExecuteNonQuery(string query, NpgsqlParameter[]? parameters = null)
        {
            int rowsAffected = 0;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            return rowsAffected;
        }
        
        public static object? ExecuteScalar(string query, NpgsqlParameter[]? parameters = null)
        {
            object? result = null;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    result = cmd.ExecuteScalar();
                }
            }
            return result;
        }
    }
}
