using System;
using System.Windows;
using erp_system.Services;

namespace erp_system.view
{
    public partial class DatabaseTest_View : Window
    {
        private readonly DatabaseTestService _dbTestService;

        public DatabaseTest_View()
        {
            InitializeComponent();
            _dbTestService = new DatabaseTestService();
            TestConnection();
        }

        private void TestConnection()
        {
            try
            {
                // Test basic connection
                var isConnected = _dbTestService.TestConnection();
                ConnectionStatus.Text = isConnected ? "✅ Connected Successfully" : "❌ Connection Failed";
                ConnectionStatus.Foreground = isConnected ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;

                if (isConnected)
                {
                    TestTables();
                    TestRecordCounts();
                    TestSampleUsers();
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus.Text = $"❌ Error: {ex.Message}";
                ConnectionStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void TestTables()
        {
            try
            {
                var tables = new[] { "Users", "Employees", "Departments", "Leave_Requests", "Performance_Records", "Payrolls", "Sales" };
                var results = new System.Text.StringBuilder();

                foreach (var table in tables)
                {
                    var exists = _dbTestService.TestTableExists(table);
                    results.AppendLine($"{table}: {(exists ? "✅" : "❌")}");
                }

                TableStatus.Text = results.ToString();
                TableStatus.Foreground = System.Windows.Media.Brushes.Black;
            }
            catch (Exception ex)
            {
                TableStatus.Text = $"Error checking tables: {ex.Message}";
                TableStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void TestRecordCounts()
        {
            try
            {
                var tables = new[] { "Users", "Employees", "Departments", "Leave_Requests", "Performance_Records", "Payrolls", "Sales" };
                var results = new System.Text.StringBuilder();

                foreach (var table in tables)
                {
                    var count = _dbTestService.GetRecordCount(table);
                    results.AppendLine($"{table}: {count} records");
                }

                RecordCounts.Text = results.ToString();
                RecordCounts.Foreground = System.Windows.Media.Brushes.Black;
            }
            catch (Exception ex)
            {
                RecordCounts.Text = $"Error counting records: {ex.Message}";
                RecordCounts.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void TestSampleUsers()
        {
            try
            {
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(_dbTestService._connectionString);
                connection.Open();
                
                var command = new Microsoft.Data.SqlClient.SqlCommand(
                    "SELECT TOP 5 UserName, RoleName FROM Users", connection);
                
                using var reader = command.ExecuteReader();
                var results = new System.Text.StringBuilder();
                
                while (reader.Read())
                {
                    results.AppendLine($"Username: {reader["UserName"]}, Role: {reader["RoleName"]}");
                }
                
                SampleUsers.Text = results.Length > 0 ? results.ToString() : "No users found";
                SampleUsers.Foreground = System.Windows.Media.Brushes.Black;
            }
            catch (Exception ex)
            {
                SampleUsers.Text = $"Error loading users: {ex.Message}";
                SampleUsers.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            TestConnection();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
