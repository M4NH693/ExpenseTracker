using System;
using System.Drawing;
using System.Windows.Forms;
using quanlycitieu.Views;
using quanlycitieu.BLL;

namespace quanlycitieu
{
    public partial class Form1 : Form
    {
        public bool IsLogout = false;

        private Panel pnlSidebar;
        private Panel pnlContent;

        // User Profile Widget
        private Panel pnlUserProfile;
        private PictureBox pbUserAvatar;
        private Label lblUserName;
        private Label lblUserSubtitle;
        
        private Button btnTrangChu;
        private Button btnLich;
        private Button btnNhapVao;
        private Button btnThongKe;
        private Button btnDanhMuc;

        private TrangChuView trangChuView;
        private LichView lichView;
        private NhapVaoView nhapVaoView;
        private ThongKeView thongKeView;
        private CategoriesView categoriesView;

        public Form1()
        {
            this.Text = "Phần Mềm Quản Lý Chi Tiêu";
            this.Size = new Size(1200, 700);
            this.MinimumSize = new Size(1200, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable; 
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.ControlBox = true;

            InitializeContent();
            InitializeSidebar();
            InitializeViews();

            LoadUserProfileHeader();

            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (this.IsLogout) return; // Không hiển thị xác nhận khi người dùng chủ động Đăng xuất

            if (MessageBox.Show("Bạn có chắc chắn muốn thoát phần mềm không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true; // Cancel the closing
            }
        }

        private void InitializeSidebar()
        {
            pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 220;
            pnlSidebar.BackColor = Color.FromArgb(41, 40, 104);

            // 1. User Profile Button Widget (Replaces "Ví Cá Nhân")
            pnlUserProfile = new Panel()
            {
                Location = new Point(10, 14),
                Size = new Size(200, 58),
                BackColor = Color.FromArgb(52, 50, 118),
                Cursor = Cursors.Hand
            };

            pbUserAvatar = new PictureBox()
            {
                Size = new Size(38, 38),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };

            lblUserName = new Label()
            {
                Text = "Người dùng",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(54, 11),
                Size = new Size(140, 20),
                AutoEllipsis = true,
                Cursor = Cursors.Hand
            };

            lblUserSubtitle = new Label()
            {
                Text = "⚙ Hồ sơ cá nhân",
                ForeColor = Color.FromArgb(195, 195, 230),
                Font = new Font("Segoe UI", 8f, FontStyle.Regular),
                Location = new Point(54, 31),
                Size = new Size(140, 16),
                Cursor = Cursors.Hand
            };

            // Hover effect on User Profile Widget
            Action<Control> attachProfileEvents = null!;
            attachProfileEvents = (ctrl) =>
            {
                ctrl.MouseEnter += (s, e) => pnlUserProfile.BackColor = Color.FromArgb(70, 68, 155);
                ctrl.MouseLeave += (s, e) => {
                    // Check if mouse left the whole pnlUserProfile rectangle
                    Point p = pnlUserProfile.PointToClient(Cursor.Position);
                    if (!pnlUserProfile.ClientRectangle.Contains(p))
                    {
                        pnlUserProfile.BackColor = Color.FromArgb(52, 50, 118);
                    }
                };
                ctrl.Click += (s, e) => OpenUserProfile();
            };

            attachProfileEvents(pnlUserProfile);
            attachProfileEvents(pbUserAvatar);
            attachProfileEvents(lblUserName);
            attachProfileEvents(lblUserSubtitle);

            pnlUserProfile.Controls.Add(pbUserAvatar);
            pnlUserProfile.Controls.Add(lblUserName);
            pnlUserProfile.Controls.Add(lblUserSubtitle);
            pnlSidebar.Controls.Add(pnlUserProfile);

            // 2. Menu Navigation Buttons
            btnTrangChu = CreateMenuButton("TRANG CHỦ", "trangchu.png", 90);
            btnLich = CreateMenuButton("LỊCH", "lich.png", 150);
            btnNhapVao = CreateMenuButton("NHẬP VÀO", "nhapvao.png", 210);
            btnThongKe = CreateMenuButton("THỐNG KÊ", "thongke.png", 270);
            btnDanhMuc = CreateMenuButton("DANH MỤC", "danhmuc.png", 330);
            
            btnTrangChu.Click += (s, e) => { SetActiveBtn(btnTrangChu); ShowView(trangChuView); };
            btnLich.Click += (s, e) => { SetActiveBtn(btnLich); ShowView(lichView); };
            btnNhapVao.Click += (s, e) => { SetActiveBtn(btnNhapVao); ShowView(nhapVaoView); };
            btnThongKe.Click += (s, e) => { SetActiveBtn(btnThongKe); ShowView(thongKeView); };
            btnDanhMuc.Click += (s, e) => { SetActiveBtn(btnDanhMuc); ShowView(categoriesView); };

            pnlSidebar.Controls.Add(btnTrangChu);
            pnlSidebar.Controls.Add(btnLich);
            pnlSidebar.Controls.Add(btnNhapVao);
            pnlSidebar.Controls.Add(btnThongKe);
            pnlSidebar.Controls.Add(btnDanhMuc);
            
            this.Controls.Add(pnlSidebar);
        }

        public void LoadUserProfileHeader()
        {
            try
            {
                var userBLL = new UserBLL();
                var user = userBLL.GetUserProfile(QuanLyChiTieu.AuthForm.CurrentUserId);
                if (user != null && !string.IsNullOrWhiteSpace(user.FullName))
                {
                    lblUserName.Text = user.FullName;
                    pbUserAvatar.Image = UserProfileForm.CreateCircularAvatar(user.FullName, 38);
                }
                else
                {
                    lblUserName.Text = "Tài khoản";
                    pbUserAvatar.Image = UserProfileForm.CreateCircularAvatar("Tài khoản", 38);
                }
            }
            catch
            {
                lblUserName.Text = "Tài khoản";
                pbUserAvatar.Image = UserProfileForm.CreateCircularAvatar("Tài khoản", 38);
            }
        }

        private void OpenUserProfile()
        {
            using (var profileForm = new UserProfileForm(QuanLyChiTieu.AuthForm.CurrentUserId))
            {
                profileForm.ShowDialog(this);
                if (profileForm.IsLogout)
                {
                    this.IsLogout = true;
                    this.Close();
                    return;
                }

                // Cập nhật lại tên hiển thị và avatar trên header ngay lập tức nếu có thay đổi
                LoadUserProfileHeader();
            }
        }

        private void InitializeContent()
        {
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = Color.FromArgb(224, 224, 224);
            this.Controls.Add(pnlContent);
        }

        private Button CreateMenuButton(string text, string iconName, int top)
        {
            var btn = new Button();
            btn.Text = "   " + text;
            btn.Top = top;
            btn.Left = 0;
            btn.Width = 220;
            btn.Height = 50;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(20, 0, 0, 0);
            btn.Image = GetIcon(iconName);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private Image? GetIcon(string iconName)
        {
            try {
                string path = System.IO.Path.Combine(Application.StartupPath, "Resources", iconName);
                if (System.IO.File.Exists(path)) return Image.FromFile(path);
                path = System.IO.Path.Combine(Application.StartupPath, "..", "..", "..", "Resources", iconName);
                if (System.IO.File.Exists(path)) return Image.FromFile(path);
            } catch { }
            return null;
        }

        private void SetActiveBtn(Button activeBtn)
        {
            foreach (Control c in pnlSidebar.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = Color.White;
                }
            }
            activeBtn.BackColor = Color.FromArgb(255, 255, 128); // Yellow
            activeBtn.ForeColor = Color.Black;
        }

        private void InitializeViews()
        {
            trangChuView = new TrangChuView() { Dock = DockStyle.Fill };
            lichView = new LichView() { Dock = DockStyle.Fill };
            nhapVaoView = new NhapVaoView() { Dock = DockStyle.Fill };
            thongKeView = new ThongKeView() { Dock = DockStyle.Fill };
            categoriesView = new CategoriesView() { Dock = DockStyle.Fill };

            pnlContent.Controls.Add(trangChuView);
            pnlContent.Controls.Add(lichView);
            pnlContent.Controls.Add(nhapVaoView);
            pnlContent.Controls.Add(thongKeView);
            pnlContent.Controls.Add(categoriesView);

            // Default to Trang Chu
            btnTrangChu.PerformClick();
        }

        private void ShowView(UserControl view)
        {
            trangChuView?.Hide();
            lichView?.Hide();
            nhapVaoView?.Hide();
            thongKeView?.Hide();
            categoriesView?.Hide();

            if (view is TrangChuView tcv) tcv.LoadData();
            if (view is LichView lv) lv.LoadData();
            if (view is ThongKeView tkv) tkv.LoadData();
            if (view is NhapVaoView nvv) nvv.LoadData();
            if (view is CategoriesView cv) cv.LoadData();

            view.Show();
            view.BringToFront();
        }
    }
}
