using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using erp_system.model;

namespace erp_system.repositories
{
    public class Employee_Repository : Repository_Base, IRepository<Employee_Model, int>
    {
        public int Add(Employee_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"INSERT INTO Employees (FirstName, LastName, Email, Phone, HireDate, Position, Status, DepartmentID)
                                    OUTPUT INSERTED.EmployeeID
                                    VALUES (@FirstName, @LastName, @Email, @Phone, @HireDate, @Position, @Status, @DepartmentID)";
            command.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = entity.FirstName ?? string.Empty;
            command.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = entity.LastName ?? string.Empty;
            command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = entity.Phone ?? string.Empty;
            command.Parameters.Add("@HireDate", SqlDbType.DateTime2).Value = entity.HireDate;
            command.Parameters.Add("@Position", SqlDbType.NVarChar).Value = entity.Position ?? string.Empty;
            command.Parameters.Add("@Status", SqlDbType.NVarChar).Value = entity.Status ?? string.Empty;
            command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = entity.DepartmentID;
            return (int)command.ExecuteScalar();
        }

        public void Update(Employee_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"UPDATE Employees
                                     SET FirstName=@FirstName, LastName=@LastName, Email=@Email, Phone=@Phone,
                                         HireDate=@HireDate, Position=@Position, Status=@Status, DepartmentID=@DepartmentID
                                     WHERE EmployeeID=@EmployeeID";
            command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = entity.EmployeeID;
            command.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = entity.FirstName ?? string.Empty;
            command.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = entity.LastName ?? string.Empty;
            command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = entity.Phone ?? string.Empty;
            command.Parameters.Add("@HireDate", SqlDbType.DateTime2).Value = entity.HireDate;
            command.Parameters.Add("@Position", SqlDbType.NVarChar).Value = entity.Position ?? string.Empty;
            command.Parameters.Add("@Status", SqlDbType.NVarChar).Value = entity.Status ?? string.Empty;
            command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = entity.DepartmentID;
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "DELETE FROM Employees WHERE EmployeeID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }

        public Employee_Model? GetById(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT EmployeeID, FirstName, LastName, Email, Phone, HireDate, Position, Status, DepartmentID
                                     FROM Employees WHERE EmployeeID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            using var rdr = command.ExecuteReader();
            if (rdr.Read())
            {
                return new Employee_Model
                {
                    EmployeeID = rdr.GetInt32(0),
                    FirstName = rdr[1]?.ToString() ?? string.Empty,
                    LastName = rdr[2]?.ToString() ?? string.Empty,
                    Email = rdr[3]?.ToString() ?? string.Empty,
                    Phone = rdr[4]?.ToString() ?? string.Empty,
                    HireDate = rdr.GetDateTime(5),
                    Position = rdr[6]?.ToString() ?? string.Empty,
                    Status = rdr[7]?.ToString() ?? string.Empty,
                    DepartmentID = rdr.GetInt32(8)
                };
            }
            return null;
        }

        public IEnumerable<Employee_Model> GetAll()
        {
            var list = new List<Employee_Model>();
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT EmployeeID, FirstName, LastName, Email, Phone, HireDate, Position, Status, DepartmentID FROM Employees";
            using var rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Employee_Model
                {
                    EmployeeID = rdr.GetInt32(0),
                    FirstName = rdr[1]?.ToString() ?? string.Empty,
                    LastName = rdr[2]?.ToString() ?? string.Empty,
                    Email = rdr[3]?.ToString() ?? string.Empty,
                    Phone = rdr[4]?.ToString() ?? string.Empty,
                    HireDate = rdr.GetDateTime(5),
                    Position = rdr[6]?.ToString() ?? string.Empty,
                    Status = rdr[7]?.ToString() ?? string.Empty,
                    DepartmentID = rdr.GetInt32(8)
                });
            }
            return list;
        }
    }
}


