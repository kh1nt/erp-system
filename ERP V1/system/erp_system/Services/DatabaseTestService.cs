using Microsoft.Data.SqlClient;
using System;
using erp_system.Configuration;

namespace erp_system.Services
{
    public class DatabaseTestService
    {
        public readonly string _connectionString;

        public DatabaseTestService()
        {
            var fromConfig = AppConfig.GetConnectionString();
            _connectionString = string.IsNullOrWhiteSpace(fromConfig)
                ? @"Data Source=DESKTOP-I5DCKI6\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
                : fromConfig;
        }

        public bool TestConnection()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }

        public bool TestTableExists(string tableName)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                
                var command = new SqlCommand(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName", 
                    connection);
                command.Parameters.AddWithValue("@tableName", tableName);
                
                var count = (int)command.ExecuteScalar();
                return count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking table {tableName}: {ex.Message}");
                return false;
            }
        }

        public int GetRecordCount(string tableName)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                
                var command = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", connection);
                return (int)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting record count for {tableName}: {ex.Message}");
                return 0;
            }
        }
    }
}
