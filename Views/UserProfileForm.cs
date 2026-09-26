using System;
using System.Drawing;
using System.Windows.Forms;
using quanlycitieu.BLL;
using quanlycitieu.DTO;

namespace quanlycitieu.Views
{
    public class UserProfileForm : Form
    {
        public bool IsLogout = false;

        private int userId;
        private UserBLL userBLL = new UserBLL();

        // Profile Controls
        private PictureBox pbAvatar;
        private Label lblHeaderName;
        private Label lblHeaderEmail;
        private TextBox txtEmail;
        private TextBox txtFullName;
        private ComboBox cbGender;
        private DateTimePicker dtpDob;
        private TextBox txtAddress;
        private Button btnSaveProfile;

        // Password Controls
        private TextBox txtOldPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnChangePassword;

        // Bottom Controls
        private Button btnLogout;
        private Button btnClose;

        public UserProfileForm(int userId)
        {
            this.userId = userId;

            this.Text = "Hồ Sơ Cá Nhân";
            this.Size = new Size(540, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            InitializeComponent();
            LoadUserData();
        }

        private void InitializeComponent()
        {
            // 1. Top Header Banner
            Panel pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(41, 40, 104)
            };

            pbAvatar = new PictureBox()
            {
                Size = new Size(54, 54),
                Location = new Point(20, 15),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            lblHeaderName = new Label()
            {
                Text = "Người dùng",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(85, 18),
                AutoSize = true
            };

            lblHeaderEmail = new Label()
            {
                Text = "user@example.com",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 230),
                Location = new Point(85, 45),
                AutoSize = true
            };

            pnlHeader.Controls.Add(pbAvatar);
            pnlHeader.Controls.Add(lblHeaderName);
            pnlHeader.Controls.Add(lblHeaderEmail);
            this.Controls.Add(pnlHeader);

            // 2. GroupBox: Basic Information
            GroupBox grpInfo = new GroupBox()
            {
                Text = " THÔNG TIN CƠ BẢN ",
                Location = new Point(20, 95),
                Size = new Size(485, 255),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 40, 104),
                BackColor = Color.White
            };

            int y = 28;
            int gap = 38;

            Label CreateLabel(string text, int top) => new Label()
            {
                Text = text,
                Location = new Point(15, top + 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            // Email (ReadOnly)
            grpInfo.Controls.Add(CreateLabel("Email đăng nhập:", y));
            txtEmail = new TextBox()
            {
                Location = new Point(140, y),
                Size = new Size(325, 25),
                Font = new Font("Segoe UI", 9),
                ReadOnly = true,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.FromArgb(90, 90, 90)
            };
            grpInfo.Controls.Add(txtEmail);

            // Full Name
            y += gap;
            grpInfo.Controls.Add(CreateLabel("Họ và tên (*):", y));
            txtFullName = new TextBox()
            {
                Location = new Point(140, y),
                Size = new Size(325, 25),
                Font = new Font("Segoe UI", 9)
            };
            grpInfo.Controls.Add(txtFullName);

            // Gender
            y += gap;
            grpInfo.Controls.Add(CreateLabel("Giới tính:", y));
            cbGender = new ComboBox()
            {
                Location = new Point(140, y),
                Size = new Size(130, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbGender.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
            grpInfo.Controls.Add(cbGender);

            // Date of birth
            y += gap;
            grpInfo.Controls.Add(CreateLabel("Ngày sinh (*):", y));
            dtpDob = new DateTimePicker()
            {
                Location = new Point(140, y),
                Size = new Size(130, 25),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                MaxDate = DateTime.Today
            };
            grpInfo.Controls.Add(dtpDob);

            // Address
            y += gap;
            grpInfo.Controls.Add(CreateLabel("Địa chỉ:", y));
            txtAddress = new TextBox()
            {
                Location = new Point(140, y),
                Size = new Size(325, 25),
                Font = new Font("Segoe UI", 9)
            };
            grpInfo.Controls.Add(txtAddress);

            // Save Profile Button
            y += gap + 2;
            btnSaveProfile = new Button()
            {
                Text = "💾 LƯU THÔNG TIN",
                Location = new Point(315, y),
                Size = new Size(150, 32),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(41, 40, 104),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSaveProfile.FlatAppearance.BorderSize = 0;
            btnSaveProfile.Click += BtnSaveProfile_Click;
            grpInfo.Controls.Add(btnSaveProfile);

            this.Controls.Add(grpInfo);

            // 3. GroupBox: Change Password
            GroupBox grpPassword = new GroupBox()
            {
                Text = " BẢO MẬT & ĐỔI MẬT KHẨU ",
                Location = new Point(20, 365),
                Size = new Size(485, 195),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 40, 104),
                BackColor = Color.White
            };

            int py = 28;
            int pgap = 36;

            grpPassword.Controls.Add(CreateLabel("Mật khẩu hiện tại (*):", py));
            txtOldPassword = new TextBox()
            {
                Location = new Point(160, py),
                Size = new Size(305, 25),
                Font = new Font("Segoe UI", 9),
                UseSystemPasswordChar = true
            };
            grpPassword.Controls.Add(txtOldPassword);

            py += pgap;
            grpPassword.Controls.Add(CreateLabel("Mật khẩu mới (*):", py));
            txtNewPassword = new TextBox()
            {
                Location = new Point(160, py),
                Size = new Size(305, 25),
                Font = new Font("Segoe UI", 9),
                UseSystemPasswordChar = true
            };
            grpPassword.Controls.Add(txtNewPassword);

            py += pgap;
            grpPassword.Controls.Add(CreateLabel("Xác nhận MK mới (*):", py));
            txtConfirmPassword = new TextBox()
            {
                Location = new Point(160, py),
                Size = new Size(305, 25),
                Font = new Font("Segoe UI", 9),
                UseSystemPasswordChar = true
            };
            grpPassword.Controls.Add(txtConfirmPassword);

            py += pgap + 2;
            btnChangePassword = new Button()
            {
                Text = "🔑 ĐỔI MẬT KHẨU",
                Location = new Point(315, py),
                Size = new Size(150, 32),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnChangePassword.FlatAppearance.BorderSize = 0;
            btnChangePassword.Click += BtnChangePassword_Click;
            grpPassword.Controls.Add(btnChangePassword);

            this.Controls.Add(grpPassword);

            // 4. Bottom Action Bar
            Panel pnlBottom = new Panel()
            {
                Location = new Point(20, 575),
                Size = new Size(485, 45),
                BackColor = Color.Transparent
            };

            btnLogout = new Button()
            {
                Text = "🚪 Đăng Xuất",
                Location = new Point(0, 5),
                Size = new Size(130, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(254, 242, 242),
                ForeColor = Color.FromArgb(220, 38, 38),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
            btnLogout.Click += BtnLogout_Click;

            btnClose = new Button()
            {
                Text = "Đóng",
                Location = new Point(355, 5),
                Size = new Size(130, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(230, 230, 235),
                ForeColor = Color.FromArgb(60, 60, 60),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            pnlBottom.Controls.Add(btnLogout);
            pnlBottom.Controls.Add(btnClose);
            this.Controls.Add(pnlBottom);
        }

        private void LoadUserData()
        {
            try
            {
                var user = userBLL.GetUserProfile(userId);
                if (user != null)
                {
                    txtEmail.Text = user.Email;
                    txtFullName.Text = user.FullName;
                    cbGender.SelectedItem = user.Gender;
                    if (cbGender.SelectedIndex == -1) cbGender.SelectedIndex = 0;

                    if (user.DateOfBirth > DateTime.MinValue && user.DateOfBirth <= DateTime.Today)
                    {
                        dtpDob.Value = user.DateOfBirth;
                    }
                    else
                    {
                        dtpDob.Value = new DateTime(2000, 1, 1);
                    }

                    txtAddress.Text = user.Address;

                    lblHeaderName.Text = user.FullName;
                    lblHeaderEmail.Text = user.Email;
                    pbAvatar.Image = CreateCircularAvatar(user.FullName, 54);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải thông tin người dùng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveProfile_Click(object? sender, EventArgs e)
        {
            try
            {
                string fullName = txtFullName.Text.Trim();
                string gender = cbGender.SelectedItem?.ToString() ?? "Nam";
                DateTime dob = dtpDob.Value.Date;
                string address = txtAddress.Text.Trim();

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    MessageBox.Show("Họ và tên không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtFullName.Focus();
                    return;
                }

                if (dob > DateTime.Today)
                {
                    MessageBox.Show("Ngày sinh không thể là ngày trong tương lai!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool success = userBLL.UpdateProfile(userId, fullName, gender, dob, address);
                if (success)
                {
                    lblHeaderName.Text = fullName;
                    pbAvatar.Image = CreateCircularAvatar(fullName, 54);
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không có thay đổi nào được lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi cập nhật", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnChangePassword_Click(object? sender, EventArgs e)
        {
            string oldPass = txtOldPassword.Text;
            string newPass = txtNewPassword.Text;
            string confirmPass = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(oldPass) || string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show("Vui lòng điền đầy đủ các ô mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPass.Length < 6)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Xác nhận mật khẩu mới không khớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                bool success = userBLL.ChangePassword(userId, oldPass, newPass, confirmPass);
                if (success)
                {
                    txtOldPassword.Clear();
                    txtNewPassword.Clear();
                    txtConfirmPassword.Clear();
                    MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất tài khoản?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.IsLogout = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public static Image CreateCircularAvatar(string name, int size)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Circle Background Gradient/Color
                using (var brush = new SolidBrush(Color.FromArgb(59, 130, 246)))
                {
                    g.FillEllipse(brush, 1, 1, size - 3, size - 3);
                }

                // Initial Letter
                string initial = "U";
                if (!string.IsNullOrWhiteSpace(name))
                {
                    string[] parts = name.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        initial = parts[parts.Length - 1].Substring(0, 1).ToUpper();
                    }
                }

                float fontSize = size * 0.42f;
                using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(initial, font, textBrush, new RectangleF(0, 0, size, size), sf);
                }

                // Outer border ring
                using (var pen = new Pen(Color.FromArgb(220, 255, 255, 255), 1.5f))
                {
                    g.DrawEllipse(pen, 1, 1, size - 3, size - 3);
                }
            }
            return bmp;
        }
    }
}
