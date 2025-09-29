using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using erp_system.model;

namespace erp_system.repositories
{
    public class Performance_Repository : Repository_Base, IRepository<Performance_Model, int>
    {
        public int Add(Performance_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"INSERT INTO Performance_Records (ReviewDate, Score, Remarks, EmployeeID)
                                    OUTPUT INSERTED.PerformanceRecordID
                                    VALUES (@ReviewDate, @Score, @Remarks, @EmployeeID)";
            command.Parameters.Add("@ReviewDate", SqlDbType.DateTime2).Value = entity.ReviewDate;
            command.Parameters.Add("@Score", SqlDbType.Decimal).Value = entity.Score;
            command.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = entity.Remarks ?? string.Empty;
            command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = entity.EmployeeID;
            return (int)command.ExecuteScalar();
        }

        public void Update(Performance_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"UPDATE Performance_Records SET ReviewDate=@ReviewDate, Score=@Score, Remarks=@Remarks, EmployeeID=@EmployeeID
                                     WHERE PerformanceRecordID=@PerformanceRecordID";
            command.Parameters.Add("@PerformanceRecordID", SqlDbType.Int).Value = entity.RecordID;
            command.Parameters.Add("@ReviewDate", SqlDbType.DateTime2).Value = entity.ReviewDate;
            command.Parameters.Add("@Score", SqlDbType.Decimal).Value = entity.Score;
            command.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = entity.Remarks ?? string.Empty;
            command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = entity.EmployeeID;
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "DELETE FROM Performance_Records WHERE PerformanceRecordID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }

        public Performance_Model? GetById(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT PerformanceRecordID, ReviewDate, Score, Remarks, EmployeeID
                                     FROM Performance_Records WHERE PerformanceRecordID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            using var rdr = command.ExecuteReader();
            if (rdr.Read())
            {
                return new Performance_Model
                {
                    RecordID = rdr.GetInt32(0),
                    ReviewDate = rdr.GetDateTime(1),
                    Score = rdr.GetDecimal(2),
                    Remarks = rdr[3]?.ToString() ?? string.Empty,
                    EmployeeID = rdr.GetInt32(4)
                };
            }
            return null;
        }

        public IEnumerable<Performance_Model> GetAll()
        {
            var list = new List<Performance_Model>();
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT PerformanceRecordID, ReviewDate, Score, Remarks, EmployeeID FROM Performance_Records";
            using var rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Performance_Model
                {
                    RecordID = rdr.GetInt32(0),
                    ReviewDate = rdr.GetDateTime(1),
                    Score = rdr.GetDecimal(2),
                    Remarks = rdr[3]?.ToString() ?? string.Empty,
                    EmployeeID = rdr.GetInt32(4)
                });
            }
            return list;
        }
    }
}


