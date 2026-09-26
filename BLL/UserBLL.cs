using System;
using System.Collections.Concurrent;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using quanlycitieu.DAL;
using quanlycitieu.DTO;
using quanlycitieu.Services;

namespace quanlycitieu.BLL
{
    public class UserBLL
    {
        private UserDAL userDAL = new UserDAL();

        // Lưu trữ mã OTP tạm thời trong bộ nhớ kèm thời gian hết hạn (5 phút)
        private class OtpEntry
        {
            public string Code { get; set; } = "";
            public DateTime ExpiryTime { get; set; }
        }
        private static readonly ConcurrentDictionary<string, OtpEntry> otpCache = new();

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

        public UserDTO? GetUserProfile(int userId)
        {
            DataTable dt = userDAL.GetUserById(userId);
            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];
            return new UserDTO
            {
                Id = Convert.ToInt32(row["Id"]),
                FullName = row["FullName"]?.ToString() ?? "",
                Gender = row["Gender"]?.ToString() ?? "Nam",
                DateOfBirth = ParseDate(row["Dob"]),
                Email = row["Email"]?.ToString() ?? "",
                Address = row["Address"]?.ToString() ?? ""
            };
        }

        private static DateTime ParseDate(object? value)
        {
            if (value == null || value == DBNull.Value) return new DateTime(2000, 1, 1);
            if (value is DateTime dt) return dt;
            if (value is DateOnly d) return d.ToDateTime(TimeOnly.MinValue);
            if (DateTime.TryParse(value.ToString(), out DateTime parsed)) return parsed;
            return new DateTime(2000, 1, 1);
        }

        public bool UpdateProfile(int userId, string fullName, string gender, DateTime dob, string address)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new Exception("Họ và tên không được để trống!");

            // Normalize gender to Nam, Nữ, Khác
            string g = gender?.Trim() ?? "Nam";
            if (g.Equals("nam", StringComparison.OrdinalIgnoreCase)) g = "Nam";
            else if (g.Equals("nữ", StringComparison.OrdinalIgnoreCase) || g.Equals("nu", StringComparison.OrdinalIgnoreCase)) g = "Nữ";
            else if (g.Equals("khác", StringComparison.OrdinalIgnoreCase) || g.Equals("khac", StringComparison.OrdinalIgnoreCase)) g = "Khác";
            else g = "Nam";

            if (dob.Date > DateTime.Today)
                throw new Exception("Ngày sinh không thể là ngày trong tương lai!");

            return userDAL.UpdateUserProfile(userId, fullName.Trim(), g, dob.Date, address?.Trim() ?? "");
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword))
                throw new Exception("Vui lòng nhập mật khẩu hiện tại!");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new Exception("Vui lòng nhập mật khẩu mới!");

            if (newPassword.Length < 6)
                throw new Exception("Mật khẩu mới phải có ít nhất 6 ký tự!");

            if (newPassword != confirmPassword)
                throw new Exception("Xác nhận mật khẩu mới không khớp!");

            DataTable dt = userDAL.GetUserById(userId);
            if (dt.Rows.Count == 0)
                throw new Exception("Không tìm thấy thông tin tài khoản!");

            string storedHash = dt.Rows[0]["Password"]?.ToString()?.Trim() ?? "";
            if (!VerifyPassword(oldPassword, storedHash))
                throw new Exception("Mật khẩu hiện tại không chính xác!");

            string newHash = HashPassword(newPassword);
            return userDAL.UpdatePassword(userId, newHash);
        }

        // =========================================================================
        // NGHIỆP VỤ QUÊN MẬT KHẨU & XÁC MINH OTP
        // =========================================================================

        /// <summary>
        /// Sinh mã OTP 6 số ngẫu nhiên, lưu vào bộ nhớ tạm (5 phút) và gửi email qua SMTP
        /// </summary>
        public async Task<string> GenerateAndSendOtpAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Vui lòng nhập địa chỉ Email!");

            string normalizedEmail = email.Trim().ToLower();

            // Kiểm tra email có tồn tại trong hệ thống không
            if (!userDAL.CheckEmailExists(normalizedEmail))
            {
                throw new Exception("Email này chưa được đăng ký trong hệ thống!");
            }

            // Sinh ngẫu nhiên mã OTP 6 chữ số
            string otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            // Lưu tạm mã OTP kèm thời hạn 5 phút
            otpCache[normalizedEmail] = new OtpEntry
            {
                Code = otpCode,
                ExpiryTime = DateTime.Now.AddMinutes(5)
            };

            // Gửi mail bất đồng bộ (async/await)
            await EmailService.SendOtpEmailAsync(normalizedEmail, otpCode);

            return otpCode;
        }

        /// <summary>
        /// Xác minh mã OTP người dùng nhập vào
        /// </summary>
        public bool VerifyOtp(string email, string otpInput)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email không hợp lệ!");

            if (string.IsNullOrWhiteSpace(otpInput))
                throw new Exception("Vui lòng nhập mã OTP!");

            string normalizedEmail = email.Trim().ToLower();

            if (!otpCache.TryGetValue(normalizedEmail, out var entry))
            {
                throw new Exception("Không tìm thấy yêu cầu OTP hoặc mã đã bị xóa. Vui lòng bấm 'Gửi mã OTP' lại!");
            }

            if (DateTime.Now > entry.ExpiryTime)
            {
                otpCache.TryRemove(normalizedEmail, out _);
                throw new Exception("Mã OTP đã hết hạn (quá 5 phút). Vui lòng nhấn gửi lại mã mới!");
            }

            if (entry.Code != otpInput.Trim())
            {
                throw new Exception("Mã OTP không chính xác. Vui lòng kiểm tra lại hộp thư!");
            }

            return true;
        }

        /// <summary>
        /// Đặt lại mật khẩu mới cho tài khoản sau khi đã xác minh OTP
        /// </summary>
        public bool ResetPassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email không hợp lệ!");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new Exception("Vui lòng nhập mật khẩu mới!");

            if (newPassword.Length < 6)
                throw new Exception("Mật khẩu mới phải có ít nhất 6 ký tự!");

            if (newPassword != confirmPassword)
                throw new Exception("Xác nhận mật khẩu mới không khớp!");

            string normalizedEmail = email.Trim().ToLower();

            if (!userDAL.CheckEmailExists(normalizedEmail))
            {
                throw new Exception("Tài khoản không tồn tại!");
            }

            string newHash = HashPassword(newPassword);
            bool success = userDAL.UpdatePasswordByEmail(normalizedEmail, newHash);

            if (success)
            {
                // Xóa OTP sau khi đổi mật khẩu thành công
                otpCache.TryRemove(normalizedEmail, out _);
            }

            return success;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
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
