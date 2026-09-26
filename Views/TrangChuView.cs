using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using QuanLyChiTieu;
using quanlycitieu.BLL;

namespace quanlycitieu.Views
{
    public class TrangChuView : UserControl
    {
        private DashboardBLL dashboardBLL = new DashboardBLL();

        // Month Navigation
        private int selectedMonth = DateTime.Now.Month;
        private int selectedYear = DateTime.Now.Year;
        private Button btnPrevMonth;
        private Button btnNextMonth;
        private Label lblCurrentMonth;
        private Button btnToday;

        // Metric Cards
        private Label txtTongThu, txtTongChi, txtConLai;

        // Transactions Table
        private Label lblRecentTitle;
        private DataGridView dgvRecent;

        // Right Panel "Nhìn Lại"
        private Panel pnlRight;
        private Panel pnlCategoriesList;
        private Label lblNhinLaiSubtitle;

        public TrangChuView()
        {
            this.Size = new Size(980, 700);
            this.BackColor = Color.FromArgb(209, 233, 255);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // 1. TIÊU ĐỀ & CỤM ĐIỀU HƯỚNG THÁNG/NĂM
            Label lblTitle = new Label()
            {
                Text = "TỔNG QUAN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 16),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Nút Prev [<]
            btnPrevMonth = new Button()
            {
                Text = "◀",
                Location = new Point(175, 16),
                Size = new Size(34, 32),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(41, 40, 104),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPrevMonth.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnPrevMonth.Click += (s, e) => ChangeMonth(-1);
            this.Controls.Add(btnPrevMonth);

            // Label hiển thị [Tháng MM/yyyy]
            lblCurrentMonth = new Label()
            {
                Text = $"Tháng {selectedMonth:D2}/{selectedYear}",
                Location = new Point(213, 16),
                Size = new Size(130, 32),
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(lblCurrentMonth);

            // Nút Next [>]
            btnNextMonth = new Button()
            {
                Text = "▶",
                Location = new Point(347, 16),
                Size = new Size(34, 32),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(41, 40, 104),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnNextMonth.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnNextMonth.Click += (s, e) => ChangeMonth(1);
            this.Controls.Add(btnNextMonth);

            // Nút Tháng Hiện Tại
            btnToday = new Button()
            {
                Text = "Tháng Này",
                Location = new Point(388, 16),
                Size = new Size(95, 32),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(46, 125, 50),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnToday.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnToday.Click += (s, e) =>
            {
                selectedMonth = DateTime.Now.Month;
                selectedYear = DateTime.Now.Year;
                UpdateMonthDisplay();
                LoadData();
            };
            this.Controls.Add(btnToday);

            // 2. 3 THẺ CHỈ SỐ TỔNG THU - TỔNG CHI - CÒN LẠI
            Panel CreateCard(string title, int x, Color titleColor, out Label valLabel)
            {
                Panel p = new Panel()
                {
                    Location = new Point(x, 62),
                    Size = new Size(198, 92),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                Label l = new Label()
                {
                    Text = title,
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    Location = new Point(14, 12),
                    AutoSize = true,
                    ForeColor = titleColor
                };
                valLabel = new Label()
                {
                    Location = new Point(14, 42),
                    Size = new Size(170, 32),
                    Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                    ForeColor = titleColor,
                    TextAlign = ContentAlignment.MiddleRight,
                    Text = "0 đ"
                };
                p.Controls.Add(l);
                p.Controls.Add(valLabel);
                return p;
            }

            var p1 = CreateCard("TỔNG THU", 20, Color.FromArgb(46, 125, 50), out txtTongThu);
            var p2 = CreateCard("TỔNG CHI", 230, Color.FromArgb(220, 38, 38), out txtTongChi);
            var p3 = CreateCard("CÒN LẠI", 440, Color.FromArgb(37, 99, 235), out txtConLai);

            this.Controls.Add(p1);
            this.Controls.Add(p2);
            this.Controls.Add(p3);

            // 3. BẢNG GIAO DỊCH TRONG THÁNG
            lblRecentTitle = new Label()
            {
                Text = $"Giao dịch trong tháng {selectedMonth:D2}/{selectedYear}",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                Location = new Point(20, 168),
                AutoSize = true
            };
            this.Controls.Add(lblRecentTitle);

            dgvRecent = new DataGridView()
            {
                Location = new Point(20, 198),
                Size = new Size(620, 480),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                RowTemplate = { Height = 34 }
            };
            this.Controls.Add(dgvRecent);

            // 4. KHUNG BÊN PHẢI (CỘT "NHÌN LẠI" - CHI TIÊU THEO DANH MỤC)
            int rightX = 660;
            pnlRight = new Panel()
            {
                Location = new Point(rightX, 16),
                Size = new Size(300, 662),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(41, 40, 104)
            };

            Label lblNhinLai = new Label()
            {
                Text = "NHÌN LẠI",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(18, 14),
                AutoSize = true
            };
            pnlRight.Controls.Add(lblNhinLai);

            lblNhinLaiSubtitle = new Label()
            {
                Text = $"Thu & Chi theo danh mục ({selectedMonth:D2}/{selectedYear})",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 205, 235),
                Location = new Point(19, 44),
                AutoSize = true
            };
            pnlRight.Controls.Add(lblNhinLaiSubtitle);

            pnlCategoriesList = new Panel()
            {
                Location = new Point(10, 72),
                Size = new Size(280, 580),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            pnlRight.Controls.Add(pnlCategoriesList);

            this.Controls.Add(pnlRight);
        }

        private void ChangeMonth(int delta)
        {
            selectedMonth += delta;
            if (selectedMonth > 12)
            {
                selectedMonth = 1;
                selectedYear++;
            }
            else if (selectedMonth < 1)
            {
                selectedMonth = 12;
                selectedYear--;
            }

            UpdateMonthDisplay();
            LoadData();
        }

        private void UpdateMonthDisplay()
        {
            lblCurrentMonth.Text = $"Tháng {selectedMonth:D2}/{selectedYear}";
            lblRecentTitle.Text = $"Giao dịch trong tháng {selectedMonth:D2}/{selectedYear}";
            lblNhinLaiSubtitle.Text = $"Thu & Chi theo danh mục ({selectedMonth:D2}/{selectedYear})";
        }

        public void LoadData()
        {
            try
            {
                int userId = AuthForm.CurrentUserId;

                // 1. Nạp tổng thu, tổng chi, còn lại theo tháng
                var (thu, chi, conLai) = dashboardBLL.GetDashboardSummary(userId, selectedMonth, selectedYear);

                txtTongThu.Text = thu.ToString("N0") + " đ";
                txtTongChi.Text = chi.ToString("N0") + " đ";
                txtConLai.Text = conLai.ToString("N0") + " đ";
                txtConLai.ForeColor = conLai >= 0 ? Color.FromArgb(37, 99, 235) : Color.FromArgb(220, 38, 38);

                // 2. Nạp danh sách giao dịch trong tháng
                DataTable dtTransactions = dashboardBLL.GetMonthlyTransactions(userId, selectedMonth, selectedYear);
                dgvRecent.DataSource = dtTransactions;

                if (dgvRecent.Columns.Contains("Mã")) dgvRecent.Columns["Mã"].Visible = false;

                if (dgvRecent.Columns.Contains("Loại"))
                {
                    dgvRecent.Columns["Loại"].HeaderText = "Phân Loại";
                    dgvRecent.Columns["Loại"].FillWeight = 22;
                    dgvRecent.Columns["Loại"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (dgvRecent.Columns.Contains("Danh Mục"))
                {
                    dgvRecent.Columns["Danh Mục"].HeaderText = "Danh Mục";
                    dgvRecent.Columns["Danh Mục"].FillWeight = 25;
                }
                if (dgvRecent.Columns.Contains("Số Tiền"))
                {
                    dgvRecent.Columns["Số Tiền"].HeaderText = "Số Tiền (VNĐ)";
                    dgvRecent.Columns["Số Tiền"].FillWeight = 25;
                    dgvRecent.Columns["Số Tiền"].DefaultCellStyle.Format = "N0";
                    dgvRecent.Columns["Số Tiền"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvRecent.Columns.Contains("Thời Gian"))
                {
                    dgvRecent.Columns["Thời Gian"].HeaderText = "Thời Gian";
                    dgvRecent.Columns["Thời Gian"].FillWeight = 28;
                    dgvRecent.Columns["Thời Gian"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    dgvRecent.Columns["Thời Gian"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (dgvRecent.Columns.Contains("Ghi Chú"))
                {
                    dgvRecent.Columns["Ghi Chú"].HeaderText = "Ghi Chú";
                    dgvRecent.Columns["Ghi Chú"].FillWeight = 30;
                }

                // Tô màu theo loại Thu/Chi
                foreach (DataGridViewRow row in dgvRecent.Rows)
                {
                    string type = row.Cells["Loại"].Value?.ToString() ?? "";
                    if (type == "Thu Nhập")
                    {
                        row.Cells["Loại"].Style.ForeColor = Color.FromArgb(46, 125, 50);
                        row.Cells["Loại"].Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                        row.Cells["Số Tiền"].Style.ForeColor = Color.FromArgb(46, 125, 50);
                        row.Cells["Số Tiền"].Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    }
                    else
                    {
                        row.Cells["Loại"].Style.ForeColor = Color.FromArgb(220, 38, 38);
                        row.Cells["Loại"].Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                        row.Cells["Số Tiền"].Style.ForeColor = Color.FromArgb(220, 38, 38);
                        row.Cells["Số Tiền"].Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    }
                }

                // 3. Nạp danh mục thu/chi cho cột "Nhìn Lại"
                pnlCategoriesList.Controls.Clear();
                DataTable dtCats = dashboardBLL.GetExpensesByCategoryInMonth(userId, selectedMonth, selectedYear);

                if (dtCats.Rows.Count == 0)
                {
                    Label lblEmpty = new Label()
                    {
                        Text = "Chưa có giao dịch nào\ntrong tháng này.",
                        Font = new Font("Segoe UI", 10.5f, FontStyle.Italic),
                        ForeColor = Color.FromArgb(210, 215, 240),
                        Location = new Point(20, 40),
                        Size = new Size(240, 50),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    pnlCategoriesList.Controls.Add(lblEmpty);
                }
                else
                {
                    int y = 5;
                    foreach (DataRow row in dtCats.Rows)
                    {
                        string cat = row["Name"]?.ToString() ?? "Khác";
                        string type = row["Type"]?.ToString() ?? "Chi Tiêu";
                        decimal total = Convert.ToDecimal(row["Total"]);
                        bool isThuNhap = (type == "Thu Nhập");

                        Panel p = new Panel()
                        {
                            Location = new Point(5, y),
                            Size = new Size(250, 48),
                            BackColor = Color.White,
                            BorderStyle = BorderStyle.FixedSingle
                        };

                        Label lblCat = new Label()
                        {
                            Text = cat,
                            Location = new Point(10, 14),
                            Size = new Size(115, 20),
                            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                            ForeColor = Color.FromArgb(40, 40, 40),
                            AutoEllipsis = true
                        };

                        Color amtColor = isThuNhap ? Color.FromArgb(46, 125, 50) : Color.FromArgb(220, 38, 38);
                        string amtPrefix = isThuNhap ? "+" : "";

                        Label lblAmt = new Label()
                        {
                            Text = $"{amtPrefix}{total:N0} đ",
                            Location = new Point(115, 14),
                            Size = new Size(125, 20),
                            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                            ForeColor = amtColor,
                            TextAlign = ContentAlignment.MiddleRight
                        };

                        p.Controls.Add(lblCat);
                        p.Controls.Add(lblAmt);
                        pnlCategoriesList.Controls.Add(p);
                        y += 54;
                    }
                }
            }
            catch { }
        }
    }
}
