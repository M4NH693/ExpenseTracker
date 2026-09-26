using System.Data;
using quanlycitieu.DAL;
using quanlycitieu.DTO;
using System;

namespace quanlycitieu.BLL
{
    public class UserBLL
    {
        private UserDAL userDAL = new UserDAL();

        public bool Login(string email, string password, out int userId)
        {
            userId = -1;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            email = email.Trim().ToLower();
            DataTable dt = userDAL.GetUserByEmail(email);
            if (dt.Rows.Count > 0)
            {
                string storedHash = dt.Rows[0]["Password"]?.ToString()?.Trim() ?? "";
                if (VerifyPassword(password, storedHash))
                {
                    userId = Convert.ToInt32(dt.Rows[0]["Id"]);
                    return true;
                }
            }
            return false;
        }

        public bool Register(UserDTO user, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(plainPassword))
                return false;
            
            user.Email = user.Email.Trim().ToLower();
            
            DataTable dt = userDAL.GetUserByEmail(user.Email);
            if (dt.Rows.Count > 0)
                throw new Exception("Email đã tồn tại!");

            user.PasswordHash = HashPassword(plainPassword);
            return userDAL.InsertUser(user);
        }

        // Simulating hash for now, you can implement real hashing later
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string plain, string hash)
        {
            hash = hash?.Trim() ?? "";
            
            // For backward compatibility with plain text passwords during dev:
            if (plain == hash) return true; 
            
            string computedHash = HashPassword(plain);
            if (computedHash == hash) return true;
            
            // Allow truncated hash if the database column length cut off the Base64 string
            if (hash.Length >= 20 && computedHash.StartsWith(hash)) return true;
            
            return false;
        }
    }
}
