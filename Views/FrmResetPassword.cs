using System;
using System.Drawing;
using System.Windows.Forms;
using quanlycitieu.BLL;

namespace quanlycitieu.Views
{
    public class FrmResetPassword : Form
    {
        private string email;
        private UserBLL userBLL = new UserBLL();

        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnSave;
        private Button btnCancel;

        public FrmResetPassword(string email)
        {
            this.email = email;

            this.Text = "Đặt Lại Mật Khẩu Mới";
            this.Size = new Size(460, 440);
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
                Height = 80,
                BackColor = Color.FromArgb(41, 40, 104)
            };

            Label lblTitle = new Label()
            {
                Text = "ĐẶT LẠI MẬT KHẨU",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 18),
                AutoSize = true
            };

            Label lblSub = new Label()
            {
                Text = $"Tài khoản: {email}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 235),
                Location = new Point(26, 48),
                AutoSize = true
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSub);
            this.Controls.Add(pnlHeader);

            int startY = 110;
            int gap = 60;

            Label CreateLabel(string text, int y) => new Label()
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(35, y),
                AutoSize = true
            };

            TextBox CreatePasswordBox(int y, string placeholder) => new TextBox()
            {
                Location = new Point(35, y + 24),
                Size = new Size(375, 30),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true,
                PlaceholderText = placeholder
            };

            // New password
            this.Controls.Add(CreateLabel("Mật khẩu mới (*):", startY));
            txtNewPassword = CreatePasswordBox(startY, "Tối thiểu 6 ký tự");
            this.Controls.Add(txtNewPassword);

            // Confirm new password
            startY += gap + 10;
            this.Controls.Add(CreateLabel("Xác nhận mật khẩu mới (*):", startY));
            txtConfirmPassword = CreatePasswordBox(startY, "Nhập lại mật khẩu mới");
            this.Controls.Add(txtConfirmPassword);

            // Action Buttons
            startY += gap + 30;

            btnSave = new Button()
            {
                Text = "LƯU THAY ĐỔI",
                Location = new Point(35, startY),
                Size = new Size(375, 42),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(41, 40, 104),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button()
            {
                Text = "Hủy bỏ",
                Location = new Point(160, startY + 50),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            string newPass = txtNewPassword.Text;
            string confirmPass = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(newPass))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPass.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có tối thiểu 6 ký tự!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                bool success = userBLL.ResetPassword(email, newPass, confirmPass);
                if (success)
                {
                    MessageBox.Show(
                        "Đặt lại mật khẩu thành công!\nBạn có thể đăng nhập bằng mật khẩu mới ngay bây giờ.",
                        "Thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi đặt lại mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
