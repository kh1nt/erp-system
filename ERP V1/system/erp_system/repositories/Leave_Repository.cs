using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using erp_system.model;

namespace erp_system.repositories
{
    public class Leave_Repository : Repository_Base, IRepository<Leave_Model, int>
    {
        public int Add(Leave_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"INSERT INTO Leaves (TypeName, MaxDays, Description)
                                    OUTPUT INSERTED.LeaveID
                                    VALUES (@TypeName, @MaxDays, @Description)";
            command.Parameters.Add("@TypeName", SqlDbType.NVarChar).Value = entity.TypeName ?? string.Empty;
            command.Parameters.Add("@MaxDays", SqlDbType.Int).Value = entity.MaxDays;
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description ?? string.Empty;
            return (int)command.ExecuteScalar();
        }

        public void Update(Leave_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"UPDATE Leaves SET TypeName=@TypeName, MaxDays=@MaxDays, Description=@Description WHERE LeaveID=@LeaveID";
            command.Parameters.Add("@LeaveID", SqlDbType.Int).Value = entity.LeaveID;
            command.Parameters.Add("@TypeName", SqlDbType.NVarChar).Value = entity.TypeName ?? string.Empty;
            command.Parameters.Add("@MaxDays", SqlDbType.Int).Value = entity.MaxDays;
            command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description ?? string.Empty;
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "DELETE FROM Leaves WHERE LeaveID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }

        public Leave_Model? GetById(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT LeaveID, TypeName, MaxDays, Description FROM Leaves WHERE LeaveID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            using var rdr = command.ExecuteReader();
            if (rdr.Read())
            {
                return new Leave_Model
                {
                    LeaveID = rdr.GetInt32(0),
                    TypeName = rdr[1]?.ToString() ?? string.Empty,
                    MaxDays = rdr.GetInt32(2),
                    Description = rdr[3]?.ToString() ?? string.Empty
                };
            }
            return null;
        }

        public IEnumerable<Leave_Model> GetAll()
        {
            var list = new List<Leave_Model>();
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT LeaveID, TypeName, MaxDays, Description FROM Leaves";
            using var rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Leave_Model
                {
                    LeaveID = rdr.GetInt32(0),
                    TypeName = rdr[1]?.ToString() ?? string.Empty,
                    MaxDays = rdr.GetInt32(2),
                    Description = rdr[3]?.ToString() ?? string.Empty
                });
            }
            return list;
        }
    }
}


