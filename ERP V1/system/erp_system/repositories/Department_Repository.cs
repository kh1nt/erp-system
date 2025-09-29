using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using erp_system.model;

namespace erp_system.repositories
{
    public class Department_Repository : Repository_Base, IRepository<Department_Model, int>
    {
        public int Add(Department_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"INSERT INTO Departments (DepartmentName, Description, CreatedDate, ManagerName)
                                    OUTPUT INSERTED.DepartmentID
                                    VALUES (@DepartmentName, @Description, @CreatedDate, @ManagerName)";
            command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar).Value = entity.DepartmentName ?? string.Empty;
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description ?? string.Empty;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime2).Value = entity.CreatedDate;
            command.Parameters.Add("@ManagerName", SqlDbType.NVarChar).Value = entity.ManagerName ?? string.Empty;
            return (int)command.ExecuteScalar();
        }

        public void Update(Department_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"UPDATE Departments
                                     SET DepartmentName=@DepartmentName, Description=@Description, CreatedDate=@CreatedDate, ManagerName=@ManagerName
                                     WHERE DepartmentID=@DepartmentID";
            command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = entity.DepartmentID;
            command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar).Value = entity.DepartmentName ?? string.Empty;
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description ?? string.Empty;
            command.Parameters.Add("@CreatedDate", SqlDbType.DateTime2).Value = entity.CreatedDate;
            command.Parameters.Add("@ManagerName", SqlDbType.NVarChar).Value = entity.ManagerName ?? string.Empty;
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "DELETE FROM Departments WHERE DepartmentID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }

        public Department_Model? GetById(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT DepartmentID, DepartmentName, Description, CreatedDate, ManagerName
                                     FROM Departments WHERE DepartmentID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            using var rdr = command.ExecuteReader();
            if (rdr.Read())
            {
                return new Department_Model
                {
                    DepartmentID = rdr.GetInt32(0),
                    DepartmentName = rdr[1]?.ToString() ?? string.Empty,
                    Description = rdr[2]?.ToString() ?? string.Empty,
                    CreatedDate = rdr.GetDateTime(3),
                    ManagerName = rdr[4]?.ToString() ?? string.Empty
                };
            }
            return null;
        }

        public IEnumerable<Department_Model> GetAll()
        {
            var list = new List<Department_Model>();
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT DepartmentID, DepartmentName, Description, CreatedDate, ManagerName FROM Departments";
            using var rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Department_Model
                {
                    DepartmentID = rdr.GetInt32(0),
                    DepartmentName = rdr[1]?.ToString() ?? string.Empty,
                    Description = rdr[2]?.ToString() ?? string.Empty,
                    CreatedDate = rdr.GetDateTime(3),
                    ManagerName = rdr[4]?.ToString() ?? string.Empty
                });
            }
            return list;
        }
    }
}


