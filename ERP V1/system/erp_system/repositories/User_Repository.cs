using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Net;
using erp_system.model;

namespace erp_system.repositories
{
    public class User_Repository : Repository_Base, IUser_Repository
    {
        public User_Repository()
        {
        }

        public void Add(User_Model user_Model)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"INSERT INTO Users (UserName, PasswordHash, RoleName, EmployeeID)
                                    OUTPUT INSERTED.UserID
                                    VALUES (@username, @passwordHash, @roleName, @employeeId)";
            command.Parameters.Add("@username", SqlDbType.NVarChar).Value = user_Model.Username ?? string.Empty;
            command.Parameters.Add("@passwordHash", SqlDbType.NVarChar).Value = HashPassword(user_Model.Password ?? string.Empty);
            command.Parameters.Add("@roleName", SqlDbType.NVarChar).Value = user_Model.Position ?? string.Empty;
            // Link to employee by EmployeeID
            command.Parameters.Add("@employeeId", SqlDbType.Int).Value = user_Model.EmployeeID;

            command.ExecuteScalar();
        }

        public bool Authenticate_User(NetworkCredential credential)
        {
            // Validate arguments
            if (string.IsNullOrEmpty(credential.UserName) || string.IsNullOrEmpty(credential.Password))
                return false;

            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "select 1 from Users where UserName=@username and PasswordHash=@password";
            command.Parameters.Add("@username", SqlDbType.NVarChar).Value = credential.UserName ?? string.Empty;
            command.Parameters.Add("@password", SqlDbType.NVarChar).Value = HashPassword(credential.Password ?? string.Empty);

            return command.ExecuteScalar() != null;
        }

        public void Edit(User_Model user_Model)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            // Update password only when provided
            if (!string.IsNullOrWhiteSpace(user_Model.Password))
            {
                command.CommandText = @"UPDATE Users
                                         SET UserName=@username, Email=@email, PasswordHash=@passwordHash
                                         WHERE UserID=@id";
                command.Parameters.Add("@passwordHash", SqlDbType.NVarChar).Value = HashPassword(user_Model.Password);
            }
            else
            {
                command.CommandText = @"UPDATE Users
                                         SET UserName=@username, Email=@email
                                         WHERE UserID=@id";
            }
            command.Parameters.Add("@id", SqlDbType.Int).Value = int.TryParse(user_Model.Id, out var uid) ? uid : 0;
            command.Parameters.Add("@username", SqlDbType.NVarChar).Value = user_Model.Username ?? string.Empty;
            command.Parameters.Add("@email", SqlDbType.NVarChar).Value = user_Model.Email ?? string.Empty;

            command.ExecuteNonQuery();
        }

        public IEnumerable<User_Model> GetAll()
        {
            var list = new List<User_Model>();
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT u.UserID, u.UserName, u.Email, e.FirstName, e.LastName
                                     FROM Users u LEFT JOIN Employees e ON u.EmployeeID = e.EmployeeID";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new User_Model
                {
                    Id = reader["UserID"]?.ToString() ?? string.Empty,
                    Username = reader["UserName"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    First_Name = reader["FirstName"]?.ToString() ?? string.Empty,
                    Last_Name = reader["LastName"]?.ToString() ?? string.Empty,
                    Password = string.Empty
                });
            }
            return list;
        }

        public User_Model GetbyId(int id)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"SELECT u.UserID, u.UserName, u.RoleName, e.FirstName, e.LastName, e.Email
                                     FROM Users u LEFT JOIN Employees e ON u.EmployeeID = e.EmployeeID
                                     WHERE u.UserID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User_Model
                {
                    Id = reader["UserID"]?.ToString() ?? string.Empty,
                    Username = reader["UserName"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    First_Name = reader["FirstName"]?.ToString() ?? string.Empty,
                    Last_Name = reader["LastName"]?.ToString() ?? string.Empty,
                    RoleName = reader["RoleName"]?.ToString() ?? string.Empty,
                    Password = string.Empty
                };
            }
            return null;
        }

        public User_Model? GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            User_Model? user = null;

            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "select u.UserID, u.UserName, u.PasswordHash, u.RoleName, u.CreatedAt, u.EmployeeID, e.FirstName, e.LastName, e.Email, e.Position, e.HireDate from Users u inner join Employees e on u.EmployeeID = e.EmployeeID where u.UserName=@username";
            command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                user = new User_Model
                {
                    Id = reader["UserID"]?.ToString() ?? string.Empty,
                    Username = reader["UserName"]?.ToString() ?? string.Empty,
                    Password = string.Empty, // don't expose password
                    First_Name = reader["FirstName"]?.ToString() ?? string.Empty,
                    Last_Name = reader["LastName"]?.ToString() ?? string.Empty,
                    Email = reader["Email"]?.ToString() ?? string.Empty,
                    Position = reader["Position"]?.ToString() ?? string.Empty,
                    RoleName = reader["RoleName"]?.ToString() ?? string.Empty,
                    HireDate = reader["HireDate"] as DateTime?,
                    EmployeeID = reader["EmployeeID"] as int? ?? 0,
                };
            }

            return user;
        }

        public void Remove(User_Model user_Model)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "DELETE FROM Users WHERE UserID=@id";
            command.Parameters.Add("@id", SqlDbType.Int).Value = int.TryParse(user_Model.Id, out var uid) ? uid : 0;
            command.ExecuteNonQuery();
        }

        public void UpdateUsername(int userId, string newUsername)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "UPDATE Users SET UserName = @username WHERE UserID = @userId";
            command.Parameters.Add("@username", SqlDbType.NVarChar).Value = newUsername ?? string.Empty;
            command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
            command.ExecuteNonQuery();
        }

        public void UpdatePassword(int userId, string newPassword)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "UPDATE Users SET PasswordHash = @passwordHash WHERE UserID = @userId";
            command.Parameters.Add("@passwordHash", SqlDbType.NVarChar).Value = HashPassword(newPassword ?? string.Empty);
            command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
            command.ExecuteNonQuery();
        }

        public bool VerifyPassword(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "SELECT 1 FROM Users WHERE UserName = @username AND PasswordHash = @password";
            command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
            command.Parameters.Add("@password", SqlDbType.NVarChar).Value = HashPassword(password);

            using var reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public void UpdateEmployeeName(int employeeId, string firstName, string lastName)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = "UPDATE Employees SET FirstName = @firstName, LastName = @lastName WHERE EmployeeID = @employeeId";
            command.Parameters.Add("@firstName", SqlDbType.NVarChar).Value = firstName ?? string.Empty;
            command.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = lastName ?? string.Empty;
            command.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;
            command.ExecuteNonQuery();
        }

        public User_Model? GetByEmployeeId(int employeeId)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();

            command.Connection = connection;
            command.CommandText = @"
                SELECT u.UserID, u.UserName, u.PasswordHash, u.RoleName, u.CreatedAt, u.EmployeeID,
                       e.FirstName, e.LastName, e.Email, e.Position, e.HireDate
                FROM Users u
                INNER JOIN Employees e ON u.EmployeeID = e.EmployeeID
                WHERE u.EmployeeID = @employeeId";

            command.Parameters.Add("@employeeId", SqlDbType.Int).Value = employeeId;

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User_Model
                {
                    Id = reader.GetInt32(0).ToString(),
                    Username = reader[1]?.ToString() ?? string.Empty,
                    Password = string.Empty, // Don't return password
                    First_Name = reader[6]?.ToString() ?? string.Empty,
                    Last_Name = reader[7]?.ToString() ?? string.Empty,
                    Email = reader[8]?.ToString() ?? string.Empty,
                    Position = reader[9]?.ToString() ?? string.Empty,
                    HireDate = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                    EmployeeID = reader.GetInt32(5)
                };
            }

            return null;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password ?? string.Empty));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}