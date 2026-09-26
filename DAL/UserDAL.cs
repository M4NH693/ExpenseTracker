using System;
using System.Data;
using Npgsql;
using quanlycitieu.DTO;

namespace quanlycitieu.DAL
{
    public class UserDAL
    {
        public DataTable GetUserByEmail(string email)
        {
            string query = "SELECT * FROM Users WHERE LOWER(Email) = LOWER(@email)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@email", email.Trim())
            };
            return DbConnection.ExecuteQuery(query, parameters);
        }

        public bool CheckEmailExists(string email)
        {
            string query = "SELECT COUNT(1) FROM Users WHERE LOWER(Email) = LOWER(@email)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@email", email.Trim())
            };
            var result = DbConnection.ExecuteScalar(query, parameters);
            return result != null && Convert.ToInt32(result) > 0;
        }

        public bool UpdatePasswordByEmail(string email, string newHashedPassword)
        {
            string query = "UPDATE Users SET Password = @p WHERE LOWER(Email) = LOWER(@e)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@p", newHashedPassword),
                new NpgsqlParameter("@e", email.Trim())
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public DataTable GetUserById(int userId)
        {
            string query = "SELECT Id, FullName, Gender, Dob, Email, Address, Password FROM Users WHERE Id = @id";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@id", userId)
            };
            return DbConnection.ExecuteQuery(query, parameters);
        }

        public bool UpdateUserProfile(int userId, string fullName, string gender, DateTime dob, string address)
        {
            string query = "UPDATE Users SET FullName = @f, Gender = @g, Dob = @d, Address = @a WHERE Id = @id";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@f", fullName),
                new NpgsqlParameter("@g", gender),
                new NpgsqlParameter("@d", DateOnly.FromDateTime(dob)),
                new NpgsqlParameter("@a", (object?)address ?? ""),
                new NpgsqlParameter("@id", userId)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdatePassword(int userId, string newHashedPassword)
        {
            string query = "UPDATE Users SET Password = @p WHERE Id = @id";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@p", newHashedPassword),
                new NpgsqlParameter("@id", userId)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool InsertUser(UserDTO user)
        {
            string query = "INSERT INTO Users (FullName, Gender, Dob, Email, Address, Password) VALUES (@f, @g, @d, @e, @a, @p)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@f", user.FullName),
                new NpgsqlParameter("@g", user.Gender),
                new NpgsqlParameter("@d", DateOnly.FromDateTime(user.DateOfBirth)),
                new NpgsqlParameter("@e", user.Email),
                new NpgsqlParameter("@a", user.Address ?? ""),
                new NpgsqlParameter("@p", user.PasswordHash)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
