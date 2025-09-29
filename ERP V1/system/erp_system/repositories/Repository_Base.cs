using Microsoft.Data.SqlClient;
using erp_system.Configuration;

namespace erp_system.repositories
{
    public abstract class Repository_Base
    {
        private readonly string _connection_string;
        public Repository_Base()
        {
            var fromConfig = AppConfig.GetConnectionString();
            _connection_string = string.IsNullOrWhiteSpace(fromConfig)
                ? @"Data Source=DESKTOP-I5DCKI6\SQLEXPRESS;Initial Catalog=avielle;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
                : fromConfig;
        }
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connection_string);
        }
    }
}
