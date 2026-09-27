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
        private Button btnNganSach;
        private Button btnSoNo;
        private Button? currentActiveBtn;

        private TrangChuView trangChuView;
        private LichView lichView;
        private NhapVaoView nhapVaoView;
        private ThongKeView thongKeView;
        private CategoriesView categoriesView;
        private BudgetView budgetView;
        private DebtView debtView;

        public Form1()
        {
            this.Text = "Phần Mềm Quản Lý Chi Tiêu";
            this.Size = new Size(1250, 720);
            this.MinimumSize = new Size(1200, 680);
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
                Location = new Point(10, 12),
                Size = new Size(200, 56),
                BackColor = Color.FromArgb(52, 50, 118),
                Cursor = Cursors.Hand
            };

            pbUserAvatar = new PictureBox()
            {
                Size = new Size(36, 36),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };

            lblUserName = new Label()
            {
                Text = "Người dùng",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(52, 10),
                Size = new Size(142, 18),
                AutoEllipsis = true,
                Cursor = Cursors.Hand
            };

            lblUserSubtitle = new Label()
            {
                Text = "⚙ Hồ sơ cá nhân",
                ForeColor = Color.FromArgb(195, 195, 230),
                Font = new Font("Segoe UI", 8f, FontStyle.Regular),
                Location = new Point(52, 28),
                Size = new Size(142, 16),
                Cursor = Cursors.Hand
            };

            // Hover effect on User Profile Widget
            Action<Control> attachProfileEvents = null!;
            attachProfileEvents = (ctrl) =>
            {
                ctrl.MouseEnter += (s, e) => pnlUserProfile.BackColor = Color.FromArgb(70, 68, 155);
                ctrl.MouseLeave += (s, e) => {
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
            int startY = 84;
            int btnHeight = 50;
            int gap = 60;

            btnTrangChu = CreateMenuButton("TRANG CHỦ", "trangchu.png", startY, btnHeight);
            btnLich = CreateMenuButton("LỊCH", "lich.png", startY + gap, btnHeight);
            btnNhapVao = CreateMenuButton("NHẬP VÀO", "nhapvao.png", startY + gap * 2, btnHeight);
            btnThongKe = CreateMenuButton("THỐNG KÊ", "thongke.png", startY + gap * 3, btnHeight);
            btnDanhMuc = CreateMenuButton("DANH MỤC", "danhmuc.png", startY + gap * 4, btnHeight);
            btnNganSach = CreateMenuButton("NGÂN SÁCH", "wallet.png", startY + gap * 5, btnHeight);
            btnSoNo = CreateMenuButton("SỔ VAY NỢ", "sono.png", startY + gap * 6, btnHeight);
            
            btnTrangChu.Click += (s, e) => { SetActiveBtn(btnTrangChu); ShowView(trangChuView); };
            btnLich.Click += (s, e) => { SetActiveBtn(btnLich); ShowView(lichView); };
            btnNhapVao.Click += (s, e) => { SetActiveBtn(btnNhapVao); ShowView(nhapVaoView); };
            btnThongKe.Click += (s, e) => { SetActiveBtn(btnThongKe); ShowView(thongKeView); };
            btnDanhMuc.Click += (s, e) => { SetActiveBtn(btnDanhMuc); ShowView(categoriesView); };
            btnNganSach.Click += (s, e) => { SetActiveBtn(btnNganSach); ShowView(budgetView); };
            btnSoNo.Click += (s, e) => { SetActiveBtn(btnSoNo); ShowView(debtView); };

            pnlSidebar.Controls.Add(btnTrangChu);
            pnlSidebar.Controls.Add(btnLich);
            pnlSidebar.Controls.Add(btnNhapVao);
            pnlSidebar.Controls.Add(btnThongKe);
            pnlSidebar.Controls.Add(btnDanhMuc);
            pnlSidebar.Controls.Add(btnNganSach);
            pnlSidebar.Controls.Add(btnSoNo);
            
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
                    pbUserAvatar.Image = UserProfileForm.CreateCircularAvatar(user.FullName, 36);
                }
                else
                {
                    lblUserName.Text = "Tài khoản";
                    pbUserAvatar.Image = UserProfileForm.CreateCircularAvatar("Tài khoản", 36);
                }
            }
            catch
            {
                lblUserName.Text = "Tài khoản";
                pbUserAvatar.Image = UserProfileForm.CreateCircularAvatar("Tài khoản", 36);
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

        private Button CreateMenuButton(string text, string iconName, int top, int height = 50)
        {
            var btn = new Button();
            btn.Text = "   " + text;
            btn.Top = top;
            btn.Left = 0;
            btn.Width = 220;
            btn.Height = height;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(18, 0, 0, 0);
            btn.Image = GetIcon(iconName, 28, 28);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => {
                if (btn != currentActiveBtn)
                    btn.BackColor = Color.FromArgb(55, 53, 125);
            };
            btn.MouseLeave += (s, e) => {
                if (btn != currentActiveBtn)
                    btn.BackColor = Color.Transparent;
            };

            return btn;
        }

        private Image? GetIcon(string iconName, int targetWidth = 28, int targetHeight = 28)
        {
            try {
                string path = System.IO.Path.Combine(Application.StartupPath, "Resources", iconName);
                if (!System.IO.File.Exists(path))
                    path = System.IO.Path.Combine(Application.StartupPath, "..", "..", "..", "Resources", iconName);

                if (System.IO.File.Exists(path))
                {
                    using (var rawImg = Image.FromFile(path))
                    {
                        var bmp = new Bitmap(targetWidth, targetHeight);
                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.DrawImage(rawImg, 0, 0, targetWidth, targetHeight);
                        }
                        return bmp;
                    }
                }
            } catch { }
            return null;
        }

        private void SetActiveBtn(Button activeBtn)
        {
            currentActiveBtn = activeBtn;
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
            budgetView = new BudgetView() { Dock = DockStyle.Fill };
            debtView = new DebtView() { Dock = DockStyle.Fill };

            pnlContent.Controls.Add(trangChuView);
            pnlContent.Controls.Add(lichView);
            pnlContent.Controls.Add(nhapVaoView);
            pnlContent.Controls.Add(thongKeView);
            pnlContent.Controls.Add(categoriesView);
            pnlContent.Controls.Add(budgetView);
            pnlContent.Controls.Add(debtView);

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
            budgetView?.Hide();
            debtView?.Hide();

            if (view is TrangChuView tcv) tcv.LoadData();
            if (view is LichView lv) lv.LoadData();
            if (view is ThongKeView tkv) tkv.LoadData();
            if (view is NhapVaoView nvv) nvv.LoadData();
            if (view is CategoriesView cv) cv.LoadData();
            if (view is BudgetView bv) bv.LoadData();
            if (view is DebtView dv) dv.LoadData();

            view.Show();
            view.BringToFront();
        }
    }
}
