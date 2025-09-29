using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using erp_system.model;

namespace erp_system.repositories
{
    public class LeaveRequest_Repository : Repository_Base, IRepository<LeaveRequest_Model, int>
    {
        public int Add(LeaveRequest_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"INSERT INTO Leave_Requests (StartDate, EndDate, RequestDate, Status, ApprovedBy, EmployeeID, LeaveID)
                                    OUTPUT INSERTED.LeaveRequestID
                                    VALUES (@StartDate, @EndDate, @RequestDate, @Status, @ApprovedBy, @EmployeeID, @LeaveID)";
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@RequestDate", SqlDbType.DateTime2).Value = entity.RequestDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar).Value = entity.Status ?? string.Empty;
            command.Parameters.Add("@ApprovedBy", SqlDbType.NVarChar).Value = entity.ApprovedBy ?? string.Empty;
            command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = entity.EmployeeID;
            command.Parameters.Add("@LeaveID", SqlDbType.Int).Value = entity.LeaveID;
            return (int)command.ExecuteScalar();
        }

        public void Update(LeaveRequest_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"UPDATE Leave_Requests SET StartDate=@StartDate, EndDate=@EndDate, RequestDate=@RequestDate,
                                     Status=@Status, ApprovedBy=@ApprovedBy, EmployeeID=@EmployeeID, LeaveID=@LeaveID
                                     WHERE LeaveRequestID=@LeaveRequestID";
            command.Parameters.Add("@LeaveRequestID", SqlDbType.Int).Value = entity.RequestID;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@RequestDate", SqlDbType.DateTime2).Value = entity.RequestDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar).Value = entity.Status ?? string.Empty;
            command.Parameters.Add("@ApprovedBy", SqlDbType.NVarChar).Value = entity.ApprovedBy ?? string.Empty;
            command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = entity.EmployeeID;
            command.Parameters.Add("@LeaveID", SqlDbType.Int).Value = entity.LeaveID;
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "DELETE FROM Leave_Requests WHERE LeaveRequestID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }

        public LeaveRequest_Model? GetById(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT LeaveRequestID, StartDate, EndDate, RequestDate, Status, ApprovedBy, EmployeeID, LeaveID
                                     FROM Leave_Requests WHERE LeaveRequestID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            using var rdr = command.ExecuteReader();
            if (rdr.Read())
            {
                return new LeaveRequest_Model
                {
                    RequestID = rdr.GetInt32(0),
                    StartDate = rdr.GetDateTime(1),
                    EndDate = rdr.GetDateTime(2),
                    RequestDate = rdr.GetDateTime(3),
                    Status = rdr[4]?.ToString() ?? string.Empty,
                    ApprovedBy = rdr[5]?.ToString() ?? string.Empty,
                    EmployeeID = rdr.GetInt32(6),
                    LeaveID = rdr.GetInt32(7)
                };
            }
            return null;
        }

        public IEnumerable<LeaveRequest_Model> GetAll()
        {
            var list = new List<LeaveRequest_Model>();
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT LeaveRequestID, StartDate, EndDate, RequestDate, Status, ApprovedBy, EmployeeID, LeaveID FROM Leave_Requests";
            using var rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new LeaveRequest_Model
                {
                    RequestID = rdr.GetInt32(0),
                    StartDate = rdr.GetDateTime(1),
                    EndDate = rdr.GetDateTime(2),
                    RequestDate = rdr.GetDateTime(3),
                    Status = rdr[4]?.ToString() ?? string.Empty,
                    ApprovedBy = rdr[5]?.ToString() ?? string.Empty,
                    EmployeeID = rdr.GetInt32(6),
                    LeaveID = rdr.GetInt32(7)
                });
            }
            return list;
        }
    }
}


