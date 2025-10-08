using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using erp_system.model;

namespace erp_system.repositories
{
    public class Payroll_Repository : Repository_Base, IRepository<Payroll_Model, int>
    {
        public int Add(Payroll_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"INSERT INTO Payrolls (PeriodStart, PeriodEnd, Bonuses, Deductions, NetPay, Generated_Date, EmployeeID)
                                    OUTPUT INSERTED.PayrollID
                                    VALUES (@PeriodStart, @PeriodEnd, @Bonuses, @Deductions, @NetPay, @Generated_Date, @EmployeeID)";
            command.Parameters.Add("@PeriodStart", SqlDbType.DateTime2).Value = entity.PeriodStart;
            command.Parameters.Add("@PeriodEnd", SqlDbType.DateTime2).Value = entity.PeriodEnd;
            command.Parameters.Add("@Bonuses", SqlDbType.Decimal).Value = entity.Bonuses;
            command.Parameters.Add("@Deductions", SqlDbType.Decimal).Value = entity.Deductions;
            command.Parameters.Add("@NetPay", SqlDbType.Decimal).Value = entity.NetPay;
            command.Parameters.Add("@Generated_Date", SqlDbType.DateTime2).Value = entity.GeneratedDate;
            command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = entity.EmployeeID;
            return (int)command.ExecuteScalar();
        }

        public void Update(Payroll_Model entity)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"UPDATE Payrolls SET PeriodStart=@PeriodStart, PeriodEnd=@PeriodEnd, Bonuses=@Bonuses, Deductions=@Deductions,
                                     NetPay=@NetPay, Generated_Date=@Generated_Date, EmployeeID=@EmployeeID
                                     WHERE PayrollID=@PayrollID";
            command.Parameters.Add("@PayrollID", SqlDbType.Int).Value = entity.PayrollID;
            command.Parameters.Add("@PeriodStart", SqlDbType.DateTime2).Value = entity.PeriodStart;
            command.Parameters.Add("@PeriodEnd", SqlDbType.DateTime2).Value = entity.PeriodEnd;
            command.Parameters.Add("@Bonuses", SqlDbType.Decimal).Value = entity.Bonuses;
            command.Parameters.Add("@Deductions", SqlDbType.Decimal).Value = entity.Deductions;
            command.Parameters.Add("@NetPay", SqlDbType.Decimal).Value = entity.NetPay;
            command.Parameters.Add("@Generated_Date", SqlDbType.DateTime2).Value = entity.GeneratedDate;
            command.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = entity.EmployeeID;
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "DELETE FROM Payrolls WHERE PayrollID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }

        public Payroll_Model? GetById(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT PayrollID, PeriodStart, PeriodEnd, Bonuses, Deductions, NetPay, Generated_Date, EmployeeID
                                     FROM Payrolls WHERE PayrollID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;
            using var rdr = command.ExecuteReader();
            if (rdr.Read())
            {
                return new Payroll_Model
                {
                    PayrollID = rdr.GetInt32(0),
                    PeriodStart = rdr.GetDateTime(1),
                    PeriodEnd = rdr.GetDateTime(2),
                    Bonuses = rdr.GetDecimal(3),
                    Deductions = rdr.GetDecimal(4),
                    NetPay = rdr.GetDecimal(5),
                    GeneratedDate = rdr.GetDateTime(6),
                    EmployeeID = rdr.GetInt32(7),
                    BasicSalary = 0 // Will be populated from Employee data
                };
            }
            return null;
        }

        public IEnumerable<Payroll_Model> GetAll()
        {
            var list = new List<Payroll_Model>();
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT PayrollID, PeriodStart, PeriodEnd, Bonuses, Deductions, NetPay, Generated_Date, EmployeeID FROM Payrolls";
            using var rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Payroll_Model
                {
                    PayrollID = rdr.GetInt32(0),
                    PeriodStart = rdr.GetDateTime(1),
                    PeriodEnd = rdr.GetDateTime(2),
                    Bonuses = rdr.GetDecimal(3),
                    Deductions = rdr.GetDecimal(4),
                    NetPay = rdr.GetDecimal(5),
                    GeneratedDate = rdr.GetDateTime(6),
                    EmployeeID = rdr.GetInt32(7),
                    BasicSalary = 0 // Will be populated from Employee data
                });
            }
            return list;
        }
    }
}


