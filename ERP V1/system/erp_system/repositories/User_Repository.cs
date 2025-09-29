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
            command.CommandText = @"INSERT INTO Users (UserName, PasswordHash, Email, EmployeeID)
                                    OUTPUT INSERTED.UserID
                                    VALUES (@username, @passwordHash, @email, @employeeId)";
            command.Parameters.Add("@username", SqlDbType.NVarChar).Value = user_Model.Username ?? string.Empty;
            command.Parameters.Add("@passwordHash", SqlDbType.NVarChar).Value = HashPassword(user_Model.Password ?? string.Empty);
            command.Parameters.Add("@email", SqlDbType.NVarChar).Value = user_Model.Email ?? string.Empty;
            // Optional: link to employee by email or leave null if unknown
            command.Parameters.Add("@employeeId", SqlDbType.Int).Value = DBNull.Value;

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
            // Hash the password for comparison
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(credential.Password ?? string.Empty));
                var hashedPassword = Convert.ToBase64String(hashedBytes);
                command.Parameters.Add("@password", SqlDbType.NVarChar).Value = hashedPassword;
            }

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
            command.CommandText = @"SELECT u.UserID, u.UserName, u.Email, e.FirstName, e.LastName
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
            command.CommandText = "select u.*, e.FirstName, e.LastName, e.Email from Users u inner join Employees e on u.EmployeeID = e.EmployeeID where u.UserName=@username";
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

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password ?? string.Empty));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}