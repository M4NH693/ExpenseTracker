using System;
using System.Drawing;
using System.Windows.Forms;

using System.Data;

namespace QuanLyChiTieu
{
    public class AuthForm : Form
    {
        public static int CurrentUserId = 1; // Default for testing if DB not used

        private Panel pnlLeft;
        private Panel pnlRight;

        // Login Controls
        private Panel pnlLogin;
        private TextBox txtLoginEmail;
        private TextBox txtLoginPassword;
        private Button btnLogin;
        private Button btnGoToRegister;
        private Button btnForgotPassword;

        // Register Controls
        private Panel pnlRegister;
        private TextBox txtRegName, txtRegGender, txtRegEmail, txtRegAddress, txtRegPassword, txtRegConfirmPassword;
        private DateTimePicker dtpRegDob;
        private Button btnRegister;
        private Button btnGoToLogin;

        public AuthForm()
        {
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Xác Thực - Phần Mềm Quản Lý Chi Tiêu";
            this.BackColor = Color.White;

            // Left panel
            pnlLeft = new Panel() { Dock = DockStyle.Left, Width = 350, BackColor = Color.FromArgb(41, 40, 104) };
            Label lblTitle = new Label() 
            { 
                Text = "QUẢN LÝ\nCHI TIÊU", 
                ForeColor = Color.White, 
                Font = new Font("Segoe UI", 28, FontStyle.Bold), 
                AutoSize = true, 
                Location = new Point(40, 200) 
            };
            Label lblSub = new Label() 
            { 
                Text = "Giải pháp tài chính thông minh", 
                ForeColor = Color.LightGray, 
                Font = new Font("Segoe UI", 12), 
                AutoSize = true, 
                Location = new Point(45, 320) 
            };
            pnlLeft.Controls.Add(lblTitle);
            pnlLeft.Controls.Add(lblSub);
            
            pnlRight = new Panel() { Dock = DockStyle.Fill };

            this.Controls.Add(pnlRight);
            this.Controls.Add(pnlLeft);

            SetupLoginPanel();
            SetupRegisterPanel();

            ShowLogin();
        }

        private void SetupLoginPanel()
        {
            pnlLogin = new Panel() { Dock = DockStyle.Fill };
            
            Label lblHeader = new Label() { Text = "ĐĂNG NHẬP", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(41, 40, 104), AutoSize = true, Location = new Point(140, 80) };
            
            txtLoginEmail = new TextBox() { Location = new Point(90, 180), Size = new Size(300, 35), Font = new Font("Segoe UI", 14), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = " Nhập Email" };
            txtLoginPassword = new TextBox() { Location = new Point(90, 240), Size = new Size(300, 35), Font = new Font("Segoe UI", 14), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = " Nhập Mật khẩu", UseSystemPasswordChar = true };
            
            btnLogin = new Button() { Text = "ĐĂNG NHẬP", Location = new Point(90, 310), Size = new Size(300, 45), Font = new Font("Segoe UI", 12, FontStyle.Bold), BackColor = Color.FromArgb(41, 40, 104), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnForgotPassword = new Button() 
            { 
                Text = "Quên mật khẩu?", 
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular), 
                Location = new Point(85, 385), 
                Size = new Size(130, 30), 
                ForeColor = Color.FromArgb(41, 40, 104), 
                FlatStyle = FlatStyle.Flat, 
                Cursor = Cursors.Hand, 
                BackColor = Color.Transparent 
            };
            btnForgotPassword.FlatAppearance.BorderSize = 0;
            btnForgotPassword.Click += (s, e) => {
                using (var frm = new quanlycitieu.Views.FrmForgotPassword())
                {
                    frm.ShowDialog(this);
                }
            };

            Label lblDivider = new Label() 
            { 
                Text = "•", 
                Font = new Font("Segoe UI", 12, FontStyle.Bold), 
                Location = new Point(222, 388), 
                AutoSize = true, 
                ForeColor = Color.FromArgb(180, 180, 200) 
            };

            btnGoToRegister = new Button() 
            { 
                Text = "Tạo tài khoản", 
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), 
                Location = new Point(245, 385), 
                Size = new Size(135, 30), 
                ForeColor = Color.FromArgb(41, 40, 104), 
                FlatStyle = FlatStyle.Flat, 
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            btnGoToRegister.FlatAppearance.BorderSize = 0;
            btnGoToRegister.Click += (s, e) => ShowRegister();

            pnlLogin.Controls.Add(lblHeader);
            pnlLogin.Controls.Add(txtLoginEmail);
            pnlLogin.Controls.Add(txtLoginPassword);
            pnlLogin.Controls.Add(btnLogin);
            pnlLogin.Controls.Add(btnForgotPassword);
            pnlLogin.Controls.Add(lblDivider);
            pnlLogin.Controls.Add(btnGoToRegister);

            pnlRight.Controls.Add(pnlLogin);
        }

        private void SetupRegisterPanel()
        {
            pnlRegister = new Panel() { Dock = DockStyle.Fill, Visible = false };
            
            Label lblHeader = new Label() { Text = "TẠO TÀI KHOẢN", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.FromArgb(41, 40, 104), AutoSize = true, Location = new Point(130, 30) };
            pnlRegister.Controls.Add(lblHeader);

            int startY = 90;
            int gap = 45;

            TextBox AddTextBox(string placeholder, int y, bool isPassword = false) { 
                var t = new TextBox() { Location = new Point(90, y), Size = new Size(300, 30), Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = " " + placeholder, UseSystemPasswordChar = isPassword }; 
                pnlRegister.Controls.Add(t); 
                return t; 
            }

            txtRegName = AddTextBox("Họ và Tên", startY);
            txtRegGender = AddTextBox("Giới Tính (Nam/Nữ)", startY += gap);
            
            startY += gap;
            Label lblDob = new Label() { Text = "Ngày sinh:", Font = new Font("Segoe UI", 9), Location = new Point(90, startY), AutoSize = true, ForeColor = Color.Gray };
            pnlRegister.Controls.Add(lblDob);
            dtpRegDob = new DateTimePicker() { Location = new Point(90, startY + 20), Size = new Size(300, 30), Font = new Font("Segoe UI", 12), Format = DateTimePickerFormat.Short, MaxDate = DateTime.Today, Value = new DateTime(2000, 1, 1) };
            pnlRegister.Controls.Add(dtpRegDob);

            startY += 20; // Extra offset for the label
            txtRegEmail = AddTextBox("Email", startY += gap);
            txtRegAddress = AddTextBox("Địa Chỉ", startY += gap);
            txtRegPassword = AddTextBox("Mật khẩu", startY += gap, true);
            txtRegConfirmPassword = AddTextBox("Xác nhận Mật khẩu", startY += gap, true);

            btnRegister = new Button() { Text = "ĐĂNG KÝ", Location = new Point(90, startY += 55), Size = new Size(300, 40), BackColor = Color.FromArgb(41, 40, 104), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;
            pnlRegister.Controls.Add(btnRegister);

            btnGoToLogin = new Button() { Text = "← Quay lại Đăng nhập", Font = new Font("Segoe UI", 10), Location = new Point(10, 10), Size = new Size(160, 30), FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray, Cursor = Cursors.Hand, BackColor = Color.White };
            btnGoToLogin.FlatAppearance.BorderSize = 0;
            btnGoToLogin.Click += (s, e) => ShowLogin();
            pnlRegister.Controls.Add(btnGoToLogin);

            pnlRight.Controls.Add(pnlRegister);
        }

        private void ShowLogin() { pnlRegister.Visible = false; pnlLogin.Visible = true; }
        private void ShowRegister() { pnlLogin.Visible = false; pnlRegister.Visible = true; }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var bll = new quanlycitieu.BLL.UserBLL();
            if (bll.Login(txtLoginEmail.Text, txtLoginPassword.Text, out int userId))
            {
                CurrentUserId = userId;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Sai email hoặc mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtRegName.Text))
            {
                MessageBox.Show("Vui lòng nhập Họ và Tên!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtRegEmail.Text) || string.IsNullOrWhiteSpace(txtRegPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập Email và Mật khẩu!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtRegPassword.Text != txtRegConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Normalize Gender to match DB CHECK constraint
            string genderRaw = txtRegGender.Text.Trim();
            string gender;
            if (genderRaw.Equals("nam", StringComparison.OrdinalIgnoreCase)) gender = "Nam";
            else if (genderRaw.Equals("nữ", StringComparison.OrdinalIgnoreCase) || genderRaw.Equals("nu", StringComparison.OrdinalIgnoreCase)) gender = "Nữ";
            else if (genderRaw.Equals("khác", StringComparison.OrdinalIgnoreCase) || genderRaw.Equals("khac", StringComparison.OrdinalIgnoreCase)) gender = "Khác";
            else
            {
                MessageBox.Show("Giới tính phải là: Nam, Nữ hoặc Khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new quanlycitieu.DTO.UserDTO
            {
                Email = txtRegEmail.Text.Trim(),
                FullName = txtRegName.Text.Trim(),
                Gender = gender,
                DateOfBirth = dtpRegDob.Value.Date, // Chỉ gửi Date, không kèm Time
                Address = txtRegAddress.Text.Trim()
            };

            var bll = new quanlycitieu.BLL.UserBLL();
            try
            {
                if (bll.Register(dto, txtRegPassword.Text))
                {
                    MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowLogin();
                }
                else
                {
                    MessageBox.Show("Đăng ký thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
