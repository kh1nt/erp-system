using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using erp_system.model;

namespace erp_system.repositories
{
    public class Sales_Repository : Repository_Base, IRepository<Sales_Model, int>
    {
        public int Add(Sales_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"INSERT INTO Sales (SaleDate, Amount, Description, EmployeeID)
                                    OUTPUT INSERTED.SalesID
                                    VALUES (@SaleDate, @Amount, @Description, @EmployeeID)";
            command.Parameters.Add("@SaleDate", SqlDbType.DateTime2).Value = entity.SaleDate;
            command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = entity.Amount;
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description ?? string.Empty;
            command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = entity.EmployeeID;
            return (int)command.ExecuteScalar();
        }

        public void Update(Sales_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"UPDATE Sales SET SaleDate=@SaleDate, Amount=@Amount, Description=@Description, EmployeeID=@EmployeeID
                                     WHERE SalesID=@SalesID";
            command.Parameters.Add("@SalesID", SqlDbType.Int).Value = entity.SaleID;
            command.Parameters.Add("@SaleDate", SqlDbType.DateTime2).Value = entity.SaleDate;
            command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = entity.Amount;
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description ?? string.Empty;
            command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = entity.EmployeeID;
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "DELETE FROM Sales WHERE SalesID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }

        public Sales_Model? GetById(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT SalesID, SaleDate, Amount, Description, EmployeeID FROM Sales WHERE SalesID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            using var rdr = command.ExecuteReader();
            if (rdr.Read())
            {
                return new Sales_Model
                {
                    SaleID = rdr.GetInt32(0),
                    SaleDate = rdr.GetDateTime(1),
                    Amount = rdr.GetDecimal(2),
                    Description = rdr[3]?.ToString() ?? string.Empty,
                    EmployeeID = rdr.GetInt32(4)
                };
            }
            return null;
        }

        public IEnumerable<Sales_Model> GetAll()
        {
            var list = new List<Sales_Model>();
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT SalesID, SaleDate, Amount, Description, EmployeeID FROM Sales";
            using var rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Sales_Model
                {
                    SaleID = rdr.GetInt32(0),
                    SaleDate = rdr.GetDateTime(1),
                    Amount = rdr.GetDecimal(2),
                    Description = rdr[3]?.ToString() ?? string.Empty,
                    EmployeeID = rdr.GetInt32(4)
                });
            }
            return list;
        }
    }
}


