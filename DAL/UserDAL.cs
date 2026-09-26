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
                new NpgsqlParameter("@email", email)
            };
            return DbConnection.ExecuteQuery(query, parameters);
        }

        public bool InsertUser(UserDTO user)
        {
            string query = "INSERT INTO Users (FullName, Gender, Dob, Email, Address, Password) VALUES (@f, @g, @d, @e, @a, @p)";
            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@f", user.FullName),
                new NpgsqlParameter("@g", user.Gender),
                new NpgsqlParameter("@d", user.DateOfBirth),
                new NpgsqlParameter("@e", user.Email),
                new NpgsqlParameter("@a", user.Address ?? ""),
                new NpgsqlParameter("@p", user.PasswordHash)
            };
            return DbConnection.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
