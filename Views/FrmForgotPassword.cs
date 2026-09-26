using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using quanlycitieu.BLL;

namespace quanlycitieu.Views
{
    public class FrmForgotPassword : Form
    {
        private UserBLL userBLL = new UserBLL();

        private TextBox txtEmail;
        private Button btnSendOtp;
        private Label lblEmailStatus;

        private Panel pnlOtpSection;
        private TextBox txtOtp;
        private Button btnVerifyOtp;
        private Label lblOtpHint;

        private Button btnBackToLogin;

        public FrmForgotPassword()
        {
            this.Text = "Quên Mật Khẩu - Xác Minh OTP";
            this.Size = new Size(480, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.BackColor = Color.White;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Header
            Panel pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(41, 40, 104)
            };

            btnBackToLogin = new Button()
            {
                Text = "← Đăng nhập",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(15, 10),
                Size = new Size(100, 26),
                ForeColor = Color.FromArgb(200, 200, 230),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            btnBackToLogin.FlatAppearance.BorderSize = 0;
            btnBackToLogin.Click += (s, e) => this.Close();

            Label lblTitle = new Label()
            {
                Text = "QUÊN MẬT KHẨU",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 40),
                AutoSize = true
            };

            pnlHeader.Controls.Add(btnBackToLogin);
            pnlHeader.Controls.Add(lblTitle);
            this.Controls.Add(pnlHeader);

            // Step 1: Email Input
            int startY = 110;

            Label lblEmail = new Label()
            {
                Text = "Nhập Email tài khoản của bạn:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(35, startY),
                AutoSize = true
            };
            this.Controls.Add(lblEmail);

            txtEmail = new TextBox()
            {
                Location = new Point(35, startY + 28),
                Size = new Size(395, 32),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "example@gmail.com"
            };
            this.Controls.Add(txtEmail);

            btnSendOtp = new Button()
            {
                Text = "GỬI MÃ OTP",
                Location = new Point(35, startY + 70),
                Size = new Size(395, 40),
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(41, 40, 104),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSendOtp.FlatAppearance.BorderSize = 0;
            btnSendOtp.Click += BtnSendOtp_Click;
            this.Controls.Add(btnSendOtp);

            lblEmailStatus = new Label()
            {
                Text = "",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(70, 70, 70),
                Location = new Point(35, startY + 115),
                Size = new Size(395, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblEmailStatus);

            // Step 2: OTP Section (Hidden or disabled initially)
            pnlOtpSection = new Panel()
            {
                Location = new Point(35, startY + 140),
                Size = new Size(395, 200),
                BackColor = Color.Transparent,
                Visible = false
            };

            Label lblDivider = new Label()
            {
                Text = "─────────────────────────────────────────",
                ForeColor = Color.FromArgb(220, 220, 220),
                Location = new Point(0, 0),
                AutoSize = true
            };
            pnlOtpSection.Controls.Add(lblDivider);

            Label lblOtpTitle = new Label()
            {
                Text = "Nhập mã OTP xác minh (6 số):",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 40, 104),
                Location = new Point(0, 24),
                AutoSize = true
            };
            pnlOtpSection.Controls.Add(lblOtpTitle);

            txtOtp = new TextBox()
            {
                Location = new Point(0, 52),
                Size = new Size(395, 38),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                MaxLength = 6,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "••••••"
            };
            pnlOtpSection.Controls.Add(txtOtp);

            lblOtpHint = new Label()
            {
                Text = "⏱ Mã OTP có hiệu lực trong vòng 5 phút",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(220, 38, 38),
                Location = new Point(0, 95),
                Size = new Size(395, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlOtpSection.Controls.Add(lblOtpHint);

            btnVerifyOtp = new Button()
            {
                Text = "XÁC MINH OTP",
                Location = new Point(0, 120),
                Size = new Size(395, 42),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 125, 50), // Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVerifyOtp.FlatAppearance.BorderSize = 0;
            btnVerifyOtp.Click += BtnVerifyOtp_Click;
            pnlOtpSection.Controls.Add(btnVerifyOtp);

            this.Controls.Add(pnlOtpSection);
        }

        private async void BtnSendOtp_Click(object? sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ Email!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Giao diện: Khóa nút để tránh bấm nhiều lần, hiển thị trạng thái đang gửi
            btnSendOtp.Enabled = false;
            txtEmail.Enabled = false;
            btnSendOtp.Text = "⏳ Đang gửi mã OTP...";
            lblEmailStatus.Text = "Hệ thống đang kết nối SMTP để gửi email... Vui lòng đợi trong giây lát.";
            lblEmailStatus.ForeColor = Color.FromArgb(59, 130, 246);

            try
            {
                // Xử lý gửi OTP bất đồng bộ (async/await) không làm đơ giao diện
                await userBLL.GenerateAndSendOtpAsync(email);

                lblEmailStatus.Text = "✔ Mã OTP đã được gửi thành công!";
                lblEmailStatus.ForeColor = Color.FromArgb(46, 125, 50);

                // Mở phần nhập OTP
                pnlOtpSection.Visible = true;
                txtOtp.Focus();

                MessageBox.Show(
                    "Mã OTP gồm 6 chữ số đã được gửi đến email của bạn!\n" +
                    "Vui lòng kiểm tra hộp thư đến (hoặc mục Thư rác/Spam).",
                    "Đã gửi OTP",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblEmailStatus.Text = "Gửi OTP thất bại.";
                lblEmailStatus.ForeColor = Color.FromArgb(220, 38, 38);

                // Hiển thị thông báo lỗi chi tiết
                MessageBox.Show(ex.Message, "Thông báo gửi OTP", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Nếu có mã test trong thông báo (khi chưa cấu hình SMTP), vẫn mở form OTP để người dùng test được
                if (ex.Message.Contains("Mã OTP của bạn là:"))
                {
                    pnlOtpSection.Visible = true;
                    txtOtp.Focus();
                }
            }
            finally
            {
                btnSendOtp.Enabled = true;
                txtEmail.Enabled = true;
                btnSendOtp.Text = "GỬI LẠI MÃ OTP";
            }
        }

        private void BtnVerifyOtp_Click(object? sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string otp = txtOtp.Text.Trim();

            if (string.IsNullOrWhiteSpace(otp))
            {
                MessageBox.Show("Vui lòng nhập mã OTP 6 chữ số!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOtp.Focus();
                return;
            }

            try
            {
                bool isValid = userBLL.VerifyOtp(email, otp);
                if (isValid)
                {
                    // OTP chính xác -> Mở form đặt lại mật khẩu mới
                    using (var frmReset = new FrmResetPassword(email))
                    {
                        if (frmReset.ShowDialog(this) == DialogResult.OK)
                        {
                            // Đổi mật khẩu thành công -> Đóng form quên mật khẩu để về màn hình Login
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Xác minh thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtOtp.SelectAll();
                txtOtp.Focus();
            }
        }
    }
}
