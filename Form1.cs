using System;
using System.Drawing;
using System.Windows.Forms;
using quanlycitieu.Views;

namespace quanlycitieu
{
    public partial class Form1 : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        
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

            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
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

            // Wallet icon / Title
            PictureBox pbWallet = new PictureBox() { Image = GetIcon("wallet.png"), SizeMode = PictureBoxSizeMode.Zoom, Size = new Size(32, 32), Location = new Point(20, 20) };
            Label lblTitle = new Label() { Text = "Ví Cá Nhân", ForeColor = Color.White, Font = new Font("Segoe UI", 10), Location = new Point(60, 28), AutoSize = true };
            
            pnlSidebar.Controls.Add(pbWallet);
            pnlSidebar.Controls.Add(lblTitle);

            btnTrangChu = CreateMenuButton("TRANG CHỦ", "trangchu.png", 100);
            btnLich = CreateMenuButton("LỊCH", "lich.png", 160);
            btnNhapVao = CreateMenuButton("NHẬP VÀO", "nhapvao.png", 220);
            btnThongKe = CreateMenuButton("THỐNG KÊ", "thongke.png", 280);
            btnDanhMuc = CreateMenuButton("DANH MỤC", "danhmuc.png", 340);
            
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
            return btn;
        }

        private Image GetIcon(string iconName)
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
