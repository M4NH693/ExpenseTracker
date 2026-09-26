using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace quanlycitieu.Services
{
    public static class EmailService
    {
        // =========================================================================
        // CẤU HÌNH GỬI MAIL BẰNG GMAIL SMTP
        // =========================================================================
        // Hướng dẫn:
        // 1. Dùng tài khoản Gmail của bạn.
        // 2. Bật xác thực 2 bước (2-Step Verification) trên tài khoản Google.
        // 3. Vào https://myaccount.google.com/apppasswords tạo "Mật khẩu ứng dụng" (App Password 16 ký tự).
        // 4. Điền Email và Mật khẩu ứng dụng vào 2 dòng dưới:
        // =========================================================================
        public static string SmtpHost = "smtp.gmail.com";
        public static int SmtpPort = 587;
        public static bool EnableSsl = true;

        public static string SenderEmail = "vmanhsaber119@gmail.com"; // Thay bằng Gmail của bạn
        public static string SenderAppPassword = "ujqg upft cgmm mixl"; // Thay bằng Mật khẩu ứng dụng 16 ký tự của Google

        public static string SenderDisplayName = "Phần Mềm Quản Lý Chi Tiêu";

        /// <summary>
        /// Gửi mã xác minh OTP qua Gmail SMTP bất đồng bộ (async/await)
        /// </summary>
        public static async Task SendOtpEmailAsync(string recipientEmail, string otpCode)
        {
            // Kiểm tra xem người dùng đã cấu hình App Password hay chưa
            if (string.IsNullOrWhiteSpace(SenderEmail) || string.IsNullOrWhiteSpace(SenderAppPassword))
            {
                // Nếu chưa cấu hình mật khẩu ứng dụng Gmail thực tế, ném thông báo chi tiết
                // kèm mã OTP để người dùng có thể test luồng logic ngay lập tức mà không bị nghẽn
                throw new InvalidOperationException(
                    "CHƯA CẤU HÌNH GMAIL SMTP:\n" +
                    "Vui lòng mở file 'Services/EmailService.cs' và điền Gmail + Mật khẩu ứng dụng (App Password 16 ký tự).\n\n" +
                    $"👉 [CHẾ ĐỘ KIỂM THỬ] Mã OTP của bạn là: {otpCode} (Hiệu lực 5 phút)");
            }

            using (var client = new SmtpClient(SmtpHost, SmtpPort))
            {
                client.EnableSsl = EnableSsl;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(SenderEmail.Trim(), SenderAppPassword.Trim());
                client.Timeout = 15000; // 15 giây

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(SenderEmail.Trim(), SenderDisplayName);
                    message.To.Add(new MailAddress(recipientEmail.Trim()));
                    message.Subject = $"[{otpCode}] Mã xác nhận đặt lại mật khẩu - Quản Lý Chi Tiêu";
                    message.IsBodyHtml = true;
                    message.Body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 520px; margin: 0 auto; padding: 25px; border: 1px solid #e0e0e0; border-radius: 10px; background-color: #ffffff;'>
                            <div style='text-align: center; margin-bottom: 20px;'>
                                <h2 style='color: #292868; margin: 0; font-size: 24px;'>QUẢN LÝ CHI TIÊU</h2>
                                <p style='color: #666; font-size: 13px; margin-top: 5px;'>Giải pháp tài chính thông minh</p>
                            </div>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 15px 0;'/>
                            <p style='color: #333; font-size: 15px;'>Xin chào,</p>
                            <p style='color: #555; line-height: 1.5;'>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản liên kết với địa chỉ email này. Dưới đây là mã xác thực OTP của bạn:</p>
                            <div style='background-color: #f0f4ff; border: 2px dashed #292868; border-radius: 8px; padding: 18px; text-align: center; margin: 25px 0;'>
                                <span style='font-size: 34px; font-weight: bold; letter-spacing: 8px; color: #292868;'>{otpCode}</span>
                            </div>
                            <p style='color: #d9534f; font-weight: bold; font-size: 14px;'>⚠️ Mã xác minh này có hiệu lực trong vòng 5 phút.</p>
                            <p style='color: #777; font-size: 13px;'>Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này. Tài khoản của bạn vẫn được an toàn.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'/>
                            <p style='font-size: 12px; color: #999; text-align: center;'>Đây là email tự động gửi từ hệ thống Quản Lý Chi Tiêu, vui lòng không phản hồi thư này.</p>
                        </div>";

                    await client.SendMailAsync(message);
                }
            }
        }
    }
}
